using CommunityToolkit.Mvvm.ComponentModel;

namespace PacmanSolution.ViewModels;

public partial class MainWindowViewModel:ObservableObject
{
    [ObservableProperty]
    private object _currentPage;

    public MainWindowViewModel()
    {
        CurrentPage = new MenuPageViewModel(this);
        
    }
    
}