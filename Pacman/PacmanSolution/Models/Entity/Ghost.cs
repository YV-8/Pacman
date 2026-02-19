using Avalonia.Media;

namespace PacmanSolution.Models;

public class Ghost:Entity
{
    public GhostDirection Direction { get; set; } = GhostDirection.Left;
    public GhostState State { get; set; } = GhostState.Normal;
    public int AnimationFrame { get; set; } = 0;

    public Ghost(int row, int col, EntityType entityType, double width, double height, int zindex) : base(row, col, entityType, width, height, zindex)
    {
    }

    public override void Update(double deltaTime)
    {
        UpdateCanvasPosition();
    }
}