namespace PacmanSolution.Models;

public class Pellet:Entity
{
    public bool IsEnergizer { get; }
    public Pellet(int row, int col, EntityType entityType, double width, double height, int zIndex, bool isEnergizer) : base(row, col, entityType, width, height, zIndex)
    {
        row = row;
        width = width;
    }

    public override void Update(double deltaTime)
    {
        throw new System.NotImplementedException();
    }
}