namespace PacmanSolution.Models;

public class Pacman: Entity
{
    public Pacman(int row, int col, EntityType entityType, double width, double height, int zindex) : base(row, col, entityType, width, height, zindex)
    {
    }

    public override void Update(double deltaTime)
    {
        throw new System.NotImplementedException();
    }
}