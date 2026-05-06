using System.Windows;
using System.Windows.Controls;
using DeploymentTool.ViewModels;

namespace DeploymentTool;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void LogBox_TextChanged(object sender, TextChangedEventArgs e)
        => LogScroller.ScrollToBottom();

    private void ClearLog_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.LogText = string.Empty;
    }
}
