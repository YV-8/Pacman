using System;
using System.Collections.Generic;
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
    private readonly Dictionary<EntityType, (int row, int col)> _spawnPositions = new();

    private int _globalAnimationFrame = 0;
    private int _modeTimer = 0;
    private GhostDirection _pacmanDirection = GhostDirection.Left;
    private const int ScatterDuration = 28;
    private const int ChaseDuration = 80;
    private const int _size = 16;
    private Models.Pacman? _cachedPacman;

    public GhostViewModel(ObservableCollection<Entity> board,GameBoardSyncService? syncService)
    {
        _syncService = syncService;
        _board = board;
        InitializeGhosts();
    }
    public void SetPacmanDirection(GhostDirection direction)
    {
        _pacmanDirection = direction;
    }
    private void InitializeGhosts()
    {
        Ghosts.Clear();
        foreach (var entity in _board.OfType<Ghost>())
        {
            _spawnPositions[entity.Type] = (entity.Row, entity.Col);
            switch (entity.Type)
            {
                case EntityType.REDGHOST:
                    entity.ExitDelayTicks = 0;
                    entity.State = GhostState.INHOUSE;
                    break;
                case EntityType.PINKGHOST:
                    entity.ExitDelayTicks = 10;
                    entity.State = GhostState.INHOUSE;
                    break;
                case EntityType.CYANGHOST:
                    entity.ExitDelayTicks = 25;
                    entity.State = GhostState.INHOUSE;
                    break;
                case EntityType.ORANGEGHOST:
                    entity.ExitDelayTicks = 45;
                    entity.State = GhostState.INHOUSE;
                    break;
            }
            Ghosts.Add(entity);
        }
    }

    public void GhostsTimer()
    {
        _globalAnimationFrame = (_globalAnimationFrame + 1) % 2;
        UpdateHunterMode();
        MoveGhosts();
        UpdateAllSprites();
        _modeTimer++;
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
    
    private void UpdateHunterMode()
    {
        GhostHunterMode newMode;
        var cycleLife = _modeTimer % (ScatterDuration + ChaseDuration);
        if (cycleLife < ScatterDuration)
        {
            newMode = GhostHunterMode.Scatter;
        }
        else
        {
            newMode = GhostHunterMode.Chase;
        }

        foreach (var ghost in Ghosts)
        {
            if (ghost.State is GhostState.NORMAL)
            {
                ghost.HunterMode = newMode;
            }
        }
    }

    private void MoveGhosts()
    {
        SyncGhostPositionsToBoard();
        _cachedPacman ??= _board.OfType<Models.Pacman>().FirstOrDefault();
        var pacman = _cachedPacman;
        var blinky = Ghosts.FirstOrDefault(g => g.Type == EntityType.REDGHOST);
        Console.WriteLine($"[MoveGhosts] pacman={pacman?.Row},{pacman?.Col} blinky={blinky?.Row},{blinky?.Col} blinkyType={blinky?.Type}");
        if (pacman is null || blinky is null) return;
        foreach (var ghost in  Ghosts)
        {
            Console.WriteLine($"[MoveGhosts] {ghost.Type} State={ghost.State} " +
                              $"Tick={_modeTimer} ExitDelay={ghost.ExitDelayTicks} " +
                              $"CanExit={ghost.CanExitHouse(_modeTimer)} " +
                              $"IntervalCheck={_modeTimer % GetMoveInterval(ghost)}");
            if (_modeTimer % GetMoveInterval(ghost) is not 0)
            {
                continue;
            }
            
            if (ghost.State is GhostState.INHOUSE)
            {
                Console.WriteLine($"[MoveGhosts] {ghost.Type} intentando TryExit desde ({ghost.Row},{ghost.Col})");
                if (!ghost.CanExitHouse(_modeTimer)) { continue; }
                var exited = _house.TryExit(ghost, _board);
                Console.WriteLine($"[MoveGhosts] {ghost.Type} TryExit resultado={exited} nueva pos=({ghost.Row},{ghost.Col})");
                continue;
            }
            GhostDirection nextDirection = AssignDirection(ghost,pacman,blinky);

            ApplyMove(ghost, nextDirection);
            ColisionsToPacman(ghost, pacman);
        }
            
    }
    
    private GhostDirection AssignDirection(Ghost ghost, Models.Pacman pacman,Ghost blinky)
    {
        GhostDirection nextDirection = Models.GhostDirection.Up;
        if (ghost.State == GhostState.FRIGHTENED)
        {
            nextDirection = GetRandomDirection(ghost);
        }
        else if (ghost.HunterMode == GhostHunterMode.Scatter)
        {
            nextDirection = _blinky.GetScatterDirection(ghost, _board);
        }
        else
        {
            switch (ghost.Type)
            {
                case EntityType.REDGHOST:
                    nextDirection = _blinky.DecideNextDirection(ghost, pacman, _board);
                    break;
                case EntityType.PINKGHOST:
                    nextDirection = _pinky.DecideNextDirection(ghost, pacman, _pacmanDirection, _board);
                    break;
                case EntityType.CYANGHOST:
                    nextDirection = _inky.DecideNextDirection(ghost, pacman, _pacmanDirection, blinky, _board);
                    break;
                case EntityType.ORANGEGHOST:
                    nextDirection = _clyde.DecideNextDirection(ghost, pacman, _board);
                    break;
                default:
                    nextDirection = ghost.Direction;
                    break;
            }
        }

        return nextDirection;
    }
    
    private GhostDirection GetRandomDirection(Ghost ghost)
    {
        var random = new Random();
        var valid = new List<GhostDirection>();

        foreach (GhostDirection dir in Enum.GetValues<GhostDirection>())
        {
            var (rowChange, colChange) = GhostBehaviorBase.DistanceDelta(dir);
            var cell = _board.FirstOrDefault(e =>
                e.Row == ghost.Row + rowChange &&
                e.Col == ghost.Col + colChange);

            if (cell is not null && cell.Type != EntityType.WALL)
            {
                valid.Add(dir);
            }
        }

        return valid.Count > 0 ? valid[random.Next(valid.Count)] : ghost.Direction;
    }
    
    private void ApplyMove(Ghost ghost, GhostDirection dir)
    {
        var (rowChange, colChange) = GhostBehaviorBase.DistanceDelta(dir);
        int newRow = ghost.Row + rowChange;
        int newCol = ghost.Col + colChange;

        var cell = _board.FirstOrDefault(e => e.Row == newRow && e.Col == newCol);
        Console.WriteLine($"[ApplyMove] {ghost.Type} dir={dir} ({ghost.Row},{ghost.Col})→({newRow},{newCol}) cell={cell?.Type}");
        if (cell is null || cell.Type is EntityType.WALL) return;
        if (cell.Type == EntityType.DOOR && ghost.State == GhostState.NORMAL) return;
        
        ghost.Direction = dir;
        ghost.Row = newRow;
        ghost.Col = newCol;
        ghost.UpdateCanvasPosition();
    }
    private static int GetMoveInterval(Ghost ghost)
    {
        switch (ghost.Type)
        {
            case EntityType.REDGHOST:    
                return 2;
            case EntityType.PINKGHOST:   
                return 3;
            case EntityType.CYANGHOST:   
                return 3;
            case EntityType.ORANGEGHOST: 
                return 4;
            default:                     
                return 3;
        }
    }
    
    private void SyncGhostPositionsToBoard()
    {
        foreach (var cell in _board)
        {
            // Solo limpiar celdas que NO sean instancias Ghost reales
            // pero tengan tipo de fantasma (celdas huérfanas)
            if (cell is not Ghost && 
                cell.Type is EntityType.REDGHOST or EntityType.PINKGHOST 
                    or EntityType.CYANGHOST or EntityType.ORANGEGHOST)
            {
                cell.Type = EntityType.EMPTY;
            }
        }
    }

    private void ColisionsToPacman(Ghost ghost, Models.Pacman pacman)
    {
        if (ghost.Row == pacman.Row && ghost.Col == pacman.Col)
        {
            if (ghost.State == GhostState.FRIGHTENED)
            {
                //RespawnGhost(ghost);
                // Pacman come al fantasma
                ghost.State = GhostState.INHOUSE;
                ghost.Row = 14; // regresa a la casa
                ghost.Col = 13;
                ghost.UpdateCanvasPosition();
                Console.WriteLine($"[Colisión] Pacman comió a {ghost.Type}");
            }
            else
            {
                // Fantasma mata a Pacman
                Console.WriteLine($"[Colisión] {ghost.Type} mató a Pacman");
                // Aquí disparas el evento de muerte de Pacman
            }
        }
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

    private void SetInHouse()
    {
        foreach (var ghost in Ghosts)
            ghost.State = GhostState.INHOUSE;
    }
    public int GetModeTimer() => _modeTimer;
    public void RespawnGhost(Ghost ghost)
    {
        if (!_spawnPositions.TryGetValue(ghost.Type, out var spawn)) return;
    
        ghost.Row = spawn.row;
        ghost.Col = spawn.col;
        ghost.State = GhostState.INHOUSE;
        ghost.Direction = GhostDirection.Left;
        ghost.UpdateCanvasPosition();
    
        // Resetear el delay para que espere antes de salir de nuevo
        switch (ghost.Type)
        {
            case EntityType.REDGHOST:   ghost.ExitDelayTicks = _modeTimer + 0;  break;
            case EntityType.PINKGHOST:  ghost.ExitDelayTicks = _modeTimer + 10; break;
            case EntityType.CYANGHOST:  ghost.ExitDelayTicks = _modeTimer + 25; break;
            case EntityType.ORANGEGHOST:ghost.ExitDelayTicks = _modeTimer + 45; break;
        }
    }
}