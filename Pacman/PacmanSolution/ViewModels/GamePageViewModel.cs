using System.Collections.ObjectModel;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public class GamePageViewModel: ViewModelBase
{
    //[ObservableProperty]
    //private Board _gameBoard;
    private ManageBoard _manageBoard;
    private ObservableCollection<Cell> _board = new();

    public ObservableCollection<Cell> Board
    {
        get => _board;
        set => _board = value;
    }

    public GamePageViewModel()
    {
        Board.Clear();
        _manageBoard = new ManageBoard(12,12);
        // Inicializamos el tablero ( 10x10)
        
        _manageBoard.BuildGameBoard(Board);
    }
}