namespace PacmanSolution.ViewModels;

/// <summary>
/// Global configuration of game:
/// All lineups of the viusla panel use them for the map
/// fit on screen 
/// </summary>
public static class GameConfig
{
    // Tamaño total del área jugable donde se posicionan las entidades
    private const int GameWidth = 800;
    private const int GameHeight = 620;
    
    // Cantidad lógica de columnas/filas del layout del tablero (28x31).
    private const int BoardCols = 28;
    private const int BoardRows = 31;
    
    // Tamaño base de entidades móviles (Pacman, fantasmas).
    public const double EntitySize = 25;
    
    // Usamos un tamaño de celda uniforme (cuadrado). Tomamos como referencia el alto,
    // así el grid ocupa todo el alto disponible y queda algo más “cuadrado”.
    private const double CellSize = (double)GameHeight / BoardRows; // 620 / 31 ≈ 20
    
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