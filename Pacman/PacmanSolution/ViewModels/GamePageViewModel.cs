using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel: ObservableObject
{
    [ObservableProperty]
    private ManagePageChange _navigation;
    [ObservableProperty]
    private IImage? _pacmanCurrentSprite;
    private EngineManager _engineManager;
    private SoundManager _soundManager = new ();
    private SpriteManager _spriteManager = new ();
    private DispatcherTimer _gameTimer;
    private DispatcherTimer? _gameLoopTimer;
    private ObservableCollection<Entity> _board = new();
    public ObservableCollection<Entity> Board
    {
        get => _board;
        set => _board = value;
    }

    public GamePageViewModel(ManagePageChange navigation)
    {
        /*InitializePacmanPosition();
        _gameLoopTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
        _gameLoopTimer.Tick += (s, e) => {
            MovePacman();
        };*/
        _navigation = navigation;
        Board.Clear();
        _engineManager = new EngineManager(28,31);//31*32
        Score = 0;
        HighScore = 0;
        _engineManager.BuildGameBoard(Board);
        InitializePacmanPosition();
        StartGameLoop();
        StartMovementTimer();
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
    /// <summary>
    /// Detiene y limpia los timers
    /// </summary>
    public void CleanupTimers()
    {
        _animationTimer?.Stop();
        _movementTimer?.Stop();
        _gameTimer?.Stop();
    }
    
    public void ResumeGame()
    {
        _gameTimer?.Start();
        _movementTimer?.Start();
    }
    
    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        _gameTimer?.Stop();
        _movementTimer?.Stop();
    }
}