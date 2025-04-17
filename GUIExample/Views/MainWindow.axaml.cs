using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;

namespace YourAppNamespace; // �������� �� ��� ���������

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        
    }
    private void ImageArea_PointerMoved(object? sender, PointerEventArgs e)
    {
        // �������� ViewModel �� DataContext ����
        var viewModel = this.DataContext as MainWindowViewModel;
        if (viewModel == null) return; // �������, ���� ViewModel �� �������

        // �������� ������� ����������, ��� ������� ��������� ������� (��� ��� Border)
        var control = sender as Control;
        if (control == null) return;

        // �������� ���������� ���� ������������ ����� �������� ����������
        Point currentPoint = e.GetPosition(control);

        // ��������� ���������� � ViewModel
        viewModel.UpdateMouseCoordinates(currentPoint);
    }

    // ���������� ����� ���� �� ������� ImageHostArea
    private void ImageArea_PointerExited(object? sender, PointerEventArgs e)
    {
        // �������� ViewModel
        var viewModel = this.DataContext as MainWindowViewModel;
        if (viewModel == null) return;

        // ������� ���������� (��� ������������� �������� �� ���������)
        viewModel.ClearMouseCoordinates();
    }
}