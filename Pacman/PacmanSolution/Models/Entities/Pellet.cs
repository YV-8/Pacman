using CommunityToolkit.Mvvm.ComponentModel;

namespace PacmanSolution.Models;

public partial class Pellet:Entity
{
    public bool IsEnergizer { get; }
    public Pellet(int row, int col, EntityType entityType, double width, double height, int zIndex, bool isEnergizer) : base(row, col, entityType, width, height, zIndex)
    {
        IsEnergizer = isEnergizer;
        IsActive = true;
    }

    public override void Update(double deltaTime)
    {
        UpdateCanvasPosition();
    }
}