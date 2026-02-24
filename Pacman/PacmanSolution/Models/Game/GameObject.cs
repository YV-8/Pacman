using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace PacmanSolution.Models;

public class GameObject : INotifyPropertyChanged
{
    private double _x, _y;
    private double _width, _height;
    private int _zIndex;
    private IBrush? _fillColor;
    private IImage? _sprite;
    private Rect _sourceRect;

    public double X           { get => _x;          set { _x = value;          OnPropertyChanged(); } }
    public double Y           { get => _y;          set { _y = value;          OnPropertyChanged(); } }
    public double Width       { get => _width;       set { _width = value;       OnPropertyChanged(); } }
    public double Height      { get => _height;      set { _height = value;      OnPropertyChanged(); } }
    public int    Zindex      { get => _zIndex;      set { _zIndex = value;      OnPropertyChanged(); } }
    public IBrush? FillColor  { get => _fillColor;   set { _fillColor = value;   OnPropertyChanged(); } }
    /// <summary>Bitmap o CroppedBitmap </summary>
    public IImage? Sprite     { get => _sprite;      set { _sprite = value;      OnPropertyChanged(); } }
    public Rect   SourceRect  { get => _sourceRect;  set { _sourceRect = value;  OnPropertyChanged(); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}