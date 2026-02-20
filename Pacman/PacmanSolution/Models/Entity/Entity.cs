using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.ViewModels;

namespace PacmanSolution.Models;

public abstract partial class Entity:ObservableObject
{
    [ObservableProperty] 
    private IImage? _currentDisplaySprite;
    [ObservableProperty] 
    private int _row;
    [ObservableProperty] 
    private int _col;
    [ObservableProperty] 
    private double _width;
    [ObservableProperty] 
    private double _height;
    [ObservableProperty] 
    private int _zIndex;
    [ObservableProperty] 
    private EntityType _type;
    [ObservableProperty] 
    private bool _hasDot;
    [ObservableProperty] 
    private bool _isActive = true;
    [ObservableProperty] 
    private double _canvasLeft;
    [ObservableProperty] 
    private double _canvasTop;
    
    public Bitmap? Sprite { get; set; }
    public Rect? SourceRect { get; set; }
    public Entity(int row, int col, EntityType entityType, double width, double height, int zIndex)
    {
        Row = row;
        Col = col;
        Type = entityType;
        Width = width;
        Height = height;
        ZIndex = zIndex;
        UpdateCanvasPosition();
    }// <summary>
    /// Actualiza las coordenadas del canvas basándose en row/col
    /// </summary>
    public void UpdateCanvasPosition()
    {
        var (centerX, centerY) = GetCellCenter(Row, Col);
        CanvasLeft = centerX - (Width / 2);
        CanvasTop = centerY - (Height / 2);
    }

    /// <summary>
    /// Get the row and col and order in the canvas
    /// </summary>
    /// <param name="row"/>
    /// <param name="col"/>
    /// <returns></returns>
    public static (double x, double y) GetCellCenter(double row, double col)
    {
        var x = GameConfig.OffsetX + (col * GameConfig.CellWidth) + (GameConfig.CellWidth / 2);
        var y = GameConfig.OffsetY + (row * GameConfig.CellHeight) + (GameConfig.CellHeight / 2);
        return (x, y);
    }


    public abstract void Update(double deltaTime);

    public virtual void Move(int dr, int dc)
    {
        Row += dr;
        Col += dc;
    }
}