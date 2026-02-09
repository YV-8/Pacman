using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using PacmanSolution.Models;
using PacmanSolution.ViewModels;
using Avalonia.Media;
using Avalonia.Threading;

namespace PacmanSolution.Views;

public partial class PacmanGameView : UserControl
{
    private ObservableCollection<Entity> _entity;
    private DispatcherTimer _startTimer;
    private const double cellSize = 46.6;
    private const double offsetX = 160.5;
    private const double offsetY = 1.5;
    
    public PacmanGameView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        Loaded += OnLoaded;
        OnPelletEaten();
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is GamePageViewModel vm)
        {
            switch (e.Key)
            {
                case Key.Up:
                    MoveUp();
                    vm.ChangeDirection("up");
                    break;
                case Key.Down:  
                    vm.ChangeDirection("down"); 
                    break;
                case Key.Left: 
                    vm.ChangeDirection("left"); 
                    break;
                case Key.Right: 
                    vm.ChangeDirection("right"); 
                    break;
            }
        }
    }
    
    /// <summary>
    /// the OnLoaded is a method that incharge
    /// of call the DrawBoard
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is GamePageViewModel gamePageViewModel)
        {
            DrawBoard(gamePageViewModel.Board);
        }
    }
    
    private (double x, double y) GetCellCenter(int row, int col)
    {
        double x = offsetX + (col * cellSize) + (cellSize / 2);
        double y = offsetY + (row * cellSize) + (cellSize / 2);
        return (x, y);
    }

    /// <summary>
    ///  DrawBoard is who generate the inside board
    /// cellsize is the size each cell in the board
    /// </summary>
    /// <param name="board"></param>
    private void DrawBoard(ObservableCollection<Entity> board)
    {
        if (PacmanCanvas == null || board == null) return;
        
        var dynamicElements = PacmanCanvas.Children
            .Where(x => x != PacmanImage && !(x is Image { Opacity: 0.8 }))
            .ToList();
        
        foreach (var child in dynamicElements) PacmanCanvas.Children.Remove(child);
        foreach (var cell in board)
        {
            DrawEntity(cell);
        }
    }
    
    /// <summary>
    /// DrawEntity in charge of draw each entity
    /// first=> wall second => pellet or Energize; last door for ghost's door and pacman
    /// </summary>
    /// <param name="cell">
    /// each cell sie the similar but had diferent components
    /// </param>
    private void DrawEntity(Entity cell)
    {
        var (centerX, centerY) = GetCellCenter(cell.Row, cell.Col);
        
        if (cell.Type == CellType.WALL)
        {
            double size = (cell.Type == CellType.WALL) ? 25 : 16;
            var color = Brushes.Transparent;
        
            AddShapeToCanvas(new Rectangle 
                { Width = size, Height = size, Fill = color }, centerX, centerY, 2);
        }
        
        if (cell.HasPellet || cell.Type == CellType.ENERGIZE)
        {
            double size = 16;
            int zIndex = cell.HasPellet ? 4 : 3;
            if (cell.Type is CellType.ENERGIZE)
            {
                size = 25;
            }
            AddShapeToCanvas(new Ellipse 
                { Width = size, Height = size, Fill = Brushes.White }, centerX, centerY, zIndex);
        }
        
        if (cell.Type == CellType.PACMAN)
        {
            var pacmanImg = new Image { Width = 40, Height = 40 };
            pacmanImg.Bind(Image.SourceProperty, new Avalonia.Data.Binding("CurrentDisplaySprite")
                { Source = cell });
        
            AddShapeToCanvas(pacmanImg, centerX, centerY, 10);
        }

        if (cell.Type == CellType.DOOR)
        {
            double width = 25;
            double height = 10;
            var color = Brushes.White;
        
            AddShapeToCanvas(new Rectangle 
                { Width = width, Height = height, Fill = color }, centerX, centerY, 2);
        }
    }

    /// <summary>
    /// Addshape to the canvas GamePacman
    /// </summary>
    /// <param name="element"/> the type shape
    /// <param name="positionX"/> the position
    /// <param name="positionY"/>the position
    /// <param name="zIndex"/> the position between other elements
    private void AddShapeToCanvas(Control element, double positionX, double positionY, int zIndex)
    {
        element.ZIndex = zIndex;
        Canvas.SetLeft(element, positionX - (element.Width / 2));
        Canvas.SetTop(element, positionY - (element.Height / 2));
        PacmanCanvas.Children.Add(element);
    }

    private void MoveUp()
    {
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