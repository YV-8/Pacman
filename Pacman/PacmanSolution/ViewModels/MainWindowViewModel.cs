using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class MainWindowViewModel:ObservableObject
{
    [ObservableProperty]
    private ManagePageChange _navigation;
    private SoundManager soundManager = new();

    public MainWindowViewModel()
    {
        Navigation = new ManagePageChange();
    }
    [RelayCommand]
    public void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }

    [RelayCommand]
    public void ExitGame()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
    /// <summary>
    /// audio
    /// </summary>
    [RelayCommand]
    public void ToggleAudioCommand( bool isChecked)
    {
        string path = "PacmanTheme";
        if (isChecked)
        {
            soundManager.PlaySound(path,true);
        }
        else
        {
            soundManager.StopSound();
        }
    }
}