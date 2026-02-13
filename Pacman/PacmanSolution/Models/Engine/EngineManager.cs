using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PacmanSolution.Models;

public class EngineManager
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
    private Board _boardLayout;
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

    public EngineManager(int boardCol, int boardRow)
    {
        _boardCol = boardCol;
        _boardRow = boardRow;
        _boardLayout = new Board(0, 0, CellType.EMPTY);
    }
    
    

    public void BuildGameBoard(ObservableCollection<Entity> board)
    {
        if (board == null)
        {
            throw new System.ArgumentNullException(nameof(board));
        }

        board.Clear();
        if (_boardLayout?.Layout == null || _boardLayout.Layout.Length == 0)
        {
            throw new System.InvalidOperationException("Board layout is not initialized");
        }
        
        int layoutRows = _boardLayout.Layout.Length;
        
        for (int row = 0; row < layoutRows; row++)
        {
            string currentRow = _boardLayout.Layout[row];
            
            for (int col = 0; col < currentRow.Length; col++)
            {
                char cellChar = currentRow[col];
                var cell = CreateCellFromChar(row, col, cellChar);
                board.Add(cell);
            }
        }
    }

    private Entity CreateCellFromChar(int row, int col, char symbol)
    {
        switch (symbol)
        {
            case 'W':
                return new Board(row, col, CellType.WALL);
            
            case '-':
                return new Board(row, col, CellType.DOOR);
            
            case '.':
                var cellWithPellet = new Board(row, col, CellType.EMPTY);
                cellWithPellet.HasDot = true;
                return cellWithPellet;
            
            case 'o':
                return new Board(row, col, CellType.ENERGIZE);
            
            case 'P':
                return new Pacman(row, col, CellType.PACMAN, 40, 40, 10);
            
            case 'E':
            case ' ':
                return new Board(row, col, CellType.EMPTY);
            
            default:
                return new Board(row, col, CellType.EMPTY);
        }
    }
}