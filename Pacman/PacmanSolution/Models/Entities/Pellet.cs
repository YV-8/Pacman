
namespace PacmanSolution.Models.Entities;

public  class Pellet:Entity
{
    /// <summary>
    /// Indicates whether this pellet is an energizer (big dot)
    /// that activates FRIGHTENED mode on ghosts when eaten.
    /// </summary>
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