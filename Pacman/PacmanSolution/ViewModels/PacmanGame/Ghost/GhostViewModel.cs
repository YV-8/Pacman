using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GhostViewModel :ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Ghost> _ghosts = new ();

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
    }
    private void InitializeGhosts()
    {
        Ghosts.Clear();
        foreach (var entity in _board.OfType<Ghost>())
        {
            Ghosts.Add(entity);
        }
    }
    
    public void GhostsTimer()
    {
        _globalAnimationFrame = (_globalAnimationFrame + 1) % 2;
        UpdateAllSprites();
    }
    
    private static readonly Dictionary<EntityType, int> SpriteRow = new()
    {
        { EntityType.REDGHOST,    0 },
        { EntityType.PINKGHOST,   1 },
        { EntityType.CYANGHOST,   2 },
        { EntityType.ORANGEGHOST, 3 },
    };
    private static readonly Dictionary<GhostDirection, int> DirectionBaseCol = new()
    {
        { GhostDirection.Right, 0 },
        { GhostDirection.Left,  2 },
        { GhostDirection.Up,    4 },
        { GhostDirection.Down,  6 },
    };
    public void UpdateAllSprites()
    {
        foreach (var ghost in Ghosts)
        {
            ghost.AnimationFrame = _globalAnimationFrame;
            ghost.CurrentDisplaySprite = GetGhostSprite(ghost);
        }
    }

    private IImage? GetGhostSprite(Ghost ghost)
    {
        int baseCol = DirectionBaseCol[ghost.Direction];
        int col = baseCol + _globalAnimationFrame;
        if (ghost.State is GhostState.Frightened)
        {
            var frightenedRect = new PixelRect(_globalAnimationFrame * _size, 4 * _size, _size, _size);
            return _spriteManager.GetSpriteSection("GhostViews.png", frightenedRect);  
        }

        if (!SpriteRow.TryGetValue(ghost.Type, out int row)) return null;
        var rect = new PixelRect(col*_size, row*_size, _size, _size);
        return _spriteManager.GetSpriteSection("GhostViews.png", rect);
    }

    public void SetFrightened()
    {
        foreach (var ghost in Ghosts)
            ghost.State = GhostState.Frightened;
    }

    public void SetNormal()
    {
        foreach (var ghost in Ghosts)
            ghost.State = GhostState.Normal;
    }
   
}