using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace PacmanSolution.Models;

public class GameBoardSyncService
{
    private const double CellSize = 60.0;
    private const double DotSize  = 10.0;
    private const double EntitySize = 40.0;

    // Mapeo Entity → su GameObject visual correspondiente
    private readonly Dictionary<Entity, GameObject> _entityToVisual = new();
    
    private readonly ObservableCollection<GameObject> _visualObjects;

    public GameBoardSyncService(ObservableCollection<GameObject> visualObjects)
    {
        _visualObjects = visualObjects;
    }

    /// <summary>
    /// Recorre el Board completo y crea un GameObject por cada Entity relevante
    /// </summary>
    public void BuildFromBoard(ObservableCollection<Entity> board)
    {
        _visualObjects.Clear();
        _entityToVisual.Clear();

        foreach (var entity in board)
        {
            var visual = CreateVisualForEntity(entity);
            if (visual is null) continue;

            _entityToVisual[entity] = visual;
            _visualObjects.Add(visual);

            // Suscribirse a cambios de la entidad para sincronizar
            entity.PropertyChanged += (_, args) => OnEntityChanged(entity, args.PropertyName);
        }
    }

    /// <summary>
    /// Llama esto cuando Pacman se mueve para actualizar su visual
    /// </summary>
    public void UpdatePacmanVisual(Entity pacmanEntity, Bitmap? sprite, Rect sourceRect)
    {
        if (!_entityToVisual.TryGetValue(pacmanEntity, out var visual)) return;
        visual.X          = pacmanEntity.Col * CellSize;
        visual.Y          = pacmanEntity.Row * CellSize;
        visual.Sprite     = sprite;
        visual.SourceRect = sourceRect;
    }

    /// <summary>
    /// Oculta el visual de un punto cuando Pacman lo come
    /// </summary>
    public void HideDot(Entity entity)
    {
        if (_entityToVisual.TryGetValue(entity, out var visual))
            _visualObjects.Remove(visual);
    }

    private GameObject? CreateVisualForEntity(Entity entity)
    {
        return entity.Type switch
        {
            EntityType.DOT or EntityType.EMPTY when entity.HasDot => new GameObject
            {
                X         = entity.Col * CellSize + (CellSize - DotSize) / 2,
                Y         = entity.Row * CellSize + (CellSize - DotSize) / 2,
                Width     = DotSize,
                Height    = DotSize,
                Zindex    = 5,
                FillColor = Brushes.White
            },
            EntityType.ENERGIZE => new GameObject
            {
                X         = entity.Col * CellSize + (CellSize - DotSize * 2) / 2,
                Y         = entity.Row * CellSize + (CellSize - DotSize * 2) / 2,
                Width     = DotSize * 2,
                Height    = DotSize * 2,
                Zindex    = 5,
                FillColor = Brushes.OrangeRed
            },
            EntityType.PACMAN => new GameObject
            {
                X      = entity.Col * CellSize,
                Y      = entity.Row * CellSize,
                Width  = EntitySize,
                Height = EntitySize,
                Zindex = 10
            },
            EntityType.REDGHOST => new GameObject
            {
                X      = entity.Col * CellSize,
                Y      = entity.Row * CellSize,
                Width  = EntitySize,
                Height = EntitySize,
                Zindex = 9
            },
            _ => null // WALL, DOOR, etc. no necesitan visual en el ItemsControl
        };
    }

    private void OnEntityChanged(Entity entity, string? propertyName)
    {
        if (!_entityToVisual.TryGetValue(entity, out var visual)) return;

        switch (propertyName)
        {
            case nameof(Entity.HasDot) when !entity.HasDot:
                HideDot(entity);
                break;
            case nameof(Entity.Row) or nameof(Entity.Col):
                visual.X = entity.Col * CellSize;
                visual.Y = entity.Row * CellSize;
                break;
            case nameof(Entity.CurrentDisplaySprite):
                visual.Sprite = entity.CurrentDisplaySprite as Bitmap;
                break;
        }
    }
}