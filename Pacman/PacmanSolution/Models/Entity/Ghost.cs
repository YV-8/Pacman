using Avalonia.Media;
using PacmanSolution.Models.Ghosts;

namespace PacmanSolution.Models;

public class Ghost:Entity
{
    public GhostDirection Direction { get; set; } = GhostDirection.Left;
    public GhostState State { get; set; } = GhostState.INHOUSE;
    public GhostHunterMode HunterMode { get; set; } = GhostHunterMode.Scatter;
    public int ExitDelayTicks { get; set; } = 0;
    public int AnimationFrame { get; set; } = 0;

    public Ghost(int row, int col, EntityType entityType, double width, double height, int zindex) 
        : base(row, col, entityType, width, height, zindex)
    {
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
}