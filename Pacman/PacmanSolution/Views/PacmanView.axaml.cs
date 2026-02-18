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
        Loaded += (s, e) => this.Focus();
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is GameViewModel vm)
        {
            switch (e.Key)
            {
                case Key.Up or Key.W:
                    _pacmanViewModel.GetDirection("UP");
                    break;
                case Key.Down or  Key.S:  
                    _pacmanViewModel.GetDirection("DOWN"); 
                    break;
                case Key.Left or  Key.A: 
                    _pacmanViewModel.GetDirection("LEFT"); 
                    break;
                case Key.Right or Key.D: 
                    _pacmanViewModel.GetDirection("RIGHT"); 
                    break;
            }
        }
    }
    
    /// <summary>
    /// the OnLoaded is a method that in charge
    /// of call the DrawBoard
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        this.Focus();
    
        this.Unloaded += (s, e) =>
        {
            if (DataContext is GameViewModel viewModel)
            {
                viewModel.PauseGame();
            }
        };
        if (DataContext is GameViewModel gamevm)
        {
            _gamePageViewModel = gamevm;
        }
    }
}