using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GhostViewModel :ObservableObject
{
    [ObservableProperty] private IImage? _redGhostSprite;
    [ObservableProperty] private IImage? _pinkGhostSprite;
    [ObservableProperty] private IImage? _cyanGhostSprite;
    [ObservableProperty] private IImage? _orangeGhostSprite;

    //Canvas (X, Y)
    [ObservableProperty] private double _redGhostLeft;
    [ObservableProperty] private double _redGhostTop;
    
    [ObservableProperty] private double _pinkGhostTop;
    [ObservableProperty] private double _pinkGhostLeft;
    
    [ObservableProperty] private double _cyanGhostTop;
    [ObservableProperty] private double _cyanGhostLeft;
    
    [ObservableProperty] private double _orangeGhostTop;
    [ObservableProperty] private double _orangeGhostLeft;
    
    private readonly SpriteManager _spriteManager;
    private readonly ObservableCollection<Entity> _board;

    public GhostViewModel(ObservableCollection<Entity> board)
    {
        _board = board;
    }
    
    private void UpdateGohstSprites(int _animationFrame, ObservableCollection<Entity> Board)
    {
        _animationFrame = (_animationFrame + 1) % 2;

        int _size = 16; 
        RedGhostSprite = _spriteManager.GetSpriteSection("GhostViews.png", 
            new PixelRect(0, 0 * _size, _size, _size)); // Col 0, Fila 0

        PinkGhostSprite = _spriteManager.GetSpriteSection("GhostViews.png", 
            new PixelRect(0, 1 * _size, _size, _size)); // Col 0, Fila 1

        CyanGhostSprite = _spriteManager.GetSpriteSection("GhostViews.png", 
            new PixelRect(0, 2 * _size, _size, _size)); // Col 0, Fila 2

        OrangeGhostSprite = _spriteManager.GetSpriteSection("GhostViews.png", 
            new PixelRect(0, 3 * _size, _size, _size)); // Col 0, Fila 3
        
        foreach (var entity in Board)
        {
            int ghostRow = entity.Type switch
            {
                EntityType.REDGHOST => 0,
                EntityType.PINKGHOST => 1,
                EntityType.CYANGHOST => 2,
                EntityType.ORANGEGHOST => 3,
                _ => -1
            };

            if (ghostRow != -1)
            {
                var rect = new PixelRect(
                    _animationFrame * _size, // Columna 0 o 1 para la animación de pies
                    ghostRow * _size,        // Fila según el color
                    _size, _size);
            
                entity.CurrentDisplaySprite = _spriteManager.GetSpriteSection("GhostViews.png", rect);
            }
        }
    }
}