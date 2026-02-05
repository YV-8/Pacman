using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using PacmanSolution.Models;
using PacmanSolution.ViewModels;
using Avalonia.Media;

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
        OnPelletEaten();
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
        double cellSize = 40; 
    
        // Márgenes para centrar la rejilla sobre el dibujo del Board.png
        double offsetX = 60; 
        double offsetY = 15;
        //GameCanvas.Children.Clear();
        var elementsToRemove = GameCanvas.Children
            .Where(x => x != PacmanImage && !(x is Image))
            .ToList();
        foreach (var child in elementsToRemove)
        {
            GameCanvas.Children.Remove(child);
        }
        foreach (var cell in board)
        {
            if (cell.Type == CellType.WALL || cell.Type == CellType.INSIDEWALL)
            {
                var wall = new Rectangle { Width = 20, Height = 20, Fill = Brushes.Blue };
                wall.ZIndex = 2;
                Canvas.SetLeft(wall, offsetX + (cell.Column * cellSize));
                Canvas.SetTop(wall, offsetY + (cell.Row * cellSize));
                GameCanvas.Children.Add(wall);
            }
            if (cell.Type == CellType.Energize)
            {
                var powerPellet = new Ellipse { Width = 35, Height = 35, Fill = Brushes.White };
                ZIndex = 2;
                double xPos = offsetX + (cell.Column * cellSize) + (cellSize - 16) / 2;
                double yPos = offsetY + (cell.Row * cellSize) + (cellSize - 16) / 2;
                Canvas.SetLeft(powerPellet, xPos); 
                Canvas.SetTop(powerPellet, yPos);
                GameCanvas.Children.Add(powerPellet);
            }
            if (cell.HasPellet)
            {
                var dot = new Rectangle { Width = 12, Height = 12, Fill = Brushes.CadetBlue };
                ZIndex = 3;
                double xPos = offsetX + (cell.Column * cellSize) + (cellSize -40) / 5;
                double yPos = offsetY + (cell.Row * cellSize) + (cellSize - 18) / 2;
                Canvas.SetLeft(dot, xPos); 
                Canvas.SetTop(dot, yPos);
                GameCanvas.Children.Add(dot);
            }
        }
    }
    private void OnPelletEaten()
    {
        if (DataContext is GamePageViewModel vm)
        {
            vm.Score += 10;
        }
    }
    private void ManagePacman()
    {
        
    }
}