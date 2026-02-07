using Avalonia;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel
{
    private int _currentRow = 0 ;
    private void UpdateSprites()
    {
        _animationFrame = (_animationFrame + 1) % 2; // Alterna entre 0 y 1

        int size = 16; 
        var rect = new PixelRect(
            0,                             // X siempre 0
            _animationFrame * size,        // Y salta de 0 a 16
            size, 
            size);
        
        var newSprite  = _spriteManager.GetSpriteSection("PacmanViews.png", rect);
        PacmanCurrentSprite = newSprite;
        foreach (var entity in Board)
        {
            if (entity.Type == CellType.PACMAN)
            {
                entity.CurrentDisplaySprite = newSprite;
            }
        }
    }
    public void ChangeDirection(string direction)
    {
        _currentRow = direction.ToLower() switch
        {
            "right" => 0,
            "left"  => 1,
            "up"    => 2,
            "down"  => 3,
            _       => _currentRow
        };
    }
}