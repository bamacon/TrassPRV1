using Avalonia.Controls;
using Avalonia.Interactivity; // ��� RoutedEventArgs

namespace YourAppNamespace; // �������� �� ��� ���������

public partial class ImageSizeSettingsWindow : Window
{
    public ImageSizeSettingsWindow()
    {
        InitializeComponent();

        // ������� ������ �� ����� � ��������� ����������� �����
        var okButton = this.FindControl<Button>("OkButton");
        var cancelButton = this.FindControl<Button>("CancelButton");

        if (okButton != null)
            okButton.Click += OkButton_Click;
        if (cancelButton != null)
            cancelButton.Click += CancelButton_Click;
    }

    // ���������� ������ OK - ��������� ���� � ����������� true
    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close(true); // �������� true, ����� ��������, ��� ������������ ����� OK
    }

    // ���������� ������ Cancel - ��������� ���� � ����������� false
    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close(false); // �������� false (��� ����� null) ��� ������
    }
}