using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PacmanSolution.Models.Entities;
using PacmanSolution.Models.Ghosts;

namespace PacmanSolution.Models;

public class GameEngine
{ 
    private double TotalTime { get; set; }
    public int CurrentFPS { get; set; }
    private List<Entity> GameObjects { get; set; } = new ();
    private const int DotPoints = 10;
    private const int EnergizerPoints = 50;
    private const int cherryPoints = 100;
    private const int GhostPoints = 200;
    private int TotalPellets { get; set; } = 0;
    private int EatenPellets { get; set; } = 0;
    private int _frameCount;
    private double _fpsTimer;
    private DateTime _lastUpdateTime;
    
    public event Action? PacmanDied;
    public event Action? OnEnergizerEaten;
    public event Action? LevelComplete;
    public event Action<int>? GhostEaten;
    public ObservableCollection<GameObject> VisualObjects { get; } = new();

    public GameEngine()
    {
        _lastUpdateTime = DateTime.Now;
    }
    /// <summary>
    /// Counts and stores the total number of pellets on the board
    /// so the engine knows when all have been eaten 
    /// </summary>
    public void InitPelletCount(ObservableCollection<Entity> board)
    {
        TotalPellets = board.OfType<Pellet>().Count();
        EatenPellets = 0;
    }
    
    /// <summary>
    /// Main game loop tick; calculates delta time, updates FPS counter
    /// and calls Update() on every active game object
    /// Removes inactive objects from the list automatically
    /// </summary>
    public void Update()
    {
        DateTime now = DateTime.Now;
        double diff = (now - _lastUpdateTime).TotalSeconds;
        _lastUpdateTime = now;

        TotalTime += diff;

        _frameCount++;

        _fpsTimer += diff;

        if (_fpsTimer >= 1.0)
        {
            CurrentFPS = _frameCount;
            _frameCount = 0;
            _fpsTimer = 0;
        }

        for (int i = GameObjects.Count - 1; i >= 0; i--)
        {
            var obj = GameObjects[i];

            if (!obj.IsActive)
            {
                GameObjects.RemoveAt(i);
                continue;
            }

            obj.Update(diff);

        }
    }
    /// <summary>
    /// Registers an entity into the game loop so it receives Update() calls
    /// </summary>
    public void AddGameObject(Entity obj)
    {
        GameObjects.Add(obj);
    }
    /// <summary>
    /// Removes an entity from the game loop immediately
    /// </summary>
    public void RemoveGameObject(Entity obj)
    {
        GameObjects.Remove(obj); 
    }
    
    /// <summary>
    /// Clears all game objects and resets all timers and counters
    /// </summary>
    public void Reset()
    {
        GameObjects.Clear();
        TotalTime = 0;
        CurrentFPS = 0;
        _frameCount = 0;
        _fpsTimer = 0;
    }

    /// <summary>
    /// Delete a objet visual tp Canvas
    /// </summary>
    public void RemoveVisualObject(GameObject obj)
    {
        VisualObjects.Remove(obj);
    }

    
    /// <summary>
    /// Ask is this position has cell or entity type wall or door is false
    /// the counter u opposite is true
    /// </summary>
    /// <param name="targetEntity"></param>
    /// <returns></returns>
    public bool CanMoveTo(Entity? targetEntity)
    {
        if (targetEntity is null) {return false;}
        if (targetEntity is Ghost) {return true;}
        if (targetEntity.Type is EntityType.WALL || targetEntity.Type is EntityType.DOOR)
        {
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// The method is 
    /// </summary>
    /// <param name="newEntity"></param>
    public InteractionResultObject InteractionObjects(Entity newEntity)
    {
        var result = new InteractionResultObject { Success = true };
        if (newEntity is Pellet pellet)
        {
            ChooseEffectPellet(pellet, result);
            return result;
        }

        if (newEntity.Type == EntityType.CHERRY)
        {
            result.PointsEarned = cherryPoints;
            result.RemovedElementType = "cherry";
        }

        return result;
    }
    /// <summary>
    /// Checks if a ghost and Pacman share the same cell and resolves the collision:
    /// Ghost FRIGHTENED → ghost dies, GhostEaten event fires with points.
    /// Ghost NORMAL → Pacman dies, all ghosts respawn, PacmanDied event fires.
    /// Ghost DEAD or IN HOUSE → no effect.
    /// Returns -1 if the ghost was eaten, 0 otherwise.
    /// </summary>
    /// <param name="ghost"></param>
    /// <param name="pacman"></param>
    /// <param name="_board"></param>
    /// <returns></returns>
    
    public int CollisionsToPacman(Ghost ghost, Pacman pacman,ObservableCollection<Entity> _board)
    {
        if (ghost.Row == pacman.Row && ghost.Col == pacman.Col)
        {
            if (ghost.State is GhostState.DEAD || ghost.State is GhostState.INHOUSE) return 0; 
            if (ghost.State == GhostState.FRIGHTENED)
            {
                GhostEaten?.Invoke(GhostPoints);
                ghost.RespawnGhost(ghost);
                if (ghost.State == GhostState.DEAD) ;
                return -1;
            }
            else if (ghost.State == GhostState.NORMAL)
            {
                pacman.RespawnPacman();
                ghost.RespawnAllGhost(_board);
                Console.WriteLine($"[Death Pacman] por {ghost.Type}");
                PacmanDied?.Invoke();
            }
        }
        return 0;
    }

    private void ChooseEffectPellet(Pellet pellet,InteractionResultObject result)
    {
        if (pellet.IsEnergizer)
        {
            result.PointsEarned = EnergizerPoints;
            result.RemovedElementType = "energizer";
            OnEnergizerEaten?.Invoke();
        }
        else
        {
            result.PointsEarned = DotPoints;
            result.RemovedElementType = "dot";
        }
        EatenPellets++;
        if (EatenPellets >= TotalPellets)
            LevelComplete?.Invoke();
    }
}