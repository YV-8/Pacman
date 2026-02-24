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

    public GameView()
    {
        InitializeComponent();
        //KeyDown += OnKeyDown;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // Cuando la página se cierra (Unloaded), pausamos los timers
        this.Unloaded += (s, ev) =>
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.PauseGame();
            }
        };
    }
}