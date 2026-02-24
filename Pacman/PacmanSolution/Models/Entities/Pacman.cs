using System.Collections.ObjectModel;
using Avalonia;

namespace PacmanSolution.Models.Entities;
public enum PacmanState
{
    NORMAL, DIEDING
}
public class Pacman: Entity
{
    private int SpawnRow { get; set; }
    private int SpawnCol { get; set; }
    private GameObject? _pacmanVisual;
    private string _currentDirection = "RIGHT";
    public PacmanState State { get; set; } = PacmanState.NORMAL;
    
    private ObservableCollection<GameObject> VisualObjects { get; } = new();

    public Pacman(int row, int col, EntityType entityType, double width, double height, int zindex) 
        : base(row, col, entityType, width, height, zindex)
    {
        SpawnRow = row;
        SpawnCol = col;
    }
    /// <summary>
    /// update the central position
    /// divide in the middle the width and height for get the center
    /// </summary>
    /// <param name="deltaTime"></param>
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
    
    /// <summary>
    /// Respawn of the pacman the initial position with spawnRow and spawnCol
    /// user the update position
    /// </summary>
    public void RespawnPacman()
    {
        this.Row = SpawnRow;
        this.Col = SpawnCol;
        this.State = PacmanState.NORMAL;
        this._currentDirection = "RIGHT";
        this.UpdateCanvasPosition();
    }
}