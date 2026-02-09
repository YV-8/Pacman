using System;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GamePageViewModel
{
    private int _currentRow = 0 ;
    private void StartGameLoop()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(150) // Velocidad de la animación
        };
        _gameTimer.Tick += (s, e) => UpdateSprites();
        _gameTimer.Start();
    }
    
    /// <summary>
    /// UpdateSPrites is a method modification who sprite is to get
    /// whit _SpriteManager.GetSpritesSection with the Nae path and the rect is rectangle
    /// which change the board space had assigned as CellType.Pacman
    /// </summary>
    private void UpdateSprites()
    {
        _animationFrame = (_animationFrame + 1) % 2;

        int size = 16; 
        var rect = new PixelRect(
            _animationFrame * size,
            _currentRow * size,
            size, 
            size);
        
        var newSprite  = _spriteManager.GetSpriteSection("PacmanViews.png", rect);
        
        if (newSprite is not null)
        {
            PacmanCurrentSprite = newSprite;
            foreach (var entity in Board)
            {
                if (entity.Type == CellType.PACMAN)
                {
                    entity.CurrentDisplaySprite = newSprite;
                }
            }
        }
    }
    /// <summary>
    /// Method change the direccion if oldRow is diferentes the actual direcion
    /// </summary>
    /// <param name="direction"></param>
    public void ChangeDirection(string direction)
    {
        int oldRow = _currentRow;
        switch (direction.ToLower())
        {
            case "right":
                _currentRow = 0;
                break;
            case "left":
                _currentRow = 1;
                break;
            case "up":
                _currentRow = 2;
                break;
            case "down":
                _currentRow = 3;
                break;
        }
        if (oldRow != _currentRow)
        {
            UpdateSprites();
        }
    }
    [RelayCommand]
    private void ToggleMusic() { /* Lógica aquí */ }

    [RelayCommand]
    private void ViewScoresCommand()
    {
        
    }
}