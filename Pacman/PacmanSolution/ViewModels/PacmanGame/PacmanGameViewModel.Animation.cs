using System;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private int _TotalScoreCherry=1500;
    private int _scoreCherry=0;
    private const double CellSize = 45.8;
    private const double OffsetX = 175;
    private const double OffsetY = 15;
    private const double PacmanImageSize = 40;
    private string _currentDirection = "right";
    private DispatcherTimer _movementTimer;
    private DispatcherTimer? _animationTimer;
    public event EventHandler<PacmanGameView.ElementRemovedEventArgs>? OnElementRemoved;

    
    /// <summary>
    /// Initializes Pacman's position from the board
    /// </summary>
    private void InitializePacmanPosition()
    {
        var pacmanCell = _board.FirstOrDefault(c => c.Type == CellType.PACMAN);
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
        _movementTimer.Tick += (s, e) => MovePacman(PacmanRow,PacmanCol);
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
                if (entity.Type == CellType.PACMAN)
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
        int oldRow = _currentSpriteRow;
        switch (direction.ToLower())
        {
            case "right":
                _currentSpriteRow = 0;
                break;
            case "left":
                _currentSpriteRow = 1;
                break;
            case "up":
                _currentSpriteRow = 2;
                break;
            case "down":
                _currentSpriteRow = 3;
                break;
        }
        if (oldRow != _currentSpriteRow)
        {
            UpdateSprites();
        }
    }


    /// <summary>
    /// Move Pacman in the current direction
    /// </summary>
    private void MovePacman(int nextRow, int nextCol)
    {
        var targetEntity = Board.FirstOrDefault(c => c.Row == PacmanRow && c.Col == PacmanCol);
        if (_engine.CanMoveTo(targetEntity))
        {
            var result = _engine.InteractionObjects(targetEntity);
            if (result.PointsEarned > 0) {
                Score += result.PointsEarned;
                if (Score > HighScore) HighScore = Score;
                
                OnElementRemoved?.Invoke(this, new PacmanGameView.ElementRemovedEventArgs(
                    result.RemovedElementType, nextRow, nextCol));
            }
        }

        switch (_currentDirection)
        {
            case "up":
                nextRow--;
                break;
            case "down":
                nextRow++;
                break;
            case "left":
                nextCol--;
                break;
            case "right":
                nextCol++;
                break;
        }
        UpdatePacmanPosition(_animationFrame, nextRow, nextCol);
    }
    
    private void UpdatePacmanPosition(int animationFrame, int newRow, int newCol)
    {
        animationFrame = (animationFrame + 1) % 2;
        var oldCell = _board.FirstOrDefault(c => c.Row == PacmanRow && c.Col == PacmanCol);
        var newCell = _board.FirstOrDefault(c => c.Row == newRow && c.Col == newCol);
        
        if (oldCell is null || newCell is null)
            return;
        
        oldCell.Type = CellType.EMPTY;
        _engine.InteractionObjects(newCell);
        
        newCell.Type = CellType.PACMAN;
        PacmanRow = newRow;
        PacmanCol = newCol;
        UpdatePacmanCanvasPosition();
        
        animationFrame = (animationFrame + 1) % 2;
    }
    [RelayCommand]
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
    }
}