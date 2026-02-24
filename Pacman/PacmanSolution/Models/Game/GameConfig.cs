namespace PacmanSolution.Models.Game;

/// <summary>
/// Global configuration of game:
/// All lineups of the visual panel use them for the map
/// fit on screen 
/// </summary>
public static class GameConfig
{
    private const int GameHeight = 620;
    private const int BoardRows = 31;
    private const double CellSize = (double)GameHeight / BoardRows;
    public const double CellWidth = CellSize;
    public const double CellHeight = CellSize;
    public const double OffsetX = 600;
    public const double OffsetY = 20;
} 