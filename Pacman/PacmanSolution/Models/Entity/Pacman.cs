namespace PacmanSolution.Models;

public class Pacman: Entity
{
    public Pacman(int row, int col, EntityType entityType, double width, double height, int zindex) : base(row, col, entityType, width, height, zindex)
    {
    }

    public override void Update(double deltaTime)
    {
        var (centerX, centerY) = GetCellCenter(Row, Col);
        CanvasLeft = centerX - (Width / 2);
        CanvasTop = centerY - (Height / 2);
    }
}