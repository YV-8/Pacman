using System;
using System.Collections.ObjectModel;

using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel: ObservableObject
{
    [ObservableProperty]
    private int _score;
    [ObservableProperty]
    private int _highScore;
    [ObservableProperty]
    private ManagePageChange _navigation;
    [ObservableProperty]
    private IImage? _pacmanCurrentSprite;
    private EngineManager _engineManager;
    private ObservableCollection<Entity> _board = new();
    private SoundManager _soundManager = new ();
    private SpriteManager _spriteManager = new ();
    private DispatcherTimer _gameTimer;
    private int _animationFrame = 0;

    public ObservableCollection<Entity> Board
    {
        get => _board;
        set => _board = value;
    }

    public GamePageViewModel(ManagePageChange navigation)
    {
        _navigation = navigation;
        Board.Clear();
        _engineManager = new EngineManager(28,31);//31*32
        Score = 0;
        HighScore = 0;
        
        _engineManager.BuildGameBoard(Board);
        StartGameLoop();
    }
    
    [RelayCommand]
    private void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }
    [RelayCommand]
    public void ToggleAudioCommand( bool isChecked)
    {
        string path = "PacmanTheme";
        if (isChecked)
        {
            _soundManager.PlaySound(path,true);
        }
        else
        {
            _soundManager.StopSound();
        }
    }
    
}