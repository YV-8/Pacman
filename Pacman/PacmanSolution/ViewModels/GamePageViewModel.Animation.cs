using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel
{ 
    [ObservableProperty]
    private int _score;
    [ObservableProperty]
    private int _highScore;
    [ObservableProperty]
    private int _pacmanRow;
    [ObservableProperty]
    private int _pacmanCol;
    [ObservableProperty]
    private double _pacmanCanvasLeft;
    [ObservableProperty]
    private double _pacmanCanvasTop;
    private int _currentSpriteRow = 0;
    private int _animationFrame = 0;
    private const double CellSize = 45.8;
    private const double OffsetX = 175;
    private const double OffsetY = 15;
    private const double PacmanImageSize = 40;
    private string _currentDirection = "right";
    private DispatcherTimer _movementTimer;
    private DispatcherTimer? _animationTimer;
    public event EventHandler<ElementRemovedEventArgs>? OnElementRemoved;
    
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
    
    /// <summary>
    /// Start the loop the game for animations
    /// </summary>
    private void StartGameLoop()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(150)
        };
        _gameTimer.Tick += (s, e) => UpdateSprites();
        _gameTimer.Start();
    }
    
    /// <summary>
    /// Start the movement to the pacman
    /// </summary>
    private void StartMovementTimer()
    {
        _movementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200) // Velocidad del movimiento
        };
        _movementTimer.Tick += (s, e) => MovePacman();
        _movementTimer.Start();
    }
    
    /// <summary>
    /// UpdateSPrites is a method modification who sprite is to get
    /// whit _SpriteManager.GetSpritesSection with the Nae path and the rect is rectangle
    /// which change the board space had assigned as CellType.Pacman
    /// _animationFrame is the parr is between 0 - 1 
    /// </summary>
    private void UpdateSprites()
    {
        _animationFrame = (_animationFrame + 1) % 2;

        int size = 16; 
        var rect = new PixelRect(
            _animationFrame * size,
            _currentSpriteRow * size,
            size, 
            size);
        
        var newSprite  = _spriteManager.GetSpriteSection("PacmanViews.png", rect);
        
        if (newSprite is not null)
        {
            PacmanCurrentSprite = newSprite;
            /*
             * foreach (var entity in Board.Where(e => e.Type is CellType.PACMAN))
               {
                   entity.CurrentDisplaySprite = newSprite;
               }
             */
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
    private void MovePacman()
    {
        int nextRow = PacmanRow;
        int nextCol = PacmanCol;

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
        if (CanMoveTo(nextRow, nextCol))
        {
            UpdatePacmanPosition(nextRow, nextCol);
        }
    }
    /// <summary>
    /// Verification the Pacman can move the specific position
    /// </summary>
    private bool CanMoveTo(int row, int col)
    {
        var targetCell = Board.FirstOrDefault(c => c.Row == row && c.Col == col);

        if (targetCell is null)
            return false;

        return targetCell.Type is not CellType.WALL && targetCell.Type is not CellType.DOOR;
    }
    public void UpdatePacmanPosition(int newRow, int newCol)
    {
        _animationFrame = (_animationFrame + 1) % 2;
        var oldCell = _board.FirstOrDefault(c => c.Row == PacmanRow && c.Col == PacmanCol);
        var newCell = _board.FirstOrDefault(c => c.Row == newRow && c.Col == newCol);
        
        if (oldCell is null || newCell is null)
            return;
        
        oldCell.Type = CellType.EMPTY;
        EatDotsInteraction(newCell, newRow,newCol);
        
        newCell.Type = CellType.PACMAN;
        PacmanRow = newRow;
        PacmanCol = newCol;
        UpdatePacmanCanvasPosition();
        // Incrementa el frame de animación
        _animationFrame = (_animationFrame + 1) % 2;
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
    private static (double x, double y) GetCellCenter(double row, double col)
    {
        var x = OffsetX + (col * CellSize) + (CellSize / 2);
        var y = OffsetY + (row * CellSize) + (CellSize / 2);
        return (x, y);
    }
    
    /// <summary>
    /// The method is 
    /// </summary>
    /// <param name="newCell"></param>
    /// <param name="newRow"></param>
    /// <param name="newCol"></param>
    private void EatDotsInteraction(Entity newCell,double newRow,double newCol)
    {
        var newCellType ="pellet";
        if (newCell.HasPellet)
        {
            newCell.HasPellet = false;
            _score += 10;
            //UpdateScore();
            //RemoveElementFromCanvas(newCellType,newRow, newCol);
        }
        if (newCell.Type is CellType.ENERGIZE)
        {
            newCell.Type = CellType.EMPTY;
            newCellType ="energizer";
            _score += 50;
            //UpdateScore();
            //RemoveElementFromCanvas(newCellType,newRow, newCol);
        }
        OnElementRemoved?.Invoke(this, new ElementRemovedEventArgs("pellet", newRow, newCol));
    }
    
    
    /*[RelayCommand]*/
    /*public void ToggleAudioCommand(bool isChecked)
    {
        string path = "PacmanTheme";
        if (isChecked)
        {
            _soundManager.PlaySound(path, true);
        }
        else
        {
            _soundManager.StopSound();
        }
    }*/
    

    [RelayCommand]
    private void ViewScoresCommand()
    {
        // Implementar lógica para mostrar puntuaciones
    }
    /// <summary>
    /// Argumentos del evento para elementos removidos del Canvas
    /// </summary>
    public class ElementRemovedEventArgs : EventArgs
    {
        public string ElementType { get; }
        public double Row { get; }
        public double Col { get; }

        /// <summary>
        /// ElementRemovedEventArgs 
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public ElementRemovedEventArgs(string elementType, double row, double col)
        {
            ElementType = elementType;
            Row = row;
            Col = col;
        }
    }
}