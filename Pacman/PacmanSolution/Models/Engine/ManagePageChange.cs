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
                CurrentPage = new GameViewModel(this);
                break;
            case"GoScoreBoard":
                CurrentPage = new ScoreBoardViewModel();
                break;
            case"GoSettingsMenu":
                CurrentPage = new GoSettingViewModel();
                break;
            case"Menu":
                CurrentPage = new MainWindowViewModel();
                break;
            case "Exit":
                Environment.Exit(0);
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