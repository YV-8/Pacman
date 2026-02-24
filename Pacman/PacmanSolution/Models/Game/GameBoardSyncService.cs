using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media;
using PacmanSolution.Models.Entities;
using PacmanSolution.ViewModels;

namespace PacmanSolution.Models.Game;

/// <summary>
/// Synchronize the logic board with the logic of GameObjects
/// Use GameConfig for the map fits in the area the picture
/// </summary>
public class GameBoardSyncService
{
    private const double DotSize = 5;
    private const double EntitySize = 18;
    private readonly Dictionary<Entity, GameObject> _entityToVisual = new();
    private readonly ObservableCollection<GameObject> _visualObjects;
    private GameObject? _pacmanVisual;

    public GameBoardSyncService(ObservableCollection<GameObject> visualObjects)
    {
        _visualObjects = visualObjects;
    }

    /// <summary>
    /// browse the all board and create each relevant entity
    /// Pacman had visual the sprite  update with UpdatePacmanPosition
    /// </summary>
    public void BuildFromBoard(ObservableCollection<Entity> board)
    {
        _visualObjects.Clear();
        _entityToVisual.Clear();
        _pacmanVisual = null;

        foreach (var entity in board)

        {
            if (entity.Type == EntityType.PACMAN)
            {
                var visual = CreateVisualForEntity(entity);
                if (visual is null) continue;
                _pacmanVisual = visual;
                _visualObjects.Add(visual);
                continue;
            }

            var v = CreateVisualForEntity(entity);
            if (v is null) continue;

            _entityToVisual[entity] = v;
            _visualObjects.Add(v);
            entity.PropertyChanged += (_, args) => OnEntityChanged(entity, args.PropertyName);
        }
    }

    /// <summary>Update the visual position of pacman when it moves
    /// </summary>
    public void UpdatePacmanPosition(int row, int col)
    {
        if (_pacmanVisual is null) return;
        _pacmanVisual.X = col * GameConfig.CellWidth;
        _pacmanVisual.Y = row * GameConfig.CellHeight;
    }

    /// <summary>Update the pacman with the sprites
    /// </summary>
    public void UpdatePacmanSprite(object? sprite)
    {
        if (_pacmanVisual is null) return;
        _pacmanVisual.Sprite = sprite as IImage;
    }

    /// <summary>
    /// Hide the dot when the pacman eat it
    /// </summary>
    private void HideDot(Entity entity)
    {
        if (_entityToVisual.TryGetValue(entity, out var visual))
            _visualObjects.Remove(visual);
    }
    
    /// <summary>
    /// Check the ghost is in method BuildFromBoard
    /// in case it's not the method subscribe  with propertyChanged
    /// </summary>
    /// <param name="ghosts"></param>
    public void RegisterGhosts(ObservableCollection<Ghost> ghosts)
    {
        foreach (var ghost in ghosts)
        {
            if (_entityToVisual.ContainsKey(ghost))
            { continue; }
            var visual = CreateVisualForEntity(ghost);
            if (visual is null)
            { continue; }

            _entityToVisual[ghost] = visual;
            _visualObjects.Add(visual);
            ghost.PropertyChanged += (_, args) => OnEntityChanged(ghost, args.PropertyName);
        }
    }

    private GameObject? CreateVisualForEntity(Entity entity)
    {
        static GameObject MakeGhostVisual(Entity e) => new()
        {
            X      = e.Col * GameConfig.CellWidth,
            Y      = e.Row * GameConfig.CellHeight,
            Width  = EntitySize,
            Height = EntitySize,
            Zindex = 9
        };
        if (entity is Pellet pellet)
        {
            if (pellet.IsEnergizer)
            {
                return new GameObject
                {
                    X         = pellet.Col * GameConfig.CellWidth + 2,
                    Y         = pellet.Row * GameConfig.CellHeight + 2,
                    Width     = GameConfig.CellWidth - 4,
                    Height    = GameConfig.CellHeight - 4,
                    Zindex    = 5,
                    FillColor = Brushes.LemonChiffon
                };
            }
            return new GameObject
            {
                X         = pellet.Col * GameConfig.CellWidth + (GameConfig.CellWidth - DotSize) / 2,
                Y         = pellet.Row * GameConfig.CellHeight + (GameConfig.CellHeight - DotSize) / 2,
                Width     = DotSize,
                Height    = DotSize,
                Zindex    = 5,
                FillColor = Brushes.White
            };
        }
        return entity.Type switch
        {
            EntityType.PACMAN => new GameObject
            {
                X      = entity.Col * GameConfig.CellWidth,
                Y      = entity.Row * GameConfig.CellHeight,
                Width  = EntitySize,
                Height = EntitySize,
                Zindex = 10
            },
            EntityType.REDGHOST    => MakeGhostVisual(entity),
            EntityType.PINKGHOST   => MakeGhostVisual(entity),
            EntityType.CYANGHOST   => MakeGhostVisual(entity),
            EntityType.ORANGEGHOST => MakeGhostVisual(entity),
            _ => null
        };
    }

    private void OnEntityChanged(Entity entity, string? propertyName)
    {
        if (!_entityToVisual.TryGetValue(entity, out var visual))
        {
            return;
        }
        switch (propertyName)
        {
            case nameof(Entity.IsActive) when !entity.IsActive:
                HideDot(entity);
                break;
            case nameof(Entity.Row) or nameof(Entity.Col):
                visual.X = entity.Col * GameConfig.CellWidth;
                visual.Y = entity.Row * GameConfig.CellHeight;
                break;
            case nameof(Entity.CurrentDisplaySprite):
                visual.Sprite = entity.CurrentDisplaySprite;
                break;
        }
    }
}