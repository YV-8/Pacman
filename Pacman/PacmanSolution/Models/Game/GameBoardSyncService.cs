using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Media;
using PacmanSolution.ViewModels;

namespace PacmanSolution.Models;

/// <summary>
/// Sincroniza el tablero lógico con los GameObjects visuales.
/// Usa GameConfig para que el mapa completo (28×31) quepa en el área de juego sin cortar.
/// </summary>
public class GameBoardSyncService
{
    private const double DotSize = 5.0;
    private const double EntitySize = 18.0;

    private readonly Dictionary<Entity, GameObject> _entityToVisual = new();
    private readonly ObservableCollection<GameObject> _visualObjects;
    private GameObject? _pacmanVisual;

    public GameBoardSyncService(ObservableCollection<GameObject> visualObjects)
    {
        _visualObjects = visualObjects;
    }

    /// <summary>
    /// Recorre el Board completo y crea un GameObject por cada Entity relevante.
    /// Pacman tiene un único visual; su posición se actualiza con UpdatePacmanPosition.
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

    /// <summary>Actualiza la posición del visual de Pacman cuando se mueve (teclas).</summary>
    public void UpdatePacmanPosition(int row, int col)
    {
        if (_pacmanVisual is null) return;
        _pacmanVisual.X = col * GameConfig.CellWidth;
        _pacmanVisual.Y = row * GameConfig.CellHeight;
    }

    /// <summary>Actualiza el sprite del visual de Pacman (Bitmap o CroppedBitmap, ambos usables como IImage).</summary>
    public void UpdatePacmanSprite(object? sprite)
    {
        if (_pacmanVisual is null) return;
        _pacmanVisual.Sprite = sprite as Avalonia.Media.IImage;
    }

    /// <summary>
    /// Oculta el visual de un punto cuando Pacman lo come
    /// </summary>
    public void HideDot(Entity entity)
    {
        if (_entityToVisual.TryGetValue(entity, out var visual))
            _visualObjects.Remove(visual);
    }
    
    public void RegisterGhosts(ObservableCollection<Ghost> ghosts)
    {
        foreach (var ghost in ghosts)
        {
            // Verificar si ya está registrado (viene de BuildFromBoard)
            if (_entityToVisual.ContainsKey(ghost))
            {
                Console.WriteLine($"[Sync] Ghost {ghost.Type} ya registrado en ({ghost.Row},{ghost.Col})");
                continue;
            }

            // Si no está, crear visual y registrar
            Console.WriteLine($"[Sync] Registrando ghost {ghost.Type} en ({ghost.Row},{ghost.Col})");
            var visual = CreateVisualForEntity(ghost);
            if (visual is null) continue;

            _entityToVisual[ghost] = visual;
            _visualObjects.Add(visual);
            ghost.PropertyChanged += (_, args) => OnEntityChanged(ghost, args.PropertyName);
        }
    }

    private GameObject? CreateVisualForEntity(Entity entity)
    {
        static GameObject MakeGhostVisual(Entity e) => new GameObject
        {
            X      = e.Col * GameConfig.CellWidth,
            Y      = e.Row * GameConfig.CellHeight,
            Width  = EntitySize,
            Height = EntitySize,
            Zindex = 9
        };
        return entity.Type switch
        {
            EntityType.DOT or EntityType.EMPTY when entity.HasDot => new GameObject
            {
                X         = entity.Col * GameConfig.CellWidth + (GameConfig.CellWidth - DotSize) / 2,
                Y         = entity.Row * GameConfig.CellHeight + (GameConfig.CellHeight - DotSize) / 2,
                Width     = DotSize,
                Height    = DotSize,
                Zindex    = 5,
                FillColor = Brushes.White
            },
            EntityType.ENERGIZE => new GameObject
            {
                X         = entity.Col * GameConfig.CellWidth + (GameConfig.CellWidth - DotSize * 2) / 2,
                Y         = entity.Row * GameConfig.CellHeight + (GameConfig.CellHeight - DotSize * 2) / 2,
                Width     = DotSize * 2,
                Height    = DotSize * 2,
                Zindex    = 5,
                FillColor = Brushes.BurlyWood
            },
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
        if (!_entityToVisual.TryGetValue(entity, out var visual)) return;
        Console.WriteLine($"[OnEntityChanged] {entity.Type} prop={propertyName} pos=({entity.Row},{entity.Col})");

        switch (propertyName)
        {
            case nameof(Entity.HasDot) when !entity.HasDot:
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