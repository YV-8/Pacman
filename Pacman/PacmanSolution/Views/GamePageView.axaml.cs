using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using PacmanSolution.Models;
using PacmanSolution.ViewModels;
using Avalonia.Media;
using Avalonia.Threading;

namespace PacmanSolution.Views;

public partial class GamePageView : UserControl
{
    private ObservableCollection<Cell> _cells;
    public GamePageView()
    {
        InitializeComponent();
        DataContext = new  GamePageViewModel();
        KeyDown += OnKeyDown;
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is GamePageViewModel gamePageViewModel)
        {
            DrawBoard(gamePageViewModel.Board);
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space)
        {
            throw new System.NotImplementedException();
        }
    }

    public void DrawBoard(ObservableCollection<Cell> board)
    {
        if (GameCanvas == null || board == null) return;
        GameCanvas.Children.Clear();

        foreach (var cell in board)
        {
            if (cell.Type == CellType.Wall)
            {
                var wall = new Rectangle { Width = 40, Height = 40, Fill = Brushes.Blue };
                Canvas.SetLeft(wall, cell.Column * 40);
                Canvas.SetTop(wall, cell.Row * 40);
                GameCanvas.Children.Add(wall);
            }
            
            if (cell.HasPellet)
            {
                var dot = new Ellipse { Width = 8, Height = 8, Fill = Brushes.Yellow };
                // Centramos el punto en el cuadro de 40x40
                Canvas.SetLeft(dot, (cell.Column * 40) + 16); 
                Canvas.SetTop(dot, (cell.Row * 40) + 16);
                GameCanvas.Children.Add(dot);
            }
        }
    }

    private void ManagePacman()
    {
        
    }
}