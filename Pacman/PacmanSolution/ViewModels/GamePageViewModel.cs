using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel: ObservableObject
{
    //[ObservableProperty]
    //private Board _gameBoard;
    [ObservableProperty]
    private int _score;
    [ObservableProperty]
    private int _highScore;
    [ObservableProperty]
    private ManagePageChange _navigation;

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
        _manageBoard = new ManageBoard(31,28);
        Score = 0;
        HighScore = 0;
        Navigation = new ManagePageChange();
        // Inicializamos el tablero ( 10x10)
        
        _manageBoard.BuildGameBoard(Board);
    }
    [RelayCommand]
    private void ToggleMusic() { /* Lógica aquí */ }

    [RelayCommand]
    private void OpenSettings() { /* Lógica aquí */ }

    [RelayCommand]
    private void ViewScoresCommand()
    {
        
    }
    [RelayCommand]
    public void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }
}