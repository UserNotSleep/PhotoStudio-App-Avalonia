using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PhotoStudio.Views;

public partial class PhotographerDashboardView : UserControl
{
    public PhotographerDashboardView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
