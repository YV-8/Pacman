using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Models;
using PacmanSolution.Models.Game;

namespace PacmanSolution.ViewModels.Pacman;

public partial class PacmanViewModel:ObservableObject
{
    [ObservableProperty] private double _canvasLeft;
    [ObservableProperty] private double _canvasTop;
    [ObservableProperty] private IImage? _currentSource;
    [ObservableProperty] private ScoreBoardPageViewModel _score;
    [ObservableProperty]
    private IImage? _pacmanCurrentSprite;
    [ObservableProperty] private Models.Pacman _pacmanModel;
    private readonly GameEngine _engine;
    private readonly ObservableCollection<Entity> _board;
    private readonly GameBoardSyncService _syncService;
    private readonly SpriteManager _spriteManager = new ();
    private DispatcherTimer? _animationTimer;
    private readonly DispatcherTimer? _movementTimer;
    
    private int _row;
    private int _col;
    private int _currentSpriteRow = 0;
    private int _animationFrame = 0;
    private bool _isAutoMode = true;
    private const double PacmanImageSize = 40;
    public event Action? OnEnergizerEaten;

    public int Row 
    { 
        get => _row; 
        set { _row = value; 
            PacmanModel.UpdateCanvasPosition();
            OnPropertyChanged(nameof(Row)); } 
    }
    public int Col 
    { 
        get => _col; 
        set { _col = value; PacmanModel.UpdateCanvasPosition();
            OnPropertyChanged(nameof(Col)); } 
    }
    public PacmanViewModel(GameEngine engine, ObservableCollection<Entity> board,
        DispatcherTimer movementTimer, ScoreBoardPageViewModel score,
        GameBoardSyncService syncService)
    {
        _engine = engine;
        _board = board;
        _movementTimer = movementTimer;
        _syncService = syncService;
        Score = score;
        var pacmanCell = board.FirstOrDefault(e => e.Type == EntityType.PACMAN);
        int startRow = pacmanCell?.Row ?? 0;
        int startCol = pacmanCell?.Col ?? 0;
        _row = startRow;
        _col = startCol;
        PacmanModel = new Models.Pacman(startRow, startCol, EntityType.PACMAN, PacmanImageSize, PacmanImageSize, 10);
    }
    
    
    
    public void GetDirection(string direction)
    {
        if (_isAutoMode)
        {
            _isAutoMode = false;
            _movementTimer?.Stop();
            Console.WriteLine("Control manual activado. Timer detenido.");
        }
        int oldRow = _currentSpriteRow;
        _currentSpriteRow = _pacmanModel.ChangeDirection(direction, _currentSpriteRow);
        if (oldRow != _currentSpriteRow)
        {
            UpdatePacmanSprites();
        }

        GetMovePacman();
    }
    
    /// <summary>
    /// GetMovePacman get the positioni and know if pacman eat a dot or energize
    /// </summary>
    public void GetMovePacman()
    {
        var (nextRow, nextCol) = _pacmanModel.MovePacman(Row,Col);
        var targetEntity = _board.FirstOrDefault(c => c.Row == nextRow && c.Col == nextCol);
        Console.WriteLine($"[CanMove] target=({nextRow},{nextCol}) " +
                          $"type={targetEntity?.Type} " +
                          $"typeInt={(int?)targetEntity?.Type} " +
                          $"isGhost={targetEntity is Ghost} " +
                          $"instanceType={targetEntity?.GetType().Name}");
        if (targetEntity is null || _engine.CanMoveTo(targetEntity))
        {
            if (targetEntity is not null)
            {
                var result = _engine.InteractionObjects(targetEntity);
                if (result.PointsEarned > 0)
                {
                    Score.amount(result.PointsEarned);
                    targetEntity.IsActive = false;
                    if (targetEntity is Pellet { IsEnergizer: true })
                    {
                        // Disparar evento o llamar directamente
                        OnEnergizerEaten?.Invoke();
                    }
                }
            }
            UpdatePacmanPosition(_animationFrame, nextRow, nextCol);
        }
        else
        {
            if (_isAutoMode)
            {
                _isAutoMode = false;
                _movementTimer?.Stop();
            }
        }
    }
    
    /// <summary>
    /// UpdateSprites is a method modification who sprite is to get
    /// whit _SpriteManager.GetSpritesSection with the Nae path and the rect is rectangle
    /// which change the board space had assigned as CellType.Pacman
    /// _animationFrame is the parr is between 0 - 1 
    /// </summary>
    public void UpdatePacmanSprites()
    {
        _animationFrame = (_animationFrame + 1) % 2;

        int _size = 32; 
        var rect = new PixelRect(_animationFrame * _size, _currentSpriteRow * _size, _size, _size);
        var newSprite  = _spriteManager.GetSpriteSection("PacmanViews.png", rect);
        
        if (newSprite is not null)
        {
            PacmanCurrentSprite = newSprite;
            _syncService.UpdatePacmanSprite(newSprite);
        }
    }
    
    private void UpdatePacmanPosition(int animationFrame, int newRow, int newCol)
    {
        _animationFrame = (animationFrame + 1) % 2;
        var oldCell = _board.FirstOrDefault(c => c.Row == Row && c.Col == Col);
        var newCell = _board.FirstOrDefault(c => c.Row == newRow && c.Col == newCol);
        
        if (oldCell is null || newCell is null)
            return;
        if (oldCell.Type == EntityType.PACMAN)
            oldCell.Type = EntityType.EMPTY;
        if (newCell is not Ghost)
            newCell.Type = EntityType.PACMAN;
        Row = newRow;
        Col = newCol;
        PacmanModel.Row = newRow;
        PacmanModel.Col = newCol;
        _syncService.UpdatePacmanPosition(newRow, newCol);
        //animationFrame = (animationFrame + 1) % 2;
    }
    
}