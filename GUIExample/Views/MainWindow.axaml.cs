using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;

namespace YourAppNamespace; // Замените на ваш неймспейс

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        
    }
    private void ImageArea_PointerMoved(object? sender, PointerEventArgs e)
    {
        // Получаем ViewModel из DataContext окна
        var viewModel = this.DataContext as MainWindowViewModel;
        if (viewModel == null) return; // Выходим, если ViewModel не найдена

        // Получаем элемент управления, над которым произошло событие (это наш Border)
        var control = sender as Control;
        if (control == null) return;

        // Получаем координаты мыши относительно этого элемента управления
        Point currentPoint = e.GetPosition(control);

        // Обновляем координаты в ViewModel
        viewModel.UpdateMouseCoordinates(currentPoint);
    }

    // Обработчик ухода мыши из области ImageHostArea
    private void ImageArea_PointerExited(object? sender, PointerEventArgs e)
    {
        // Получаем ViewModel
        var viewModel = this.DataContext as MainWindowViewModel;
        if (viewModel == null) return;

        // Очищаем координаты (или устанавливаем значения по умолчанию)
        viewModel.ClearMouseCoordinates();
    }
}