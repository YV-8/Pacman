using System;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.ViewModels;

namespace PacmanSolution.Models;

public partial class ManagePageChange: ObservableObject
{
    [ObservableProperty]
    private object? _currentPage;
    
    public void ChangePage(string initialsPage)
    {
        switch (initialsPage)
        {
            case"GoGame":
                CurrentPage = new PacmanGameViewModel(this);
                break;
            case"GoScoreBoard":
                CurrentPage = new ScoreBoardPageViewModel();
                break;
            case"GoSettingsMenu":
                CurrentPage = new GoSettingsViewModel();
                break;
            case"Menu":
                CurrentPage = new MainWindowViewModel();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(initialsPage), initialsPage, null);
        }
    }
    
    /*public MainWindowViewModel()
    {
        CurrentPage = new SettingPageViewModel();

    }*/
}