using System.Collections.Generic;
using System.Collections.ObjectModel;
using PacmanSolution.Models.Entities;
using PacmanSolution.Models.Game;
using PacmanSolution.ViewModels;

namespace PacmanSolution.Models;

public class EngineManager
{
    private int _boardCol;
    private int _boardRow;
    private int _ghostCount = 0;
    private List<Ghost> _ghosts;
    private Pacman _pacman;
    private EntityType _entityType;
    private Board _boardLayout;
    private EntityType _ghostEntityType;

    /// <summary>
    /// Pass row and col the board for build the size
    /// </summary>
    /// <param name="boardCol"></param>
    /// <param name="boardRow"></param>
    public EngineManager(int boardCol, int boardRow)
    {
        _boardCol = boardCol;
        _boardRow = boardRow;
        _boardLayout = new Board(0, 0, EntityType.EMPTY);
    }
    
    /// <summary>
    /// Build the type entity in the board with layout
    /// and configure the structure
    /// </summary>
    /// <param name="board"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void BuildGameBoard(ObservableCollection<Entity> board)
    {
        if (board == null)
        {
            throw new System.ArgumentNullException(nameof(board));
        }
        _ghostCount = 0;
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
        var entityWidth = GameConfig.CellWidth;
        var entityHeight = GameConfig.CellHeight;
        var ghostCount = 0;
        switch (symbol)
        {
            case 'W':
                return new Board(row, col, EntityType.WALL){ Width = entityWidth, Height = entityHeight };;
            
            case '-':
                return new Board(row, col, EntityType.DOOR);
            
            case '.':
                var dot = new Pellet(row, col, EntityType.DOT, 4, 4, 5, isEnergizer: false);
                dot.UpdateCanvasPosition();
                return dot;
            
            case 'o':
                var energizer = new Pellet(row, col, EntityType.ENERGIZE, 8, 8, 5, isEnergizer: true);
                energizer.UpdateCanvasPosition();
                return energizer;
            case 'P':
                return new Pacman(row, col, EntityType.PACMAN, 25 , 25, 10){ Width = 15, Height = 15 };
            case 'G':
                var ghostType = GetTypeGhost(_ghostCount);
                _ghostCount++;
                return new Ghost(row, col, ghostType, 25, 25, 10);
            case 'E':
            case ' ':
                return new Board(row, col, EntityType.EMPTY);
            
            default:
                return new Board(row, col, EntityType.EMPTY);
        }
    }

    private EntityType GetTypeGhost(int ghostCount)
    {
        switch(ghostCount)
        {
            case 0:
                return EntityType.REDGHOST;
            case 1:
                return EntityType.PINKGHOST;  
            case 2:
                return EntityType.CYANGHOST;
            case 3:
                return EntityType.ORANGEGHOST;
            default:
                return EntityType.REDGHOST;
        };
    }
}