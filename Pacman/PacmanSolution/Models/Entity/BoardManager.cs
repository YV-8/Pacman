using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PacmanSolution.Models;

public class BoardManager
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
    private Board _board;

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

    public BoardManager(int boardCol, int boardRow)
    {
        _boardCol = boardCol;
        _boardRow = boardRow;
    }
    
    

    public void BuildGameBoard(ObservableCollection<Entity> Board)
    {
        for (int row = 0; row < _boardRow; row++)
        {
            for (int col = 0; col < _boardCol; col++)
            {
                char cellChar = _board.Layout[row][col];
                var cell = CreateCellFromChar(row, col, cellChar);
                Board.Add(cell);
            }
        }
    }

    private Board CreateCellFromChar(int row, int col, char symbol)
    {
        var cell = new Board(row, col, CellType.EMPTY);
        cell.HasPellet = false;

        switch (symbol)
        {
            case 'W':
                cell.Type = CellType.WALL;
                break;
            case '-':
                cell.Type = CellType.DOOR;
                break;
            case '.':
                cell.Type = CellType.EMPTY;
                cell.HasPellet = true;
                break;
            case 'o':
                cell.Type = CellType.ENERGIZE;
                break;
            case 'E':
                break;
            case ' ':
                cell.Type = CellType.EMPTY;
                break;
        }
        return cell;
    }
}