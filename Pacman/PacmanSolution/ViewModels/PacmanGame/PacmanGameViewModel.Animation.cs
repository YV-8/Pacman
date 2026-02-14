using System;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Models;
using PacmanSolution.Views;

namespace PacmanSolution.ViewModels;

public partial class PacmanGameViewModel
{
    [ObservableProperty]
    private int _pacmanRow;
    [ObservableProperty]
    private int _pacmanCol;
    [ObservableProperty]
    private double _pacmanCanvasLeft;
    [ObservableProperty]
    private double _pacmanCanvasTop;
    [ObservableProperty]
    private int _score;
    [ObservableProperty]
    private int _highScore;
    
    private int _currentSpriteRow = 0;
    private int _animationFrame = 0;
    private int _totalScore=1200;
    private int _totalScoreCherry=1500;
    private int _scoreCherry=0;
    
    private const double CellSize = 45.8;
    private const double OffsetX = 175;
    private const double OffsetY = 15;
    private const double PacmanImageSize = 40;
    private string _currentDirection = "RIGHT";
    
    private bool _isAutoMode = true;
    private DispatcherTimer _movementTimer;
    private DispatcherTimer? _animationTimer;
    public event EventHandler<PacmanGameView.ElementRemovedEventArgs>? OnElementRemoved;

    
    /// <summary>
    /// Initializes Pacman's position from the board
    /// </summary>
    private void InitializePacmanPosition()
    {
        var pacmanCell = _board.FirstOrDefault(c => c.Type == EntityType.PACMAN);
        if (pacmanCell is not null)
        {
            _pacmanRow = pacmanCell.Row;
            _pacmanCol = pacmanCell.Col;
            UpdatePacmanCanvasPosition();
        }
    }
    
    private void StartGameLoop()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(150)
        };
        _gameTimer.Tick += (s, e) => UpdateSprites();
        _gameTimer.Start();
    }
    
    private void StartMovementTimer()
    {
        _movementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200) // speed move
        };
        _movementTimer.Tick += (s, e) => MovePacman();
        _movementTimer.Start();
    }
    
    /// <summary>
    /// UpdateSprites is a method modification who sprite is to get
    /// whit _SpriteManager.GetSpritesSection with the Nae path and the rect is rectangle
    /// which change the board space had assigned as CellType.Pacman
    /// _animationFrame is the parr is between 0 - 1 
    /// </summary>
    private void UpdateSprites()
    {
        _animationFrame = (_animationFrame + 1) % 2;

        int _size = 16; 
        var rect = new PixelRect(
            _animationFrame * _size,
            _currentSpriteRow * _size,
            _size, 
            _size);
        
        var newSprite  = _spriteManager.GetSpriteSection("PacmanViews.png", rect);
        
        if (newSprite is not null)
        {
            PacmanCurrentSprite = newSprite;
            
            foreach (var entity in Board)
            {
                if (entity.Type == EntityType.PACMAN)
                {
                    entity.CurrentDisplaySprite = newSprite;
                }
            }
        }
    }
    /// <summary>
    /// Method change the direction if oldRow is different the actual direction
    /// </summary>
    /// <param name="direction"></param>
    public void ChangeDirection(string direction)
    {
        if (_isAutoMode)
        {
            _isAutoMode = false;
            _movementTimer?.Stop();
            Console.WriteLine("Control manual activado. Timer detenido.");
        }
        _currentDirection = direction.ToUpper();
        int oldRow = _currentSpriteRow;
        switch (_currentDirection)
        {
            case "RIGHT":
                _currentSpriteRow = 0;
                break;
            case "LEFT":
                _currentSpriteRow = 1;
                break;
            case "UP":
                _currentSpriteRow = 2;
                break;
            case "DOWN":
                _currentSpriteRow = 3;
                break;
        }
        if (oldRow != _currentSpriteRow)
        {
            UpdateSprites();
        }
        MovePacman();
    }


    /// <summary>
    /// Move Pacman in the current direction
    /// </summary>
    private void MovePacman()
    {
        int nextRow = PacmanRow;
        int nextCol = PacmanCol;
        switch (_currentDirection)
        {
            case "UP": 
                nextRow--;
                break;
            case "DOWN":
                nextRow++;
                break;
            case "LEFT":
                nextCol--;
                break;
            case "RIGHT":
                nextCol++;
                break;
        }
        var targetEntity = Board.FirstOrDefault(c => c.Row == nextRow && c.Col == nextCol);
        if (targetEntity is not null && _engine.CanMoveTo(targetEntity))
        {
            Console.WriteLine($"Intentando mover a {nextRow}, {nextCol}");
            var result = _engine.InteractionObjects(targetEntity);
            if (result.PointsEarned > 0) {
                Score += result.PointsEarned;
                if (Score > HighScore) HighScore = Score;
                Console.WriteLine($"🎯 +{result.PointsEarned} puntos! Score: {Score}");
                OnElementRemoved?.Invoke(this, new PacmanGameView.ElementRemovedEventArgs(
                    result.RemovedElementType, nextRow, nextCol));
            }
            UpdatePacmanPosition(_animationFrame, nextRow, nextCol);
        }
        else
        {
            if (_isAutoMode)
            {
                _isAutoMode = false;
                _movementTimer?.Stop();
                
                if (targetEntity == null)
                {
                    Console.WriteLine($"🛑 MODO AUTOMÁTICO DETENIDO - Fuera del tablero en ({nextRow}, {nextCol})");
                }
                else
                {
                    Console.WriteLine($"🛑 MODO AUTOMÁTICO DETENIDO - Chocó con {targetEntity.Type} en ({nextRow}, {nextCol})");
                }
                
                Console.WriteLine("💡 Ahora puedes controlarlo manualmente con las teclas");
            }
            else
            {
                // En modo manual, solo mostrar el bloqueo
                if (targetEntity == null)
                {
                    Console.WriteLine($"❌ No existe celda en ({nextRow}, {nextCol})");
                }
                else
                {
                    Console.WriteLine($"❌ Bloqueado por {targetEntity.Type} en ({nextRow}, {nextCol})");
                }
            }
        }
    }
    
    private void UpdatePacmanPosition(int animationFrame, int newRow, int newCol)
    {
        animationFrame = (animationFrame + 1) % 2;
        var oldCell = _board.FirstOrDefault(c => c.Row == PacmanRow && c.Col == PacmanCol);
        var newCell = _board.FirstOrDefault(c => c.Row == newRow && c.Col == newCol);
        
        if (oldCell is null || newCell is null)
            return;
        
        oldCell.Type = EntityType.EMPTY;
        newCell.Type = EntityType.PACMAN;
        PacmanRow = newRow;
        PacmanCol = newCol;
        UpdatePacmanCanvasPosition();
        animationFrame = (animationFrame + 1) % 2;
        /*if (PacmanRow == 22 && PacmanCol == 21)
        {
            _isAutoMode = false;
            _movementTimer?.Stop();
            Console.WriteLine("Llegamos a la meta (22,21). Timer apagado. ¡Tu turno!");
        }*/
    }
    /*[RelayCommand]
    private void AddPoints(string cellType, int points)
    {
        if (cellType is "Cherry")
        {
            Score += 100;
        }
        else if (cellType is "pellet" || cellType is "energizer")
        { 
            Score = _score;
        }
        _engine.ScoreStateValidate(Score, _totalScore,HighScore);
    }*/
}