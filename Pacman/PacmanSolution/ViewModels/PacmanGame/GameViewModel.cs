using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;
using PacmanSolution.ViewModels.Pacman;
using PacmanSolution.Views;

namespace PacmanSolution.ViewModels;

public partial class GameViewModel: ObservableObject
{
    [ObservableProperty] private PacmanViewModel _pacman;
    [ObservableProperty] private GhostViewModel _ghosts;
    [ObservableProperty] private ScoreBoardPageViewModel _score;
    [ObservableProperty] private ManagePageChange _navigation;
    [ObservableProperty] private object _currentSubView;
    
    private readonly SoundManager _soundManager = new ();
    private readonly GameEngine _engine = new();
    private readonly EngineManager _engineManager;
    private readonly GameBoardSyncService _syncService;
    private DispatcherTimer _gameTimer;
    private DispatcherTimer? _gameLoopTimer;
    private DispatcherTimer _movementTimer;
    private const string _nameSprite = null;
    public ObservableCollection<GameObject> GameObjects => _engine.VisualObjects;

    private ObservableCollection<Entity> _board = new();
    public ObservableCollection<Entity> Board
    {
        get => _board;
        set => _board = value;
    }

    public GameViewModel(ManagePageChange navigation)
    {
        _navigation = navigation;
        Board.Clear();
        Score = new ScoreBoardPageViewModel();
        _engineManager = new EngineManager(28, 31);
        _engineManager.BuildGameBoard(Board);
        
        _syncService = new GameBoardSyncService(_engine.VisualObjects);
        _syncService.BuildFromBoard(Board);
        _movementTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        Pacman = new PacmanViewModel(_engine, Board, _movementTimer, Score, _syncService);
        Pacman.UpdatePacmanSprites(); // Sprite inicial para que Pacman se vea
        Ghosts = new GhostViewModel(Board);
        StartGameLoop();
        CurrentSubView = this;
        // Asegurar sprite en UI cuando la vista ya esté cargada (por si el binding se actualiza después)
        Avalonia.Threading.Dispatcher.UIThread.Post(() => Pacman.UpdatePacmanSprites(), Avalonia.Threading.DispatcherPriority.Loaded);
    }
    
    [RelayCommand]
    private void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }
    [RelayCommand]
    private void ToggleAudioCommand( bool isChecked)
    {
        var path = "PacmanTheme";
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
    /// stop and clean the timers
    /// </summary>
    public void CleanupTimers()
    {
        //_animationTimer?.Stop();
        _movementTimer?.Stop();
        _gameTimer?.Stop();
    }
    /// <summary>
    /// Resume game with time 
    /// </summary>
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