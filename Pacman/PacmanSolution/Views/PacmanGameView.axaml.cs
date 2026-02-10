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
    private ObservableCollection<Entity> _board;
    private DispatcherTimer _gameTimer;
    private const double cellSize = 46.6;
    private const double offsetX = 160.5;
    private const double offsetY = 1.5;
    private double _horizontalSpped = 10;
    private double _pacmanRow = 1;
    private double _pacmanCol = 1;
    private string _currentDirection = "right";
    private int _score;
    
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
        this.Unloaded += (s, e) => _gameTimer?.Stop();
        if (DataContext is GamePageViewModel gamePageViewModel)
        {
            DrawBoard(gamePageViewModel.Board);
            _board = gamePageViewModel.Board;
            var pacmanCell = _board.FirstOrDefault(c => c.Type == CellType.PACMAN);
            if (pacmanCell != null)
            {
                _pacmanRow = pacmanCell.Row;
                _pacmanCol = pacmanCell.Col;
            }
            StartGameTimer();
        }
    }
    
    private (double x, double y) GetCellCenter(int row, int col)
    {
        double x = offsetX + (col * cellSize) + (cellSize / 2);
        double y = offsetY + (row * cellSize) + (cellSize / 2);
        return (x, y);
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
        
        foreach (var child in dynamicElements) PacmanCanvas.Children.Remove(child);
        foreach (var cell in board)
        {
            DrawEntity(cell);//vm.build()
            // methodxhere(object sender, 
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
    private void DefaultTimerOn(Object sender, EventArgs e)
    {
        double nextRow = _pacmanRow;
        double nextCol = _pacmanCol;
        _horizontalSpped += 10;
        _pacmanRow = Canvas.GetLeft(PacmanImage);
        _pacmanCol = Canvas.GetTop(PacmanImage);
        var pacmanPosition = Canvas.GetLeft(PacmanImage) + _horizontalSpped;
        ValidateWall(pacmanPosition,_currentDirection,_pacmanRow,_pacmanCol,nextRow, nextCol);
        switch (_currentDirection)
        {
            case "up":    nextRow--; break;
            case "down":  nextRow++; break;
            case "left":  nextCol--; break;
            case "right": nextCol++; break;
        }
    }

    private void ValidateWall(double pacmanPosition, string currentDirection, double pacRow,
        double pacCol, double nextRow, double nextCol)
    {
        var targetCellType = _board.FirstOrDefault(c => c.Row == pacRow && c.Col == pacCol);
        if (!IsWall(targetCellType,pacRow,pacCol))
        {
            
        }
        if (!IsWall(targetCellType,nextRow,nextCol))
        {
            pacmanPosition = 0;
        }
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
    private bool IsWall(Entity targetCellType, double row, double col)
    {
        bool isWall = false;
        
        if (targetCellType is null)
        {
            isWall = true;
        }

        if (targetCellType.Type == CellType.WALL || targetCellType.Type == CellType.DOOR)
        {
            isWall =false;
        }
        return isWall;
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
            if(_board is not null)
            {
                UpdatePacmanPosition(nextRow, nextCol);
            }
            
        }
    }
    
    /// <summary>
    /// CanMove ask the targetcell  isn't null
    /// if it's not null; It isn't wall o door is true; but it's false
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool CanMoveTo(double row, double col)
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
    
    private void UpdatePacmanPosition(double newRow, double newCol)
    {
        var oldCell = _board.FirstOrDefault(c => c.Row == _pacmanRow && c.Col == _pacmanCol);
        var newCell = _board.FirstOrDefault(c => c.Row == newRow && c.Col == newCol);
        
        if (oldCell == null || newCell == null)
            return;
        
        oldCell.Type = CellType.EMPTY;
        
        if (newCell.HasPellet)
        {
            newCell.HasPellet = false;
            _score += 10;
            UpdateScore();
            // Aquí deberías buscar el elipse en el Canvas y quitarlo, 
            // o usar una técnica de "DataTemplates" más adelante
        }
        
        if (newCell.Type == CellType.ENERGIZE)
        {
            newCell.Type = CellType.EMPTY;
            _score += 50;
            UpdateScore();
            // Aquí activar el modo "frightened" de los fantasmas
        }
        
        newCell.Type = CellType.PACMAN;
        _pacmanRow = newRow;
        _pacmanCol = newCol;
        var (targetX, targetY) = GetCellCenter((int)newRow, (int)newCol);
        Canvas.SetLeft(PacmanImage, targetX - (PacmanImage.Width / 2));
        Canvas.SetTop(PacmanImage, targetY - (PacmanImage.Height / 2));
    }
    
    private void UpdateScore()
    {
        if (DataContext is GamePageViewModel vm)
        {
            vm.Score = _score;
        }
    }
}