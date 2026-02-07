using System;
using System.Collections.Generic;

namespace PacmanSolution.Models;

public class GameEngine
{
    public const int TargetFPS = 60;

    public const int TargetFrameMS = 1000 / TargetFPS;

    public double TotalTime { get; set; }

    public int CurrentFPS { get; set; }
    public List<Entity> GameObjects { get; private set; } = new List<Entity>();

    public SpriteManager SpriteManager { get; private set; } = new SpriteManager();

    private int _frameCount;

    private double _fpsTimer;

    private DateTime _lastUpdateTime;

    public GameEngine()
    {
        _lastUpdateTime = DateTime.Now;
    }

    // LLamado en cada Tick
    public void Update()
    {
        //hora actual - hora anterior = diff
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

    }
}