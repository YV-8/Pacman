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
using PacmanSolution.ViewModels.Pacman;

namespace PacmanSolution.Views;

public partial class PacmanGameView : UserControl
{
    private GameViewModel? _gamePageViewModel;
    private GhostViewModel? _ghostViewModel;
    private PacmanViewModel? _pacmanViewModel;
    private double _horizontalSpeed = 10;
    public event EventHandler<ElementRemovedEventArgs>? OnElementRemoved;
    
    public PacmanGameView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
        Loaded += OnLoaded;
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
            gamevm.DrawBoard(Board);
            gamevm.OnElementRemoved += OnElementRemovedFromBoard;
        }
    }
    
    private void SetupGhostPositionBindings()
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
    }
    
    /// <summary>
    /// CanMove ask the targetEntity  isn't null
    /// if it's not null; It isn't wall o door is true; but it's false
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    /// <returns></returns>
    private void UpdatePacmanPosition(Image PhotoImagen, double left, double top)
    {
        Canvas.SetLeft(PacmanImage, left);
        Canvas.SetTop(PacmanImage, top);
    }
    
    
    private void AddShapeToCanvas(Control element, double positionX, double positionY, int zIndex)
    {
        element.ZIndex = zIndex;
        Canvas.SetLeft(element, positionX - (element.Width / 2));
        Canvas.SetTop(element, positionY - (element.Height / 2));
        PacmanCanvas.Children.Add(element);
    }
    /// <summary>
    /// Manage the event where an element would be to remove of board
    /// </summary>
    private void OnElementRemovedFromBoard(object? sender, ElementRemovedEventArgs e)
    {
        RemoveElementFromCanvas(e.ElementType, e.Row, e.Col);
    }
    
    private void RemoveElementFromCanvas(string cellType, double row, double col)
    {
        string tag = $"{cellType}_{row}_{col}";
        var pelletToRemove = PacmanCanvas.Children
            .OfType<Ellipse>()
            .FirstOrDefault(e => e.Tag?.ToString() == tag);
        
        if (pelletToRemove is not null)
        {
            PacmanCanvas.Children.Remove(pelletToRemove);
        }
    }
    /// <summary>
    /// Arguments of the event for elements remove of the Canvas
    /// </summary>
    public class ElementRemovedEventArgs : EventArgs
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