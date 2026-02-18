namespace PacmanSolution.ViewModels;

/// <summary>
/// Configuración global del juego. Las dimensiones del área visual se usan para que
/// el mapa completo (28×31 celdas) quepa en pantalla sin cortar la parte de abajo.
/// </summary>
public static class GameConfig
{
    public const double CellSize = 45.8;
    public const double OffsetX = 175;
    public const double OffsetY = 15;
    public const double EntitySize = 40;

    /// <summary>Columnas del tablero (layout).</summary>
    public const int BoardCols = 28;
    /// <summary>Filas del tablero (layout).</summary>
    public const int BoardRows = 31;
    /// <summary>Ancho en píxeles del área de juego (canvas). Tamaño reducido para que quepa todo.</summary>
    public const int GameWidth = 840;
    /// <summary>Alto en píxeles del área de juego. Debe caber 31 filas sin cortar.</summary>
    public const int GameHeight = 620;
    /// <summary>Ancho de celda en píxeles (GameWidth / BoardCols).</summary>
    public const double CellWidth = (double)GameWidth / BoardCols;
    /// <summary>Alto de celda en píxeles (GameHeight / BoardRows).</summary>
    public const double CellHeight = (double)GameHeight / BoardRows;
}