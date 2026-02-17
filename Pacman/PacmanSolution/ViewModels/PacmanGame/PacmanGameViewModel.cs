using System;
using System.Collections.ObjectModel;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class PacmanGameViewModel: ObservableObject
{
    [ObservableProperty]
    private ManagePageChange _navigation;
    [ObservableProperty]
    private IImage? _pacmanCurrentSprite;
    [ObservableProperty]
    private ScoreBoardPageViewModel _scoreViewModel = new();
    private readonly SoundManager _soundManager = new ();
    private readonly SpriteManager _spriteManager = new ();
    private readonly GameEngine _engine = new();
    private EngineManager _engineManager;
    private DispatcherTimer _gameTimer;
    private DispatcherTimer? _gameLoopTimer;
    private ObservableCollection<Entity> _board = new();
    public ObservableCollection<Entity> Board
    {
        get => _board;
        set => _board = value;
    }

    public PacmanGameViewModel(ManagePageChange navigation)
    {
        _navigation = navigation;
        Board.Clear();
        _engineManager = new EngineManager(28,31);//31*32
        
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
        _animationTimer?.Stop();
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
    private void UpdatePacmanCanvasPosition()
    {
        var (centerX, centerY) = GetCellCenter(PacmanRow, PacmanCol);
        PacmanCanvasLeft = centerX - (PacmanImageSize / 2);
        PacmanCanvasTop = centerY - (PacmanImageSize / 2);
    }
    /// <summary>
    /// Get the row and col and order in the canvas
    /// </summary>
    /// <param name="row"/>
    /// <param name="col"/>
    /// <returns></returns>
    public static (double x, double y) GetCellCenter(double row, double col)
    {
        var x = OffsetX + (col * CellSize) + (CellSize / 2);
        var y = OffsetY + (row * CellSize) + (CellSize / 2);
        return (x, y);
    }
}