using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class MainWindowViewModel:ObservableObject
{
    [ObservableProperty]
    private ManagePageChange _navigation;
    [ObservableProperty]
    private bool _isMusicEnabled;
    private readonly SoundManager soundManager =new();    


    public MainWindowViewModel()
    {
        Navigation = new ManagePageChange();
    }
    [RelayCommand]
    private void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }

    [RelayCommand]
    public void ExitGame()
    {
        if (Avalonia.Application.Current?.ApplicationLifetime 
            is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
    /// <summary>
    /// audio
    /// </summary>
    [RelayCommand]
    private void ToggleAudio()
    {
        var path = "PacManGameSound";
        if (_isMusicEnabled)
        {
            soundManager.PlaySound(path, true);
        }
        else
            soundManager.StopSound();
    }
}