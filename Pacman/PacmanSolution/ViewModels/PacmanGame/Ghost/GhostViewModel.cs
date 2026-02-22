using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
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
    private readonly ScoreBoardViewModel _score;
    
    private int _globalAnimationFrame;
    private int _modeTimer;
    private int _modeCycleTimer;
    private int _ghostsEatenThisRound = 0;
    private static readonly int[] GhostPoints = { 200, 400, 800, 1600 };
    private const int ScatterDuration = 45;
    private const int ChaseDuration = 80;
    private const int _size = 16;
    private GhostDirection _pacmanDirection = GhostDirection.Left;
    private GameEngine _gameEngine;
    private Ghost _ghostModel;
    private DispatcherTimer _timerFrighten;
    public int GetModeTimer() => _modeTimer;

    public GhostViewModel(ObservableCollection<Entity> board,GameBoardSyncService? syncService, 
        Models.Pacman pacmanModel,GameEngine engine,ScoreBoardViewModel score)
    {
        _syncService = syncService;
        _board = board;
        _pacmanModel = pacmanModel;
        _gameEngine = engine;
        _score = score;
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
            entity.SetupInitialState(entity.Row, entity.Col);
            // Guardamos en el diccionario por si lo necesitas para otras lógicas
            _spawnPositions[entity.Type] = (entity.Row, entity.Col);
        
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
        if (ghost.State == GhostState.DEAD)
        {
            int deadFright =9 + _globalAnimationFrame;
            var deadRect = new PixelRect(deadFright * _size, 1 * _size, _size, _size);
            return _spriteManager.GetSpriteSection("GhostViews.png", deadRect);
        }
        
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
        bool anyOutside = Ghosts.Any(g => g.State == GhostState.NORMAL);
    
        if (anyOutside)
            _modeCycleTimer++;
        var cycleLife = _modeCycleTimer % (ScatterDuration + ChaseDuration);
        GhostHunterMode newMode = cycleLife < ScatterDuration ? GhostHunterMode.Scatter : GhostHunterMode.Chase;

        foreach (var ghost in Ghosts)
        {
            if (ghost.State == GhostState.NORMAL)
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
        var pacman = _pacmanModel;
        var blinky = Ghosts.FirstOrDefault(g => g.Type == EntityType.REDGHOST);
        if (pacman is null || blinky is null) return;
        foreach (var ghost in  Ghosts)
        {
            if (_modeTimer % ghost.GetMoveInterval(ghost) is not 0)
            {
                continue;
            }
            if (ghost.State == GhostState.DEAD)
            {
                ghost.DeadTicksRemaining--;
                if (ghost.Row < ghost.SpawnRow) ghost.Row++;
                else if (ghost.Row > ghost.SpawnRow) ghost.Row--;
            
                if (ghost.Col < ghost.SpawnCol) ghost.Col++;
                else if (ghost.Col > ghost.SpawnCol) ghost.Col--;

                ghost.UpdateCanvasPosition();

                // ¿Llegó a casa?
                if (ghost.DeadTicksRemaining <= 0 || 
                    (ghost.Row == ghost.SpawnRow && ghost.Col == ghost.SpawnCol))
                {
                    ghost.Row = ghost.SpawnRow;
                    ghost.Col = ghost.SpawnCol;
                    ghost.State = GhostState.INHOUSE;
                    ghost.Direction = GhostDirection.Left;
                    ghost.HunterMode = GhostHunterMode.Chase;
                    int delay = ghost.GetRespawnDelay(ghost.Type);
                    ghost.ExitDelayTicks = _modeTimer + delay;
                    ghost.UpdateCanvasPosition();
                }
                continue;
            }
            
            if (ghost.State is GhostState.INHOUSE)
            {
                if (!ghost.CanExitHouse(_modeTimer)) { continue; }
                bool exited = _house.TryExit(ghost, _board);
                if (!exited) continue;
                ghost.ApplyGhostsMove(ghost, nextDirection, _board);
                int collisionResult = _gameEngine.CollisionsToPacman(ghost, pacman, _modeTimer);
                if (collisionResult == -1)
                {
                    int points = GetGhostPoints();
                    _score.amount(points);
                    Console.WriteLine($"THe pacman ate ghost{points}");
                }
            }
            GhostDirection nextDirection = ghost.AssignDirection(ghost,pacman,blinky,_board);

            ghost.ApplyGhostsMove(ghost, nextDirection, _board);
            int collisionResult = _gameEngine.CollisionsToPacman(ghost, pacman, _modeTimer);
            if (collisionResult == -1)
            {
                int points = GetGhostPoints();
                _score.amount(points);
                Console.WriteLine($"THe pacman ate ghost{points}");
            }
        }
            
    }
    
    public void SetFrightened()
    {
        _ghostsEatenThisRound = 0;
        foreach (var ghost in Ghosts)
        {
            if (ghost.State != GhostState.DEAD)
            {
                ghost.State = GhostState.FRIGHTENED;
            }
        }
        UpdateAllSprites();    
    }

    public void StartFrightenedMode()
    {
        _timerFrighten?.Stop();
        foreach (var ghost in Ghosts)
        {
            if (ghost.State == GhostState.NORMAL)
                ghost.State = GhostState.FRIGHTENED;
        }
        UpdateAllSprites();

        _timerFrighten = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(8000)
        };
        _timerFrighten.Tick += (s, e) =>
        {
            _timerFrighten.Stop();
            SetNormal();
        };
        _timerFrighten.Start();
    }
    
    public int GetGhostPoints()
    {
        int index = Math.Min(_ghostsEatenThisRound, GhostPoints.Length - 1);
        int points = GhostPoints[index];
        _ghostsEatenThisRound++;
        return points;
    }

    public void SetNormal()
    {
        foreach (var ghost in Ghosts)
        {
            if (ghost.State == GhostState.FRIGHTENED)
                ghost.State = GhostState.NORMAL;
        }
        
        UpdateAllSprites();
    }
    public void PauseFrightenedTimer() => _timerFrighten?.Stop();
    public void ResumeFrightenedTimer()
    {
        if (_timerFrighten is not null)
            _timerFrighten.Start();
    }

    private void SetInHouse()
    {
        foreach (var ghost in Ghosts)
        {
            if (ghost.State == GhostState.FRIGHTENED)
            {ghost.State = GhostState.NORMAL; }
        }
    }
}