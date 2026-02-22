using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PacmanSolution.Models;

public class GameEngine
{
    private const int TargetFPS = 60;
    //public const int TargetFrameMS = 1000 / TargetFPS;
    private double TotalTime { get; set; }
    public int CurrentFPS { get; set; }
    private List<Entity> GameObjects { get; set; } = new List<Entity>();
    private int _frameCount;
    private double _fpsTimer;
    private DateTime _lastUpdateTime;
    private const int DotPoints = 10;
    private const int EnergizerPoints = 50;
    private const int cherryPoints = 100;
    public event Action? PacmanDied;
    public event Action? OnEnergizerEaten;
    public ObservableCollection<GameObject> VisualObjects { get; } = new();
    public int TotalPellets { get; private set; } = 0;
    public int EatenPellets { get; private set; } = 0;
    public event Action? LevelComplete;
    public event Action<int>? GhostEaten;

    public GameEngine()
    {
        _lastUpdateTime = DateTime.Now;
    }
    public void InitPelletCount(ObservableCollection<Entity> board)
    {
        TotalPellets = board.OfType<Pellet>().Count();
        EatenPellets = 0;
    }
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
    public void AddGameObject(Entity obj)
    {
        GameObjects.Add(obj);
    }
    public void RemoveGameObject(Entity obj)
    {
        GameObjects.Remove(obj); 
    }
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
    public bool CanMoveTo(Entity targetEntity)
    {
        if (targetEntity is null) return false;
        if (targetEntity is Ghost) return true;
        if (targetEntity.Type is EntityType.REDGHOST or EntityType.PINKGHOST
            or EntityType.CYANGHOST or EntityType.ORANGEGHOST) return true;
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
    
    public int CollisionsToPacman(Ghost ghost, Models.Pacman pacman, int _modeTimer)
    {
        if (ghost.Row == pacman.Row && ghost.Col == pacman.Col)
        {
            if (ghost.State is GhostState.DEAD || ghost.State is GhostState.INHOUSE) return 0; 
            if (ghost.State == GhostState.FRIGHTENED)
            {
                ghost.RespawnGhost(ghost);
                if (ghost.State == GhostState.DEAD) ;
                return -1;
            }
            else if (ghost.State == GhostState.NORMAL)
            {
                
                pacman.RespawnPacman(pacman);
                Console.WriteLine($"[Muerte Pacman] por {ghost.Type}");
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
            OnEnergizerEaten?.Invoke();//evento para el pacman pero podria usar lo que uso para que los fantasmas sean comidos
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

    private void EffectDotPoints(Entity newEntity, InteractionResultObject result)
    {
        
        if (newEntity.Type == EntityType.CHERRY)
        {
            result.PointsEarned = cherryPoints;
            result.RemovedElementType = "cherry";
        }
    }
    
    /// <summary>
    /// verify the point had the score
    /// </summary>
    public void ScoreStateValidate(int Score, int _totalScore, int HighScore)
    {
        if (Score < _totalScore)
        {
            Console.WriteLine("Winner");
        }else if (Score > HighScore)
        {
            HighScore = Score;
            Console.WriteLine("Continue game");
        }
    }
}