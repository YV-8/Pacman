using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using PacmanSolution.Model;
using PacmanSolution.Models;
using PacmanSolution.Models.Game;
using PacmanSolution.Models.Ghosts;

namespace PacmanSolution.ViewModels;

public partial class GhostViewModel :ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Ghost> _ghosts = new ();
    
    private readonly HouseBehavior  _house  = new();
    private readonly ObservableCollection<Entity> _board;
    private readonly SpriteManager _spriteManager = new();
    private readonly GameBoardSyncService? _syncService;
    private readonly Models.Pacman _pacmanModel;
    private readonly Dictionary<EntityType, (int row, int col)> _spawnPositions = new();

    private int _globalAnimationFrame = 0;
    private int _modeTimer = 0;
    private const int ScatterDuration = 28;
    private const int ChaseDuration = 80;
    private const int _size = 16;
    private GhostDirection _pacmanDirection = GhostDirection.Left;
    private GameEngine _gameEngine;
    private Ghost _ghostModel;
    public int GetModeTimer() => _modeTimer;

    public GhostViewModel(ObservableCollection<Entity> board,GameBoardSyncService? syncService, 
        Models.Pacman pacmanModel)
    {
        _syncService = syncService;
        _board = board;
        _pacmanModel = pacmanModel;
        InitializeGhosts();
    }
    /*public void SetPacmanDirection(GhostDirection direction)
    {
        _pacmanDirection = direction;
    }*/
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

    /// <summary>
    /// Update the animation frame each ghost
    /// then Update the GhostMode
    /// the mode each ghost and update the sprites
    /// </summary>
    public void GhostsTimer()
    {
        _globalAnimationFrame = (_globalAnimationFrame + 1) % 2;
        UpdateHunterMode();
        MoveGhosts();
        UpdateAllSprites();
        _modeTimer++;
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
    /// <summary>
    /// each ghost take the entity en animation  and current sprite
    /// </summary>
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
            int colFright =8 + _globalAnimationFrame;
            var frightenedRect = new PixelRect(colFright * _size, 0 * _size, _size, _size);
            return _spriteManager.GetSpriteSection("GhostViews.png", frightenedRect);
        }

        int row = ghost.GetSpriteRow(ghost.Type);
        int col = GetDirectionBaseCol(ghost.Direction) + _globalAnimationFrame;
        return _spriteManager.GetSpriteSection("GhostViews.png", 
            new PixelRect(col * _size, row * _size, _size, _size));
    }
    
    private void UpdateHunterMode()
    {
        
        var cycleLife = _modeTimer % (ScatterDuration + ChaseDuration);
        GhostHunterMode newMode = cycleLife < ScatterDuration ? GhostHunterMode.Scatter : GhostHunterMode.Chase;

        foreach (var ghost in Ghosts)
        {
            if (ghost.State is not GhostState.INHOUSE && 
                ghost.State is not GhostState.FRIGHTENED)
            {
                ghost.HunterMode = newMode;
            }
        }
    }
    
    /// <summary>
    /// Manage the ghost with the first position of the pacman and
    /// other ghost the first where each ghost  get an interval time
    /// then in order each ghost out go the house also give next direction
    /// </summary>
    private void MoveGhosts()
    {
        SyncGhostPositionsToBoard();
        var pacman = _pacmanModel;
        var blinky = Ghosts.FirstOrDefault(g => g.Type == EntityType.REDGHOST);
        if (pacman is null || blinky is null) return;
        foreach (var ghost in  Ghosts)
        {
            if (_modeTimer % ghost.GetMoveInterval(ghost) is not 0)
            {
                continue;
            }
            
            if (ghost.State is GhostState.INHOUSE)
            {
                if (!ghost.CanExitHouse(_modeTimer)) { continue; }
                var exited = _house.TryExit(ghost, _board);
                continue;
            }
            GhostDirection nextDirection = ghost.AssignDirection(ghost,pacman,blinky,_board);

            ghost.ApplyGhostsMove(ghost, nextDirection, _board);
            CollisionsToPacman(ghost, pacman);
        }
            
    }
    
    /// <summary>
    /// Synchronize  the position with the board
    /// and change the cell
    /// </summary>
    private void SyncGhostPositionsToBoard()
    {
        foreach (var cell in _board)
        {
            if(cell is Ghost) continue;
            if (cell is not Ghost && 
                cell.Type is EntityType.REDGHOST or EntityType.PINKGHOST 
                    or EntityType.CYANGHOST or EntityType.ORANGEGHOST)
            {
                cell.Type = EntityType.EMPTY;
            }
        }
    }

    private void CollisionsToPacman(Ghost ghost, Models.Pacman pacman)
    {
        if (ghost.Row == pacman.Row && ghost.Col == pacman.Col)
        {
            if (ghost.State is GhostState.FRIGHTENED)
            {
                RespawnGhost(ghost);
                // Pacman come al fantasma
                ghost.State = GhostState.INHOUSE;
                ghost.Row = 14; // regresa a la casa
                ghost.Col = 13;
                ghost.UpdateCanvasPosition();
                Console.WriteLine($"[Colisión] Pacman comió a {ghost.Type}");
            }
            else
            {
                //pacman.Die
                Console.WriteLine($"[Colisión] {ghost.Type} mató a Pacman");
                // Aquí disparas el evento de muerte de Pacman
            }
        }
    }
    private void RespawnGhost(Ghost ghost)
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
    public void SetFrightened()
    {
        foreach (var ghost in Ghosts)
        {
            ghost.State = GhostState.FRIGHTENED;
        }
        UpdateAllSprites();    
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
}