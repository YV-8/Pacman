using System;
using System.Collections.Generic;

namespace PacmanSolution.Models;

public class GameEngine
{
    private const int TargetFPS = 60;
    public const int TargetFrameMS = 1000 / TargetFPS;
    private double TotalTime { get; set; }
    public int CurrentFPS { get; set; }
    private List<Entity> GameObjects { get; set; } = new List<Entity>();
    public SpriteManager SpriteManager { get; private set; } = new SpriteManager();
    private int _frameCount;
    private double _fpsTimer;
    private DateTime _lastUpdateTime;
    private const int DotPoints = 10;
    private const int EnergizerPoints = 50;
    private const int cherryPoints = 100;

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
    /// Ask is this position has cell or entity type wall or door
    /// </summary>
    /// <param name="targetEntity"></param>
    /// <returns></returns>
    public bool CanMoveTo(Entity targetEntity)
    {
        var canMove = false;
        if (targetEntity is null)
        {
            return canMove;
        } 
        if (targetEntity.Type is not CellType.WALL && targetEntity.Type is not CellType.DOOR)
        {
            canMove = true;
        }
        return canMove;
    }
    
    /// <summary>
    /// The method is 
    /// </summary>
    /// <param name="newEntity"></param>
    public InteractionResultObject InteractionObjects(Entity newEntity)
    {
        /*string newCellType ="pellet";
        bool _ateSomething = false;
        if (newCell.HasPellet)
        {
            newCell.HasPellet = false;
            Score += 10;
            _ateSomething = true;
        }
        if (newCell.Type is CellType.ENERGIZE)
        {
            newCell.Type = CellType.EMPTY;
            newCellType ="energizer";
            _ateSomething = true;
            Score += 50;
        }
        if (_ateSomething)
        {
            OnElementRemoved?.Invoke(this, 
                new ElementRemovedEventArgs(newCellType, newRow, newCol));
            UpdateScoreViewCommand(newCellType);
        }*/
        var result = new InteractionResultObject { Success = true };

        if (newEntity.HasDot) {
            newEntity.HasDot = false;
            result.PointsEarned = DotPoints;
            result.RemovedElementType = "pellet";
        }else if (newEntity.Type == CellType.ENERGIZE) {
            newEntity.Type = CellType.EMPTY;
            result.PointsEarned = EnergizerPoints;
            result.RemovedElementType = "energizer";
        }else if (newEntity.Type == CellType.CHERRY)
        {
            newEntity.Type = CellType.EMPTY;
            result.PointsEarned = cherryPoints;
            result.RemovedElementType = "cherry";
            result.Success = true;
        }
        
        return result;
    }
    
    /// <summary>
    /// verificate the point had the score
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