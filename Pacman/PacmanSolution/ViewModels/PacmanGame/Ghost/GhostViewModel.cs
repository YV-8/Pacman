using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Model;
using PacmanSolution.Models;
using PacmanSolution.Models.Ghosts;

namespace PacmanSolution.ViewModels;

public partial class GhostViewModel :ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Ghost> _ghosts = new ();
    private readonly BlinkyBehavior _blinky = new();
    private readonly PinkyBehavior  _pinky  = new();
    private readonly InkyBehavior   _inky   = new();
    private readonly ClydeBehavior  _clyde  = new();
    private readonly HouseBehavior  _house  = new();
    private readonly ObservableCollection<Entity> _board;
    private readonly SpriteManager _spriteManager = new();
    private readonly GameBoardSyncService? _syncService;
    private int _globalAnimationFrame = 0;
    private const int _size = 16;

    public GhostViewModel(ObservableCollection<Entity> board,GameBoardSyncService? syncService)
    {
        _syncService = syncService;
        _board = board;
        InitializeGhosts();
        MoveGhosts();
        UpdateAllSprites();
    }
    private void InitializeGhosts()
    {
        Ghosts.Clear();
        foreach (var entity in _board.OfType<Ghost>())
        {
            switch (entity.Type)
            {
                case EntityType.REDGHOST:
                    entity.ExitDelayTicks = 0;
                    break;
                case EntityType.PINKGHOST:
                    entity.ExitDelayTicks = 10;
                    break;
                case EntityType.CYANGHOST:
                    entity.ExitDelayTicks = 25;
                    break;
                case EntityType.ORANGEGHOST:
                    entity.ExitDelayTicks = 45;
                    break;
            }
            Ghosts.Add(entity);
        }
    }
    
    public void GhostsTimer()
    {
        _globalAnimationFrame = (_globalAnimationFrame + 1) % 2;
        UpdateAllSprites();
    }
    
    private static int GetSpriteRow(EntityType type)
    {
        switch (type)
        {
            case EntityType.REDGHOST:    
                return 0;
            case EntityType.PINKGHOST:   
                return 1;
            case EntityType.CYANGHOST:   
                return 2;
            case EntityType.ORANGEGHOST: 
                return 3;
            default:                     
                return 0;
        }
        
    }
    private static int  GetDirectionBaseCol(GhostDirection ChangeDirection)
    {
        switch (ChangeDirection)
        {
            case GhostDirection.Right: 
                return 0;
            case GhostDirection.Left:  
                return 2;
            case GhostDirection.Up:    
                return 4;
            case GhostDirection.Down:  
                return 6;
            default:
                return 0;
        }
    }
    public void UpdateAllSprites()
    {
        foreach (var ghost in Ghosts)
        {
            ghost.AnimationFrame = _globalAnimationFrame;
            ghost.CurrentDisplaySprite = GetGhostSprite(ghost);
        }
    }

    private void MoveGhosts()
    {
        var pacman = _board.FirstOrDefault(e => e.Type == EntityType.PACMAN) as Models.Pacman;
        var blinky = Ghosts.FirstOrDefault(g => g.Type == EntityType.REDGHOST);
        if (pacman is null || blinky is null) return; 
    }

    private IImage? GetGhostSprite(Ghost ghost)
    {
        if (ghost.State == GhostState.FRIGHTENED)
        {
            var frightenedRect = new PixelRect(_globalAnimationFrame * _size, 4 * _size, _size, _size);
            return _spriteManager.GetSpriteSection("GhostViews.png", frightenedRect);
        }

        int row = GetSpriteRow(ghost.Type);
        int col = GetDirectionBaseCol(ghost.Direction) + _globalAnimationFrame;
        return _spriteManager.GetSpriteSection("GhostViews.png", 
            new PixelRect(col * _size, row * _size, _size, _size));
    }
    public void SetFrightened()
    {
        foreach (var ghost in Ghosts)
            ghost.State = GhostState.FRIGHTENED;
    }

    public void SetNormal()
    {
        foreach (var ghost in Ghosts)
            ghost.State = GhostState.NORMAL;
    }
   
}