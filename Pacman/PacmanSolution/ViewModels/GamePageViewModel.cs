using System;
using System.Collections.ObjectModel;
using Avalonia;
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
    private BoardManager _boardManager;
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
        _boardManager = new BoardManager(28,31);//31*32
        Score = 0;
        HighScore = 0;
        
        _boardManager.BuildGameBoard(Board);
        StartGameLoop();
    }
    [RelayCommand]
    private void ToggleMusic() { /* Lógica aquí */ }

    //[RelayCommand]
    //private void OpenSettings() { /* Lógica aquí */ }

    [RelayCommand]
    private void ViewScoresCommand()
    {
        
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

    private void StartGameLoop()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(150) // Velocidad de la animación
        };
        _gameTimer.Tick += (s, e) => UpdateSprites();
        _gameTimer.Start();
    }
    
}