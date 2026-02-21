namespace PacmanSolution.ViewModels;

/// <summary>
/// Global configuration of game:
/// All lineups of the viusla panel use them for the map
/// fit on screen 
/// </summary>
public static class GameConfig
{
    private const int GameWidth = 800;
    private const int GameHeight = 620;
    private const int BoardCols = 28;
    private const int BoardRows = 31;
    public const double EntitySize = 25;
    private const double CellSize = (double)GameHeight / BoardRows;
    public const double CellWidth = CellSize;
    public const double CellHeight = CellSize;
    public const double OffsetX = 600;
    public const double OffsetY = 20;
} 