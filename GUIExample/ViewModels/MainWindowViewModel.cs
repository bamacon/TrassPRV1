using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
//using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Media.Imaging; // Для Bitmap
using CommunityToolkit.Mvvm.ComponentModel; // Для ObservableObject и атрибутов
using CommunityToolkit.Mvvm.Input; // Для RelayCommand
using System;
using System.Diagnostics; // Для Debug.WriteLine
using System.Threading.Tasks; // Для асинхронных операций (если понадобятся)
using Avalonia.Controls.ApplicationLifetimes; // Для выхода из приложения
using Avalonia; // Для Application.Current
using GUIExample.Models; //мой namespace
using System.IO;
using Point = Avalonia.Point;
using MntCuda.Visualisation;
using System.Numerics;

namespace YourAppNamespace; // Замените на ваш неймспейс

public partial class MainWindowViewModel : ObservableObject
{
    // Свойство для хранения и отображения изображения
    // Атрибут [ObservableProperty] автоматически создаст свойство DisplayedImage
    // и будет уведомлять UI об изменениях
    [ObservableProperty]
    private Bitmap? _displayedImage; // ? означает, что Bitmap может быть null

    [ObservableProperty]
    private int _mouseX; // Используем int для простоты отображения
    [ObservableProperty]
    private int _mouseY;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowTitleWithSize))] // Обновлять заголовок при изменении
    private int _desiredWidth = 1000; // Значение по умолчанию
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WindowTitleWithSize))] // Обновлять заголовок при изменении
    private int _desiredHeight = 1000; // Значение по умолчанию
    public string WindowTitleWithSize => $"My Ray Tracer Viewer ({DesiredWidth}x{DesiredHeight})";

    private Vector3 _viewCenter = Vector3.Zero; // Центр области просмотра
    private float _viewHalfWidth = 12.5f;  // Половина ширины (исходя из ваших начальных точек)
    private float _viewHalfHeight = 12.5f; // Половина высоты
    private const float ZoomFactor = 1.2f; // Коэффициент масштабирования (можно сделать настраиваемым)
    private const float MinViewSize = 0.1f;  // Минимальный размер, чтобы не уйти в ноль

    [RelayCommand]
    private void OpenFile()
    {
        Debug.WriteLine("File -> Open clicked");
        // TODO: Добавить логику открытия файла изображения
        // Например, показать диалог выбора файла, загрузить изображение,
        // преобразовать его в Avalonia Bitmap и присвоить свойству DisplayedImage
        // DisplayedImage = LoadBitmapFromFile("path/to/image.png");
    }

    [RelayCommand]
    private void SaveFile()
    {
        Debug.WriteLine("File -> Save clicked");
        // TODO: Добавить логику сохранения текущего изображения (DisplayedImage)
        // Например, показать диалог сохранения файла и сохранить Bitmap
    }

    [RelayCommand]
    private void ExitApp()
    {
        Debug.WriteLine("File -> Exit clicked");
        // Корректный способ закрыть десктопное приложение Avalonia
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
        else // Запасной вариант для других типов жизненного цикла
        {
            Environment.Exit(0);
        }
    }

    [RelayCommand]
    private async Task ZoomIn()
    {
        Debug.WriteLine("Viewer -> Zoom In clicked");
        // Уменьшаем размер области просмотра
        _viewHalfWidth /= ZoomFactor;
        _viewHalfHeight /= ZoomFactor;
        // Проверка на минимальный размер
        if (_viewHalfWidth < MinViewSize) _viewHalfWidth = MinViewSize;
        if (_viewHalfHeight < MinViewSize) _viewHalfHeight = MinViewSize;

        Debug.WriteLine($"New view size: {_viewHalfWidth * 2}x{_viewHalfHeight * 2}");
        // Запускаем перерисовку с новыми параметрами
        await RenderCurrentImage(); // Вызываем новый общий метод рендеринга
    }

    [RelayCommand]
    private async Task ZoomOut()
    {
        Debug.WriteLine("Viewer -> Zoom Out clicked");
        // Увеличиваем размер области просмотра
        _viewHalfWidth *= ZoomFactor;
        _viewHalfHeight *= ZoomFactor;
        // Опционально: добавить максимальный размер

        Debug.WriteLine($"New view size: {_viewHalfWidth * 2}x{_viewHalfHeight * 2}");
        // Запускаем перерисовку с новыми параметрами
        await RenderCurrentImage(); // Вызываем новый общий метод рендеринга
    }

    [RelayCommand]
    private async Task Redraw()
    {
        Debug.WriteLine("Viewer -> Redraw clicked");
        await RenderCurrentImage();
    }

    [RelayCommand]
    private async Task RunTrace() // Используем async Task для потенциально долгой операции
    {
        Debug.WriteLine("Run clicked");
        // Опционально: сбросить зум к значениям по умолчанию?
        _viewCenter = Vector3.Zero;
        _viewHalfWidth = 12.5f;
        _viewHalfHeight = 12.5f;
        await RenderCurrentImage();
    }

    private async Task RenderCurrentImage()
    {
        Debug.WriteLine("Starting Render...");
        // IsBusy = true; (Опционально)

        Bitmap? avaloniaBitmap = null;

        try
        {
            RBMKCell1 cell = new RBMKCell1();
            TraceVisualizer vis = new TraceVisualizer(TraceVisualizer.AlgorithmEnum.CheckContains);

            int currentWidth = DesiredWidth;
            int currentHeight = DesiredHeight;

            // --- Вычисляем точки для DrawRectangle на основе _viewCenter, _viewHalfWidth, _viewHalfHeight ---
            // !! ВАЖНО: Этот расчет основан на предположении, что ByThreePoints(p1, p2, p3)
            //    использует p1 как нижний левый угол, p2 как нижний правый, p3 как верхний левый.
            //    Если это не так, адаптируйте расчет векторов!
            Vector3 p1 = _viewCenter + new Vector3(-_viewHalfWidth, -_viewHalfHeight, 0); // Bottom-left
            Vector3 p2 = _viewCenter + new Vector3(_viewHalfWidth, -_viewHalfHeight, 0); // Bottom-right
            Vector3 p3 = _viewCenter + new Vector3(-_viewHalfWidth, _viewHalfHeight, 0); // Top-left

            Debug.WriteLine($"Rendering rectangle: P1={p1}, P2={p2}, P3={p3}");

            Image<Rgb24> imageSharpImage = vis.GetImage(cell, currentWidth, currentHeight,
                TraceVisualizer.DrawRectangle.ByThreePoints(p1, p2, p3), // Используем вычисленные точки
                null
            );
            // --- Конец вызова GetImage ---


            if (imageSharpImage != null)
            {
                // --- Конвертация и обновление ---
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await imageSharpImage.SaveAsPngAsync(memoryStream);
                    memoryStream.Position = 0;
                    avaloniaBitmap = new Bitmap(memoryStream);
                }
                imageSharpImage.Dispose();

                DisplayedImage?.Dispose();
                DisplayedImage = avaloniaBitmap;
                Debug.WriteLine("Render completed and image updated.");
                // --- Конец конвертации и обновления ---
            }
            else
            {
                Debug.WriteLine("Render completed but GetImage returned null.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error during render/image conversion: {ex.Message}\n{ex.StackTrace}");
            // TODO: Показать ошибку пользователю
        }
        finally
        {
            // IsBusy = false; (Опционально)
        }
    }

    [RelayCommand]
    private async Task ShowData(object? parameter)
    {
        Debug.WriteLine("Data clicked");

        // Получаем родительское окно из параметра команды
        var parentWindow = parameter as Window;
        if (parentWindow == null)
        {
            Debug.WriteLine("Could not get parent window instance for the dialog.");
            // Возможно, показать сообщение об ошибке?
            return;
        }

        // Создаем ViewModel для диалога, передавая ТЕКУЩИЕ желаемые размеры
        var dialogViewModel = new ImageSizeSettingsViewModel(this.DesiredWidth, this.DesiredHeight);

        // Создаем окно диалога
        var dialog = new ImageSizeSettingsWindow
        {
            DataContext = dialogViewModel
        };

        // Показываем диалог модально и ждем результат (true если OK, false/null если Отмена)
        var result = await dialog.ShowDialog<bool?>(parentWindow);

        if (result == true)
        {
            // Пользователь нажал OK, обновляем размеры в главном ViewModel
            this.DesiredWidth = dialogViewModel.ImageWidth;
            this.DesiredHeight = dialogViewModel.ImageHeight;
            Debug.WriteLine($"New image size set: {DesiredWidth}x{DesiredHeight}");
        }
        else
        {
            // Пользователь нажал Отмена или закрыл окно
            Debug.WriteLine("Image size setting cancelled.");
        }
    }
    
    public void UpdateMouseCoordinates(Point position)
    {
        MouseX = (int)Math.Round(position.X); // Округляем до целого
        MouseY = (int)Math.Round(position.Y);
    }
    public void ClearMouseCoordinates()
    {
        // Можно установить в -1 или какое-то другое значение по умолчанию
        MouseX = -1;
        MouseY = -1;
        // Или оставить последние координаты, просто не обновлять их
    }

    


}