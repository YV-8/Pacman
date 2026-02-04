using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PacmanSolution.ViewModels;

namespace PacmanSolution.Views;

public partial class MenuPageView : UserControl
{
    public MenuPageView()
    {
        InitializeComponent();
        //DataContext = new MenuPageViewModel();
    }
}