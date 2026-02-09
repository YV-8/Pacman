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
    
    private readonly string[] _layout = {
        "WWWWWWWWWWWWWWWWWWWWWWWWWWWW",
        "W............WW............W",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "WoWWWW.WWWWW.WW.WWWWW.WWWWoW",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "W..........................W",
        "W.WWWW.WW.WWWWWWWW.WW.WWWW.W",
        "W.WWWW.WW.WWWWWWWW.WW.WWWW.W",
        "W......WW....WW....WW......W",
        "WWWWWW.WWWWW EE WWWWW.WWWWWW",
        "EEEEEW.WWWWW EE WWWWW.WEEEEE",
        "EEEEEW.WW          WW.WEEEEE",
        "EEEEEW.WW WWW--WWW WW.WEEEEE",
        "WWWWWW.WW WEEEEEEW WW.WWWWWW",
        "E      EE WEEEEEEW EE      E",
        "WWWWWW.WW WEEEEEEW WW.WWWWWW",
        "EEEEEW.WW WWWWWWWW WW.WEEEEE",
        "EEEEEW.WW          WW.WEEEEE",
        "EEEEEW.WW WWWWWWWW WW.WEEEEE",
        "WWWWWW.WW WWWWWWWW WW.WWWWWW",
        "W............WW............W",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "W.WWWW.WWWWW.WW.WWWWW.WWWW.W",
        "Wo..WW................WW..oW",
        "WWW.WW.WW.WWWWWWWW.WW.WW.WWW",
        "WWW.WW.WW.WWWWWWWW.WW.WW.WWW",
        "W......WW....WW....WW......W",
        "W.WWWWWWWWWW.WW.WWWWWWWWWW.W",
        "W.WWWWWWWWWW.WW.WWWWWWWWWW.W",
        "W..........................W",
        "WWWWWWWWWWWWWWWWWWWWWWWWWWWW"
    };

    public void BuildGameBoard(ObservableCollection<Entity> Board)
    {
        for (int row = 0; row < _boardRow; row++)
        {
            for (int col = 0; col < _boardCol; col++)
            {
                char cellChar = _layout[row][col];
                var cell = CreateCellFromChar(row, col, cellChar);
                Board.Add(cell);
            }
        }
    }

    private BoardCell CreateCellFromChar(int row, int col, char symbol)
    {
        var cell = new BoardCell(row, col, CellType.EMPTY);
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