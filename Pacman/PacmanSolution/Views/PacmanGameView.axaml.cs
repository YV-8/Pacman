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
using Avalonia.Threading;

namespace PacmanSolution.Views;

public partial class PacmanGameView : UserControl
{
    //private GamePageViewModel? _gamePageViewModel;
    private const double _cellSize = 45.8;
    private const double _offsetX = 175;
    private const double _offsetY = 15;
    private double _horizontalSpped = 10;
    
    public PacmanGameView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        Loaded += OnLoaded;
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is GamePageViewModel vm)
        {
            switch (e.Key)
            {
                case Key.Up:
                    _currentDirection = "up";
                    vm.ChangeDirection("up");
                    break;
                case Key.Down:  
                    _currentDirection = "down";
                    vm.ChangeDirection("down"); 
                    break;
                case Key.Left: 
                    _currentDirection = "left";
                    vm.ChangeDirection("left"); 
                    break;
                case Key.Right: 
                    _currentDirection = "right";
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
            if (DataContext is GamePageViewModel viewModel)
            {
                viewModel.PauseGame();
            }
        };
        if (DataContext is GamePageViewModel gamePageViewModel)
        {
            DrawBoard(gamePageViewModel.Board);
            _board = gamePageViewModel.Board;
            var pacmanCell = _board.FirstOrDefault(c => c.Type == CellType.PACMAN);
            if (pacmanCell is not null)
            {
                _pacmanRow = pacmanCell.Row;
                _pacmanCol = pacmanCell.Col;
                
                var (centerX, centerY) = GetCellCenter(_pacmanRow, _pacmanCol);
                Canvas.SetLeft(PacmanImage, centerX - (PacmanImage.Width / 2));
                Canvas.SetTop(PacmanImage, centerY - (PacmanImage.Height / 2));
            }
            StartGameTimer();
        }
    }
    public (double x, double y) GetCellCenter(double row, double col)
    {
        double x = _offsetX + (col * _cellSize) + (_cellSize / 2);
        double y = _offsetY + (row * _cellSize) + (_cellSize / 2);
        return (x, y);
    }
    /// <summary>
    /// CanMove ask the targetcell  isn't null
    /// if it's not null; It isn't wall o door is true; but it's false
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public bool CanMoveTo(double row, double col)
    {
        var targetCell = _board.FirstOrDefault(c => c.Row == row && c.Col == col);
        bool isNotWall = false;
        if (targetCell is null)
        {
            isNotWall = false;
        }
        else if(targetCell.Type is not CellType.WALL && targetCell.Type is not CellType.DOOR)
        {
            isNotWall = true;
        }

        return isNotWall;
    }
    
    public void UpdatePacmanPosition(double newRow, double newCol)
    {
        var oldCell = _board.FirstOrDefault(c => c.Row == _pacmanRow && c.Col == _pacmanCol);
        var newCell = _board.FirstOrDefault(c => c.Row == newRow && c.Col == newCol);
        
        if (oldCell == null || newCell == null)
            return;
        
        oldCell.Type = CellType.EMPTY;
        OnPelletEaten(newCell, newRow,newCol);
        
        newCell.Type = CellType.PACMAN;
        _pacmanRow = newRow;
        _pacmanCol = newCol;
        var (targetX, targetY) = GetCellCenter((int)newRow, (int)newCol);
        Canvas.SetLeft(PacmanImage, targetX - (PacmanImage.Width / 2));
        Canvas.SetTop(PacmanImage, targetY - (PacmanImage.Height / 2));
    }
    public void OnPelletEaten(Entity newCell,double newRow,double newCol)
    {
        var newCellType ="pellet";
        if (newCell.HasPellet)
        {
            newCell.HasPellet = false;
            _score += 10;
            UpdateScore();
            RemoveElementFromCanvas(newCellType,newRow, newCol);
        }
        if (newCell.Type == CellType.ENERGIZE)
        {
            newCell.Type = CellType.EMPTY;
            newCellType ="energizer";
            _score += 50;
            UpdateScore();
            RemoveElementFromCanvas(newCellType,newRow, newCol);
        }
    }

    /// <summary>
    ///  DrawBoard is who generate the wall board
    /// cellsize is the size each cell in the board
    /// </summary>
    /// <param name="board"></param>
    private void DrawBoard(ObservableCollection<Entity> board)
    {
        if (PacmanCanvas == null || board == null) return;
        
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
                { Width = size, Height = size, Fill = color }, 
                centerX, centerY, 2);
        }
        
        if (cell.HasPellet && cell.Type is not CellType.ENERGIZE)
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
                Tag = $"energizer_{cell.Row}_{cell.Col}" // Etiqueta única
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
    private void StartGameTimer()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _gameTimer.Tick += GameTimerTick;
        _gameTimer.Start();
    }
    private void GameTimerTick(object? sender, EventArgs e)
    {
        ManagePacman();
    }
    private void ManagePacman()
    {
        //if (DataContext is GamePageViewModel vm)
        double nextRow = _pacmanRow;
        double nextCol = _pacmanCol;
        
        switch (_currentDirection)
        {
            case "up":
                nextRow--;
                break;
            case "down":
                nextRow++;
                break;
            case "left":
                nextCol--;
                break;
            case "right":
                nextCol++;
                break;
        }
        
        if (CanMoveTo(nextRow, nextCol))
        {
            UpdatePacmanPosition(nextRow, nextCol);
        }
    }
    
    /// <summary>
    /// Elimina una píldora específica del Canvas usando su Tag
    /// </summary>
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
    
    private void UpdateScore()
    {
        if (DataContext is GamePageViewModel vm)
        {
            vm.Score = _score;
        }
    }
}