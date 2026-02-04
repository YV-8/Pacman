using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PacmanSolution.ViewModels;

public partial class MenuPageViewModel
{
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    public MenuPageViewModel(MainWindowViewModel mainWVM)
    {
        _mainWindowViewModel = mainWVM;
    }
    public void GoGame()
    {
        _mainWindowViewModel.CurrentPage = new GamePageViewModel();
    }

    public void GoScoreBoard()
    {
        _mainWindowViewModel.CurrentPage = new ScoreBoardPageViewModel();
    }

    public void GoSettingsMenu()
    {
        //cambair por una ventana emergente
        _mainWindowViewModel.CurrentPage = new GoSettingsViewModel();
    }

    public void ExitGame()
    {
        Console.WriteLine("Exit...Game...");
    }
    


}