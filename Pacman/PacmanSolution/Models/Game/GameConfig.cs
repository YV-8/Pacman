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
    
    /// <summary>Ancho de celda en píxeles.</summary>
    public const double CellWidth = CellSize;
    /// <summary>Alto de celda en píxeles.</summary>
    public const double CellHeight = CellSize;
    
    /// <summary>
    /// Desplazamiento horizontal para centrar el tablero lógico (28 columnas)
    /// dentro del canvas de 840 px de ancho.
    /// </summary>
    public const double OffsetX = 600;
    
    /// <summary>
    /// Desplazamiento vertical: bajamos un poco el grid para alinearlo
    /// mejor con la imagen de fondo.
    /// </summary>
    public const double OffsetY = 20;
} 