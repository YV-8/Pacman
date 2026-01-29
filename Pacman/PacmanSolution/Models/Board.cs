using System.Collections.Generic;

namespace PacmanSolution.Models;

public class Board
{
    private int _boardWidth;
    private int _boardHeight;
    private List<Ghost> _ghosts;
    private Pacman _pacman;
    private CellType [,] _board = new CellType[10, 10];
    private CellType _cellType;//r 0 r9 c0 c9

    public int BoardWidth
    {
        get => _boardWidth;
        set => _boardWidth = value;
    }

    public int BoardHeight
    {
        get => _boardHeight;
        set => _boardHeight = value;
    }

    public CellType CellType
    {
        get => _cellType;
        set => _cellType = value;
    }

    public Board(int boardWidth, int boardHeight)
    {
        _boardWidth = boardWidth;
        _boardHeight = boardHeight;
    }
    
    public List<CellType> CreateBoard()
    {
        var list = new List<CellType>();
        // crear borders
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                _board[i, j] = CellType.Empty;
                //col = j * 50;
                //row = i * 50;
            }
        }

        return list;
    }

    public void CreateWallBoard()
    {
        
    }
}