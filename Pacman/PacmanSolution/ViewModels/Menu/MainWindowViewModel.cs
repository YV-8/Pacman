using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels.Menu;

public partial class MainWindowViewModel:ObservableObject
{
    [ObservableProperty]
    private ManagePageChange _navigation;
    [ObservableProperty]
    private bool _isMusicEnabled;
    private readonly SoundManager _soundManager =new();    

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
    private void ExitGame()
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
            _soundManager.PlaySound(path, true);
        }
        else
            _soundManager.StopSound();
    }
}