using Avalonia.Controls;
using Avalonia.Interactivity; // Для RoutedEventArgs

namespace YourAppNamespace; // Замените на ваш неймспейс

public partial class ImageSizeSettingsWindow : Window
{
    public ImageSizeSettingsWindow()
    {
        InitializeComponent();

        // Находим кнопки по имени и добавляем обработчики клика
        var okButton = this.FindControl<Button>("OkButton");
        var cancelButton = this.FindControl<Button>("CancelButton");

        if (okButton != null)
            okButton.Click += OkButton_Click;
        if (cancelButton != null)
            cancelButton.Click += CancelButton_Click;
    }

    // Обработчик кнопки OK - закрываем окно с результатом true
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close(true); // Передаем true, чтобы показать, что пользователь нажал OK
    }

    // Обработчик кнопки Cancel - закрываем окно с результатом false
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close(false); // Передаем false (или можно null) для отмены
    }
}