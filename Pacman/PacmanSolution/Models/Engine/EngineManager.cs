using System.Collections.Generic;
using System.Collections.ObjectModel;
using PacmanSolution.ViewModels;

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
    private EntityType _entityType;
    private Board _boardLayout;
    private EntityType _ghostEntityType;
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

    public EntityType EntityType
    {
        get => _entityType;
        set => _entityType = value;
    }

    public EngineManager(int boardCol, int boardRow)
    {
        _boardCol = boardCol;
        _boardRow = boardRow;
        _boardLayout = new Board(0, 0, EntityType.EMPTY);
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
        double EntityWidth = GameConfig.CellWidth;
        double EntityHeight = GameConfig.CellHeight;
        int ghostCount = 0;
        switch (symbol)
        {
            case 'W':
                return new Board(row, col, EntityType.WALL){ Width = EntityWidth, Height = EntityHeight };;
            
            case '-':
                return new Board(row, col, EntityType.DOOR);
            
            case '.':
                var dot = new Board(row, col, EntityType.EMPTY)
                {
                    HasDot = true,
                    // Hacemos los puntos un poco más pequeños para que
                    // se adapten mejor al tamaño de la celda.
                    Width = 4,
                    Height = 4
                };
                dot.UpdateCanvasPosition();
                return dot;
            
            case 'o':
                var energizer = new Board(row, col, EntityType.ENERGIZE)
                {
                    // Energizadores también un poco más pequeños que antes.
                    Width = 10,
                    Height = 10
                };
                energizer.UpdateCanvasPosition();
                return energizer;
            case 'P':
                return new Pacman(row, col, EntityType.PACMAN, 25 , 25, 10);
            case 'G':
                _ghostEntityType = GetTypeGhost(ghostCount + 1);
                return new Ghost(row, col, _ghostEntityType, 25 , 25, 10);
            case 'E':
            case ' ':
                return new Board(row, col, EntityType.EMPTY);
            
            default:
                return new Board(row, col, EntityType.EMPTY);
        }
    }

    private EntityType GetTypeGhost(int ghostCount)
    {
        ghostCount++;
        switch(ghostCount)
        {
            case 1:
                return EntityType.REDGHOST;
            case 2:
                return EntityType.CYANGHOST;  
            case 3:
                return EntityType.PINKGHOST;
            case 4:
                return EntityType.ORANGEGHOST;
            default:
                return EntityType.REDGHOST;
        };
    }
}