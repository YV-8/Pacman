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

/*private void SetupGhostPositionBindings()
{
    if (_ghostViewModel is null) return;

    // Actualizar posición inicial
    Canvas.SetLeft(RedGhost, _ghostViewModel.RedGhostLeft);
    Canvas.SetTop(RedGhost, _ghostViewModel.RedGhostTop);

    // Escuchar cambios de posición
    _ghostViewModel.PropertyChanged += (s, e) =>
    {
        if (e.PropertyName == nameof(GhostViewModel.RedGhostLeft))
            Canvas.SetLeft(RedGhost, _ghostViewModel.RedGhostLeft);

        if (e.PropertyName == nameof(GhostViewModel.RedGhostTop))
            Canvas.SetTop(RedGhost, _ghostViewModel.RedGhostTop);
    };
}*/
    
    /// <summary>
    /// CanMove ask the targetEntity  isn't null
    /// if it's not null; It isn't wall o door is true; but it's false
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    /*private void UpdatePacmanPosition(Image PhotoImagen, double left, double top)
    {
        Canvas.SetLeft(PacmanImage, left);
        Canvas.SetTop(PacmanImage, top);*/