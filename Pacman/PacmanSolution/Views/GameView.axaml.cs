using Avalonia.Controls;
using Avalonia.Interactivity;
using PacmanSolution.ViewModels;
using PacmanSolution.ViewModels.Pacman;

namespace PacmanSolution.Views;

public partial class GameView : UserControl
{
    private GameViewModel? _gamePageViewModel;
    private GhostViewModel? _ghostViewModel;
    private PacmanViewModel? _pacmanViewModel;
    private double _horizontalSpeed = 10;
    
    /// <summary>
    /// Initialize the game and components
    /// </summary>
    public GameView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    /// <summary>
    /// load the data context and pause the game 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.Unloaded += (s, ev) =>
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.PauseGame();
            }
        };
    }
}