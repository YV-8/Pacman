using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PacmanSolution.Models;

public class ManageBoard
{
    /// <summary>
    /// Row ---- horizontal
    /// Col | vertical
    /// </summary>
    private int _boardCol;
    private int _boardRow;
    //private int Rows = 10;
    //private int Cols = 10;
    private List<Ghost> _ghosts;
    private Pacman _pacman;
    private CellType _cellType;

    public int BoardCol
    {
        get => _boardCol;
        set => _boardCol = value;
    }

    public int BoardRow
    {
        get => _boardRow;
        set => _boardRow = value;
    }

    public CellType CellType
    {
        get => _cellType;
        set => _cellType = value;
    }

    public ManageBoard(int boardCol, int boardRow)
    {
        _boardCol = boardCol;
        _boardRow = boardRow;
    }
    
    public void BuildGameBoard(ObservableCollection<Cell> Board)
    {
        // crear borders
        for (int row = 0; row < BoardRow; row++)
        {
            for (int col = 0; col < BoardCol; col++)
            {
                var cell = new Cell { Row = row, Column = col };
                BuildWallBoard(cell,row,col);
                Board.Add(cell);
            }
        }
    }
    private void BuildWallBoard(Cell cell, int Row, int Col)
    {
        var row = cell.Row;
        var col = cell.Column;
        if (row == 0 || row == BoardRow - 1 || col == 0 || col == BoardCol - 1)
        {
            cell.Type = CellType.WALL;
            cell.HasPellet = false;
        }
        else
        {
            cell.Type = CellType.EMPTY;
            cell.HasPellet = true;
        }
        BuildInsidewallBoard(cell, row, col);
    }

    private void BuildInsidewallBoard(Cell cell, int row, int col)
    {
        var rowInverse = BoardRow - 3;
        var colInverse = BoardCol - 3;
        if (row >= 2 && row <= 5)
        {
            if ((col >= 2 && col <= 5) || ( col >= 7 && col <= 11))
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if ((col >= 16 && col <= 22) || (col >= 24 && col <= colInverse))
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
        }

        if (row is >= 7 and <= 8)
        {
            if (col is >= 2 and <= 5 || (col >= 24 && col <= colInverse))
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col is >=10 and <=17)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
        }
        if (row >= 10 && row <= 14)
        {
            // Pilares laterales
            if (col >= 7 && col <= 8)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 19 && col <= 20)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            // Casa de fantasmas - entrada especial
            else if (row >= 10 && row <= 11 && col >= 10 && col <= 17)
            {
                if (col >= 13 && col <= 14 && row == 10)
                {
                    // Entrada (sin pared, sin pellet)
                    cell.Type = CellType.EMPTY;
                    cell.HasPellet = false;
                }
                else
                {
                    cell.Type = CellType.INSIDEWALL;
                    cell.HasPellet = false;
                }
            }
            // Casa de fantasmas interior
            else if (row >= 12 && row <= 14 && col >= 10 && col <= 17)
            {
                cell.Type = CellType.EMPTY;
                cell.HasPellet = false; // Dentro de la casa no hay pellets
            }
        }
        if (row >= 16 && row <= 17)
        {
            // Bloques horizontales largos
            if (col >= 2 && col <= 8)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 10 && col <= 11)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 13 && col <= 14)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 16 && col <= 17)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 19 && col <= 25)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
        }
        if (row >= 19 && row <= 20)
        {
            if (col >= 2 && col <= 5)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 7 && col <= 8)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 10 && col <= 17)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 19 && col <= 20)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 22 && col <= 25)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
        }
        if (row >= 22 && row <= 23)
        {
            if (col >= 7 && col <= 8)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 13 && col <= 14)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 19 && col <= 20)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
        }

        // ===== FILA 25-28: Bloques inferiores finales =====
        if (row >= 25 && row <= 28)
        {
            if (col >= 2 && col <= 5)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 7 && col <= 11)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 13 && col <= 14)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 16 && col <= 20)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
            else if (col >= 22 && col <= 25)
            {
                cell.Type = CellType.INSIDEWALL;
                cell.HasPellet = false;
            }
        }

        // ===== POWER PELLETS (esquinas) =====
        // Puedes agregar lógica especial para los power pellets grandes
        if ((row == 3 && col == 1) || (row == 3 && col == 26) ||
            (row == 23 && col == 1) || (row == 23 && col == 26))
        {
            cell.Type = CellType.Energize;
            cell.HasPellet = false; // No es pellet normal
        }
    }
}