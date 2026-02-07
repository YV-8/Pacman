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

    private BoardManager _boardManager;
    private ObservableCollection<Cell> _board = new();

    public ObservableCollection<Cell> Board
    {
        get => _board;
        set => _board = value;
    }

    public GamePageViewModel(ManagePageChange navigation)
    {
        _navigation = navigation;
        Board.Clear();
        _boardManager = new BoardManager(31,31);
        Score = 0;
        HighScore = 0;
        // Inicializamos el tablero ( 10x10)
        
        _boardManager.BuildGameBoard(Board);
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
    private void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }
}