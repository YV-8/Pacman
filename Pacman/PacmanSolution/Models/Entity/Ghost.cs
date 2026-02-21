using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using PacmanSolution.Model;
using PacmanSolution.Models.Ghosts;

namespace PacmanSolution.Models;

public class Ghost:Entity
{
    public GhostDirection Direction { get; set; } = GhostDirection.Left;
    public GhostState State { get; set; } = GhostState.INHOUSE;
    public GhostHunterMode HunterMode { get; set; } = GhostHunterMode.Scatter;
    public int ExitDelayTicks { get; set; } = 0;
    public int AnimationFrame { get; set; } = 0;
    private readonly BlinkyBehavior _blinky = new();
    private readonly PinkyBehavior  _pinky  = new();
    private readonly InkyBehavior   _inky   = new();
    private readonly ClydeBehavior  _clyde  = new();

    public Ghost(int row, int col, EntityType entityType, double width, double height, int zindex) 
        : base(row, col, entityType, width, height, zindex)
    {
    }
    
    public  int GetSpriteRow(EntityType type)
    {
        switch (type)
        {
            case EntityType.REDGHOST:    
                return 0;
            case EntityType.PINKGHOST:   
                return 1;
            case EntityType.CYANGHOST:   
                return 2;
            case EntityType.ORANGEGHOST: 
                return 3;
            default:                     
                return 0;
        }
    }
    public bool CanExitHouse(int currentTick)
    {
        return currentTick >= ExitDelayTicks;
    }
    public bool IsOutside()
    {
        return State != GhostState.INHOUSE;
    }

    public override void Update(double deltaTime)
    {
        UpdateCanvasPosition();
    }
    
    public void ApplyGhostsMove(Ghost ghost, GhostDirection dir,ObservableCollection<Entity> _board)
    {
        var (rowChange, colChange) = GhostBehaviorBase.DistanceDelta(dir);
        int newRow = ghost.Row + rowChange;
        int newCol = ghost.Col + colChange;

        var cell = _board.FirstOrDefault(e => e.Row == newRow && e.Col == newCol);
        if (cell is null || cell.Type is EntityType.WALL) return;
        if (cell.Type == EntityType.DOOR && ghost.State == GhostState.NORMAL) return;
        
        ghost.Direction = dir;
        ghost.Row = newRow;
        ghost.Col = newCol;
        ghost.UpdateCanvasPosition();
    }
    
    /// <summary>
    /// Assign the direction to each ghost
    /// </summary>
    /// <param name="ghost"></param>
    /// <param name="pacman"></param>
    /// <param name="blinky"></param>
    /// <param name="_board"></param>
    /// <returns></returns>
    public GhostDirection AssignDirection(Ghost ghost, Pacman pacman, Ghost blinky, ObservableCollection<Entity> _board)
    {
        Console.WriteLine($"[AssignDirection] {ghost.Type} HunterMode={ghost.HunterMode} State={ghost.State}");
        Console.WriteLine($"[Assign] {ghost.Type} Mode={ghost.HunterMode} " +
                          $"ghost=({ghost.Row},{ghost.Col}) pacman=({pacman.Row},{pacman.Col})");
        GhostDirection nextDirection;
        if (ghost.State == GhostState.FRIGHTENED)
        {
            nextDirection = GetRandomDirection(ghost, _board);
        }
        else if (ghost.HunterMode == GhostHunterMode.Scatter)
        {
            nextDirection = _blinky.GetScatterDirection(ghost, _board);
        }
        else
        {
            switch (ghost.Type)
            {
                case EntityType.REDGHOST:
                    nextDirection = _blinky.DecideNextDirection(ghost, pacman, _board);
                    break;
                case EntityType.PINKGHOST:
                    nextDirection = _pinky.DecideNextDirection(ghost, pacman, ghost.Direction, _board);
                    break;
                case EntityType.CYANGHOST:
                    nextDirection = _inky.DecideNextDirection(ghost, pacman, ghost.Direction, blinky, _board);
                    break;
                case EntityType.ORANGEGHOST:
                    nextDirection = _clyde.DecideNextDirection(ghost, pacman, _board);
                    break;
                default:
                    nextDirection = ghost.Direction;
                    break;
            }
        }
        return nextDirection;
    }
    
    private GhostDirection GetRandomDirection(Ghost ghost,ObservableCollection<Entity> _board)
    {
        var random = new Random();
        var valid = new List<GhostDirection>();

        foreach (GhostDirection dir in Enum.GetValues<GhostDirection>())
        {
            var (rowChange, colChange) = GhostBehaviorBase.DistanceDelta(dir);
            var cell = _board.FirstOrDefault(e =>
                e.Row == ghost.Row + rowChange &&
                e.Col == ghost.Col + colChange);

            if (cell is not null && cell.Type != EntityType.WALL)
            {
                valid.Add(dir);
            }
        }
        return valid.Count > 0 ? valid[random.Next(valid.Count)] : ghost.Direction;
    }
    
    public int GetMoveInterval(Ghost ghost)
    {
        switch (ghost.Type)
        {
            case EntityType.REDGHOST:    
                return 2;
            case EntityType.PINKGHOST:   
                return 3;
            case EntityType.CYANGHOST:   
                return 3;
            case EntityType.ORANGEGHOST: 
                return 4;
            default:                     
                return 3;
        }
    }
}