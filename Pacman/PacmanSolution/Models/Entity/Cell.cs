namespace PacmanSolution.Models;

public class Cell
{
    public int Row { get; set; }
    public int Column { get; set; }
    public CellType Type { get; set; }
    public bool HasPellet { get; set; }
}