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
            cell.Type = CellType.Wall;
            cell.HasPellet = false;
        }
        else if ((row == 2 && col >= 2 && col <= 4) || (row == 8 && col >= 7 && col <= 9))
        {
            cell.Type = CellType.Wall;
            cell.HasPellet = false;
        }
        else
        {
            cell.Type = CellType.Empty;
            cell.HasPellet = true;
        }
    }
}