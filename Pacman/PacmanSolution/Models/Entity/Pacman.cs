using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media.Imaging;

namespace PacmanSolution.Models;

public class Pacman: Entity
{
    private GameObject? _pacmanVisual;
    private string _currentDirection = "RIGHT";
    public int DeadTicksRemaining { get; set; } = 0;
    private const int DeadDuration = 84; 
    public PacmanState State { get; set; } = PacmanState.NORMAL;
    private ObservableCollection<GameObject> VisualObjects { get; } = new();

    public Pacman(int row, int col, EntityType entityType, double width, double height, int zindex) 
        : base(row, col, entityType, width, height, zindex)
    {
    }
    // normal, imbencible, muerto, muriendo es el periodo de tiempo
    /// <summary>
    /// Initialize the objet pacman visual in canvas
    /// </summary>
    public GameObject CreatePacmanVisual(double x, double y, Bitmap sprite, Rect sourceRect)
    {
        var visual = new GameObject
        {
            X          = x,
            Y          = y,
            Width      = 40,
            Height     = 40,
            Zindex     = 10,
            Sprite     = sprite,
            SourceRect = sourceRect
        };
        _pacmanVisual = visual;
        VisualObjects.Add(visual);
        return visual;
    }
    
    /// <summary>
    /// Update the visual position pacman in the canvas
    /// </summary>
    public void UpdatePacmanVisual(double x, double y, Rect newSourceRect)
    {
        if (_pacmanVisual is null) return;
        _pacmanVisual.X          = x;
        _pacmanVisual.Y          = y;
        _pacmanVisual.SourceRect = newSourceRect;
    }

    public override void Update(double deltaTime)
    {
        var (centerX, centerY) = GetCellCenter(Row, Col);
        CanvasLeft = centerX - (Width / 2);
        CanvasTop = centerY - (Height / 2);
    }
    /// <summary>
    /// Method change the direction if oldRow is different the actual direction
    /// </summary>
    /// <param name="direction"></param>
    public int ChangeDirection(string direction, int _currentSpriteRow)
    {
        _currentDirection = direction.ToUpper();
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

        return _currentSpriteRow;
    }
    
    /// <summary>
    /// Move Pacman in the current direction
    /// </summary>
    public (int row, int col) MovePacman(int PacmanRow, int PacmanCol)
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
        return (nextRow, nextCol);
    }
    
    public void RespawnPacman(Pacman pacman )
    {
        pacman.State = PacmanState.DIEDING;
        pacman.DeadTicksRemaining = 48;
        pacman.DeadTicksRemaining = DeadDuration;
        pacman.UpdateCanvasPosition();
        Console.WriteLine($"[Comido] {pacman.Type}");
        
    }
}