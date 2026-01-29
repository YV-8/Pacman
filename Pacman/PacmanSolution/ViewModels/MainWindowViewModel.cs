using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PacmanSolution.ViewModels;

public partial class MainWindowViewModel:ViewModelBase
{
    [ObservableProperty]
    private object _currentPage;

    public MainWindowViewModel()
    {
        CurrentPage = new MainPageViewM(this);
    }
    
}