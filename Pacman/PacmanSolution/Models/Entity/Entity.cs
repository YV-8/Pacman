using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

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
    private bool _isActive = true;
    [ObservableProperty] 
    private EntityType _type;
    [ObservableProperty] 
    private bool _hasDot;
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
    }

    public abstract void Update(double deltaTime);

    public virtual void Move(int dr, int dc)
    {
        Row += dr;
        Col += dc;
    }
}