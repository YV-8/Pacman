namespace PacmanSolution.Models;

public class BoardCell:Entity
{
    public BoardCell(int row, int col, CellType type) 
        : base(row, col, type, 46.6, 46.6, 1) 
    { }

    public override void Update(double deltaTime) 
    { 
        // Las celdas del tablero normalmente no cambian cada frame
    }
}