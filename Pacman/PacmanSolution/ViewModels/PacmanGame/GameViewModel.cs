using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;
using PacmanSolution.Models.Game;
using PacmanSolution.ViewModels.Pacman;

namespace PacmanSolution.ViewModels;

public partial class GameViewModel: ObservableObject
{
    [ObservableProperty] private PacmanViewModel _pacman;
    [ObservableProperty] private GhostViewModel _ghosts;
    [ObservableProperty] private ScoreBoardViewModel _score;
    [ObservableProperty] private ManagePageChange _navigation;
    [ObservableProperty] private object _currentSubView;
    [ObservableProperty] private bool _showWinOverlay = false;
    [ObservableProperty] private bool _showGameOverOverlay = false;
    [ObservableProperty] private string _finalScoreText = "";
    [ObservableProperty] private bool _isMusicEnabled;
    [ObservableProperty] private string _playerName = string.Empty;
    [ObservableProperty] private string _saveMessage = string.Empty;
    [ObservableProperty] private bool _isSaveInputVisible = false;
    
    private readonly SoundManager soundManager =new();    
    private readonly GameEngine _engine = new();
    private readonly EngineManager _engineManager;
    private readonly GameBoardSyncService _syncService;
    private const string _nameSprite = null;
    private int _countLivePacman=3;
    private DispatcherTimer _gameTimer;
    private DispatcherTimer? _gameLoopTimer;
    private DispatcherTimer _movementTimer;
    private DispatcherTimer? _frightenedTimer;
    public ObservableCollection<GameObject> GameObjects => _engine.VisualObjects;
    private ObservableCollection<Entity> Board { get; set; } = new();

    public GameViewModel(ManagePageChange navigation)
    {
        _navigation = navigation;//navega en paginas
        Board.Clear();
        CurrentSubView = this;// esto es del navegador
        Score = new ScoreBoardViewModel(navigation);// es el score board que luego me toca hacerlo 
        _engineManager = new EngineManager(28, 31);//pasa parametros
        _engineManager.BuildGameBoard(Board);// lo contruye
        
        _syncService = new GameBoardSyncService(_engine.VisualObjects);// hace visual los objectos
        _syncService.BuildFromBoard(Board);// locontruye para lo visual ahora
        
        Pacman = new PacmanViewModel(_engine, Board, _movementTimer, Score, _syncService);
        Ghosts = new GhostViewModel(Board, _syncService,Pacman.PacmanModel, _engine, Score);
        _syncService.RegisterGhosts(Ghosts.Ghosts);
        StartGameLoop();// empieza el loop posiblement elo que tenga que urgar para las vidas
        // Asegurar sprite en UI cuando la vista ya esté cargada (por si el binding se actualiza después)
        Dispatcher.UIThread.Post(() => Pacman.UpdatePacmanSprites(), DispatcherPriority.Loaded);
    }
    
    [RelayCommand]
    private void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }
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
        ResumeAllTimers();
    }
    
    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        PauseAllTimers();
    }
}