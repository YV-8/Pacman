using System;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using PacmanSolution.Models;
using PacmanSolution.Views;

namespace PacmanSolution.ViewModels;

public partial class GameViewModel
{
    private int _currentSpriteRow = 0;
    private int _animationFrame = 0;
    private const double CellSize = 45.8;
    private const double OffsetX = 175;
    private const double OffsetY = 15;
    private const double PacmanImageSize = 40;
    private const string _nameSprite = null;
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
        _gameTimer.Tick += (s, e) => UpdatePacmanSprites();
        _gameTimer.Start();
    }
    
    private void StartMovementTimer()
    {
        _movementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _movementTimer.Tick += (s, e) => GetMotionPacman();
        _movementTimer.Start();
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
        _currentSpriteRow = _engine.ChangeDirection(direction, _currentSpriteRow);
        if (oldRow != _currentSpriteRow)
        {
            UpdatePacmanSprites();
        }

        GetMotionPacman();
    }

    private void GetMotionPacman()
    {
        var (nextRow, nextCol) = _engine.MovePacman(PacmanRow,PacmanCol);
        var targetEntity = Board.FirstOrDefault(c => c.Row == nextRow && c.Col == nextCol);
        if (targetEntity is not null && _engine.CanMoveTo(targetEntity))
        {
            Console.WriteLine($"Intentando mover a {nextRow}, {nextCol}");
            var result = _engine.InteractionObjects(targetEntity);
            int pointsResultEarned = result.PointsEarned;
            if (pointsResultEarned > 0)
            {
                _scoreViewModel.amount(pointsResultEarned);
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
                
                if (targetEntity is null)
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
    
    /// <summary>
    /// UpdateSprites is a method modification who sprite is to get
    /// whit _SpriteManager.GetSpritesSection with the Nae path and the rect is rectangle
    /// which change the board space had assigned as CellType.Pacman
    /// _animationFrame is the parr is between 0 - 1 
    /// </summary>
    private void UpdatePacmanSprites()
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
    }
}