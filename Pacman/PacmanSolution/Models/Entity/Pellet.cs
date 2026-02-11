namespace PacmanSolution.Models;

public class Pellet:Entity
{
    public bool IsEnergizer { get; }
    public Pellet(int row, int col, CellType cellType, double width, double height, int zIndex, bool isEnergizer) : base(row, col, cellType, width, height, zIndex)
    {
        row = row;
        width = width;
    }

    public override void Update(double deltaTime)
    {
        throw new System.NotImplementedException();
    }
}