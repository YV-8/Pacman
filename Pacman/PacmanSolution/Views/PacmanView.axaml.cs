using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PacmanSolution.ViewModels;
using PacmanSolution.ViewModels.Pacman;

namespace PacmanSolution.Views;

public partial class PacmanView : UserControl
{
    private GameViewModel? _gamePageViewModel;
    private PacmanViewModel? _pacmanViewModel;
    public PacmanView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        Loaded += OnLoaded;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_pacmanViewModel is null) return;
        switch (e.Key)
        {
            case Key.Up or Key.W:
                _pacmanViewModel.GetDirection("UP");
                break;
            case Key.Down or Key.S:
                _pacmanViewModel.GetDirection("DOWN");
                break;
            case Key.Left or Key.A:
                _pacmanViewModel.GetDirection("LEFT");
                break;
            case Key.Right or Key.D:
                _pacmanViewModel.GetDirection("RIGHT");
                break;
        }
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.Focus();
        if (DataContext is GameViewModel gamevm)
        {
            _gamePageViewModel = gamevm;
            _pacmanViewModel = gamevm.Pacman;
            _pacmanViewModel.UpdatePacmanSprites(); // Aplicar sprite cuando la vista ya está en el árbol
        }
        Unloaded += (_, _) =>
        {
            if (DataContext is GameViewModel vm)
                vm.PauseGame();
        };
    }
}