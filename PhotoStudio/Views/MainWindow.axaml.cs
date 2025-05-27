using Avalonia.Controls;
using PhotoStudio.ViewModels;

namespace PhotoStudio.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new AuthViewModel();
    }
}