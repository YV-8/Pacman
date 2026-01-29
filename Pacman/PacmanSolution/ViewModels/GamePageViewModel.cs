using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel: ViewModelBase
{
    [ObservableProperty]
    private Board _gameBoard;

    public GamePageViewModel()
    {
        // Inicializamos el tablero (ej. 10x10)
        GameBoard = new Board(10, 10);
        GameBoard.CreateBoard();
    }
}