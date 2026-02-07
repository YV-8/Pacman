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
    private ObservableCollection<Entity> _entity;
    
    
    public GamePageView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        this.Loaded += OnLoaded;
        OnPelletEaten();
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
    ///  DrawBoard is who generate the inside board
    /// cellsize is the size each cell in the board
    /// </summary>
    /// <param name="board"></param>
    public void DrawBoard(ObservableCollection<Entity> board)
    {
        if (GameCanvas == null || board == null) return;
        double cellSize = 35;


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
            if (cell.Type == CellType.WALL)
            {
                DrawOuterWalls(cell);
                /*var wall = new Rectangle { Width = outerWallSize, Height =outerWallSize, Fill = Brushes.Blue };
                wall.ZIndex = 2;
                Canvas.SetLeft(wall, cellCenterX + (cellSize - outerWallSize) / 2);
                Canvas.SetTop(wall, cellCenterY + (cellSize - outerWallSize) / 2);
                GameCanvas.Children.Add(wall);*/
            }
            else
            {
                DrawInnerElements(cell, cell.Type);
            }
            /*if (cell.Type == CellType.INSIDEWALL)
            {
                /*var wall = new Rectangle { Width = innerSize, Height = innerSize, Fill = Brushes.Blue };

                Canvas.SetLeft(wall, cellCenterX + (cellSize - innerSize) / 2);
                Canvas.SetTop(wall, cellCenterY + (cellSize - innerSize) / 2);
                GameCanvas.Children.Add(wall);*/
        }
    }

    private void DrawOuterWalls(Entity cell)
    {
        double cellSizeOuter = 46.6; 
        double outerWallSize = 25;
        
        // Márgenes para centrar la rejilla sobre el dibujo del Board
        double offsetX = 160.5;
        double offsetY = 1.5;
        var cellCenterX = offsetX + (cell.Col * cellSizeOuter) + (cellSizeOuter / 2);
        var cellCenterY = offsetY + (cell.Row * cellSizeOuter) + (cellSizeOuter / 2);
            
        var wall = new Rectangle 
        { 
            Width = outerWallSize, 
            Height = outerWallSize, 
            Fill = Brushes.Blue 
        };
        wall.ZIndex = 2;
        //wall.SetValue(Canvas.ZIndexProperty, 2);
        Canvas.SetLeft(wall, cellCenterX - (outerWallSize / 2));
        Canvas.SetTop(wall, cellCenterY - (outerWallSize / 2));
        GameCanvas.Children.Add(wall);
            
    }

    public void DrawInnerElements(Entity cell, CellType cellType)
    {
        double cellSizeInner = 46.6;    // Tamaño de celda para elementos internos (más pequeño)
        double innerWallSize = 16;    // Tamaño de paredes internas (más pequeñas)
        double dotSize = 6;           // Tamaño de los pellets
        double energizerSize = 15;     // Tamaño de los energizadores
    
        double offsetX = 160.5;
        double offsetY = 1.5;
        double cellCenterX = offsetX + (cell.Col * cellSizeInner) + (cellSizeInner / 2);
        double cellCenterY = offsetY + (cell.Row * cellSizeInner) + (cellSizeInner / 2);
        if (cell.Type == CellType.INSIDEWALL)
        {
            var wall = new Rectangle 
            { 
                Width = innerWallSize, 
                Height = innerWallSize, 
                Fill = Brushes.Transparent
            };
            wall.SetValue(Canvas.ZIndexProperty, 2);
            Canvas.SetLeft(wall, cellCenterX - (innerWallSize / 2));
            Canvas.SetTop(wall, cellCenterY - (innerWallSize / 2));
            GameCanvas.Children.Add(wall);
        }
        
        // Energizadores
        if (cell.Type == CellType.ENERGIZE)
        {
            var powerPellet = new Ellipse 
            { 
                Width = energizerSize, 
                Height = energizerSize, 
                Fill = Brushes.White 
            };
            powerPellet.SetValue(Canvas.ZIndexProperty, 3);
            Canvas.SetLeft(powerPellet, cellCenterX - (energizerSize / 2)); 
            Canvas.SetTop(powerPellet, cellCenterY - (energizerSize / 2));
            GameCanvas.Children.Add(powerPellet);
        }
        // Pellets (puntos pequeños)
        if (cell.HasPellet)
        {
            var dot = new Ellipse 
            { 
                Width = dotSize, 
                Height = dotSize, 
                Fill = Brushes.White 
            };
            dot.SetValue(Canvas.ZIndexProperty, 4);
            Canvas.SetLeft(dot, cellCenterX - (dotSize / 2)); 
            Canvas.SetTop(dot, cellCenterY - (dotSize / 2));
            GameCanvas.Children.Add(dot);
        }
        if (cell.Type == CellType.PACMAN)
        {
            var pacmanImg = new Image
            {
                Width = 40,
                Height = 40,
                ZIndex = 10
            };
            
            pacmanImg.Bind(Image.SourceProperty, new Avalonia.Data.Binding("CurrentDisplaySprite") { Source = cell });

            Canvas.SetLeft(pacmanImg, cellCenterX - 20);
            Canvas.SetTop(pacmanImg, cellCenterY - 20);
            GameCanvas.Children.Add(pacmanImg);
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