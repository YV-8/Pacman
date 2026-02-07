namespace PacmanSolution.Models;

public class Pacman: Entity
{
    public Pacman(int row, int col, CellType cellType, double width, double height, int zindex) : base(row, col, cellType, width, height, zindex)
    {
    }

    public override void Update(double deltaTime)
    {
        throw new System.NotImplementedException();
    }
}