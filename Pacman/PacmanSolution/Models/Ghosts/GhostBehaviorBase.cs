using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PacmanSolution.Models;

namespace PacmanSolution.Model;

public abstract class GhostBehaviorBase
{
    protected static double Distance(int row, int col, int targetRow, int targetCol)
    { 
        double distanceRow = row - targetRow;
        double distanceCol = col - targetCol;
        double distanceRowCol = distanceRow * distanceRow + distanceCol * distanceCol;
        return distanceRowCol;
    }

    /// <summary>
    /// Calculate  the distance  for each  type ghost's  movmient
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static (int distanceRow, int distanceCol) DistanceDelta(GhostDirection distance)
    {
        switch (distance)
        {
            case GhostDirection.Left:
                return (0, -1);
            case GhostDirection.Right:
                return (0, 1);
            case GhostDirection.Up:
                return (-1, 0);
            case GhostDirection.Down:
                return (1, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(distance), distance, null);
        }
    }

    private static bool IsOpposite(GhostDirection current, GhostDirection otherDirectioni)
    {
        return (current == GhostDirection.Up    && otherDirectioni == GhostDirection.Down)
               || (current == GhostDirection.Down  && otherDirectioni == GhostDirection.Up)
               || (current == GhostDirection.Left  && otherDirectioni == GhostDirection.Right)
               || (current == GhostDirection.Right && otherDirectioni == GhostDirection.Left);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ghost"></param>
    /// <param name="targetRow"></param>
    /// <param name="targetCol"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    public GhostDirection GetBestDirectionToTarget(
        Ghost ghost, int targetRow, int targetCol,
        ObservableCollection<Entity> board)
    {
        var candidates = new List<(GhostDirection directionRow, double dist)>();

        foreach (GhostDirection directionRow in Enum.GetValues<GhostDirection>())
        {
            if (IsOpposite(directionRow, ghost.Direction)) continue;

            var (rowChange, colChange) = DistanceDelta(directionRow);
            int testRow = ghost.Row + rowChange;
            int testCol = ghost.Col + colChange;

            var entity = board.FirstOrDefault(e => e.Row == testRow && e.Col == testCol);
            if (entity is null || entity.Type is EntityType.WALL) continue;

            //if (entity.Type is EntityType.DOOR && ghost.State == GhostState.NORMAL) continue;
            if (entity.Type is EntityType.DOOR && 
                ghost.State != GhostState.INHOUSE && 
                ghost.State != GhostState.DEAD) continue;
            double dist = Distance(testRow, testCol, targetRow, targetCol);
            candidates.Add((directionRow, dist));
        }

        return candidates.Count > 0
            ? candidates.OrderBy(d => d.dist).First().directionRow
            : ghost.Direction;
    }
    private static (int row, int col) GetScatterCorner(EntityType type)
    {
        switch (type)
        {
            case EntityType.REDGHOST:    
                return (0, 27);
            case EntityType.PINKGHOST:   
                return (0, 0);
            case EntityType.CYANGHOST:   
                return (30, 27);
            case EntityType.ORANGEGHOST: 
                return (30, 0);
            default:                     
                return (0, 0);
        }
    }
    public GhostDirection GetScatterDirection(Ghost ghost,ObservableCollection<Entity> _board)
    {
        var (cornerRow, cornerCol) = GetScatterCorner(ghost.Type);
        return GetBestDirectionToTarget(ghost, cornerRow, cornerCol, _board);
    } 
}