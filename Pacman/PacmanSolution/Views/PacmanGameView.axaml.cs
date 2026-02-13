using System;
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

public abstract partial class PacmanGameView : UserControl
{
    private PacmanGameViewModel? _gamePageViewModel;
    private const double _cellSize = 45.8;
    private const double _offsetX = 175;
    private const double _offsetY = 15;
    private double _horizontalSpped = 10;
    public event EventHandler<ElementRemovedEventArgs>? OnElementRemoved;
    
    public PacmanGameView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        Loaded += OnLoaded;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is PacmanGameViewModel vm)
        {
            switch (e.Key)
            {
                case Key.Up or Key.W:
                    vm.ChangeDirection("up");
                    break;
                case Key.Down or  Key.S:  
                    vm.ChangeDirection("down"); 
                    break;
                case Key.Left or  Key.A: 
                    vm.ChangeDirection("left"); 
                    break;
                case Key.Right or Key.D: 
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
        this.Unloaded += (s, e) =>
        {
            if (DataContext is PacmanGameViewModel viewModel)
            {
                viewModel.PauseGame();
            }
        };
        if (DataContext is PacmanGameViewModel gamevm)
        {
            _gamePageViewModel = gamevm;
            DrawBoard(gamevm.Board);
            OnElementRemoved += OnElementRemovedFromBoard;
        
            // Configura el binding de la posición de Pacman
            SetupPacmanPositionBinding();
        }
    }
    /// <summary>
    /// Configura el binding manual para la posición de Pacman en el Canvas
    /// </summary>
    private void SetupPacmanPositionBinding()
    {
        if (_gamePageViewModel is null)
            return;

        // Actualiza la posición inicial
        UpdatePacmanPosition(_gamePageViewModel.PacmanCanvasLeft, _gamePageViewModel.PacmanCanvasTop);

        // Suscribe a los cambios de posición
        _gamePageViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(PacmanGameViewModel.PacmanCanvasLeft) or 
                nameof(PacmanGameViewModel.PacmanCanvasTop))
            {
                UpdatePacmanPosition(_gamePageViewModel.PacmanCanvasLeft, _gamePageViewModel.PacmanCanvasTop);
            }
        };
    }
    /// <summary>
    /// CanMove ask the targetcell  isn't null
    /// if it's not null; It isn't wall o door is true; but it's false
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    public void UpdatePacmanPosition(double left, double top)
    {
        Canvas.SetLeft(PacmanImage, left);
        Canvas.SetTop(PacmanImage, top);
    }

    /// <summary>
    ///  DrawBoard is who generate the wall board
    /// cellsize is the size each cell in the board
    /// </summary>
    /// <param name="board"></param>
    private void DrawBoard(ObservableCollection<Entity> board)
    {
        if (PacmanCanvas is null || board is null) return;
        
        var dynamicElements = PacmanCanvas.Children
            .Where(x => x != PacmanImage && !(x is Image { Opacity: 0.8 }))
            .ToList();

        foreach (var child in dynamicElements)
        {
            PacmanCanvas.Children.Remove(child);
        }
        foreach (var cell in board)
        {
            DrawEntity(cell);
        }
    }
    
    private void DrawEntity(Entity cell)
    {
        var (centerX, centerY) =PacmanGameViewModel.GetCellCenter(cell.Row, cell.Col);
        
        if (cell.Type == CellType.WALL)
        {
            double size = (cell.Type == CellType.WALL) ? 25 : 16;
            var color = Brushes.Transparent;
        
            AddShapeToCanvas(new Rectangle 
                { Width = size, Height = size, Fill = color }, 
                centerX, centerY, 2);
        }
        
        if (cell.HasDot && cell.Type is not CellType.ENERGIZE)
        {
            var pellet = new Ellipse 
            { 
                Width = 8, 
                Height = 8, 
                Fill = Brushes.White,
                Tag = $"pellet_{cell.Row}_{cell.Col}"
            };
            AddShapeToCanvas(pellet, centerX, centerY, 4);
        }
        
        if (cell.Type is CellType.ENERGIZE)
        {
            var energizer = new Ellipse 
            { 
                Width = 20, 
                Height = 20, 
                Fill = Brushes.White,
                Tag = $"energizer_{cell.Row}_{cell.Col}"
            };
            AddShapeToCanvas(energizer, centerX, centerY, 5);
        }

        if (cell.Type == CellType.DOOR)
        {
            double width = 25;
            double height = 10;
            var color = Brushes.White;
        
            AddShapeToCanvas(new Rectangle 
                { Width = width, Height = height, Fill = color }, 
                centerX, centerY, 2);
        }
    }
    
    private void AddShapeToCanvas(Control element, double positionX, double positionY, int zIndex)
    {
        element.ZIndex = zIndex;
        Canvas.SetLeft(element, positionX - (element.Width / 2));
        Canvas.SetTop(element, positionY - (element.Height / 2));
        PacmanCanvas.Children.Add(element);
    }
    /// <summary>
    /// Maneja el evento cuando un elemento debe ser removido del tablero
    /// </summary>
    private void OnElementRemovedFromBoard(object? sender, ElementRemovedEventArgs e)
    {
        RemoveElementFromCanvas(e.ElementType, e.Row, e.Col);
    }
    
    private void RemoveElementFromCanvas(String cellType, double row, double col)
    {
        string tag = $"{cellType}_{row}_{col}";
        var pelletToRemove = PacmanCanvas.Children
            .OfType<Ellipse>()
            .FirstOrDefault(e => e.Tag?.ToString() == tag);
        
        if (pelletToRemove != null)
        {
            PacmanCanvas.Children.Remove(pelletToRemove);
        }
    }
    /// <summary>
    /// Argumentos del evento para elementos removidos del Canvas
    /// </summary>
    public abstract class ElementRemovedEventArgs : EventArgs
    {
        public string ElementType { get; }
        public double Row { get; }
        public double Col { get; }

        /// <summary>
        /// ElementRemovedEventArgs 
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public ElementRemovedEventArgs(string elementType, double row, double col)
        {
            ElementType = elementType;
            Row = row;
            Col = col;
        }
    }
}