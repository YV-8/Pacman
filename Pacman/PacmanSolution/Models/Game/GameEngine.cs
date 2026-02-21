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
    //public SpriteManager SpriteManager { get; private set; } = new SpriteManager();
    private int _frameCount;
    private double _fpsTimer;
    private DateTime _lastUpdateTime;
    private const int DotPoints = 10;
    private const int EnergizerPoints = 50;
    private const int cherryPoints = 100;
    public ObservableCollection<GameObject> VisualObjects { get; } = new();

    
    

    public GameEngine()
    {
        _lastUpdateTime = DateTime.Now;
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

    private void ChooseEffectPellet(Pellet pellet,InteractionResultObject result)
    {
        if (pellet.IsEnergizer)
        {
            result.PointsEarned = EnergizerPoints;
            result.RemovedElementType = "energizer";
        }
        else
        {
            result.PointsEarned = DotPoints;
            result.RemovedElementType = "dot";
        }
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