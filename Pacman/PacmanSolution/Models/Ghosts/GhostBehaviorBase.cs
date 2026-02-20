using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PacmanSolution.Models;

namespace PacmanSolution.Model;

public abstract class GhostBehaviorBase
{
    private static double Distance(int row, int col, int targetRow, int targetCol)
    {
        double distanceRow = row - targetRow;
        double distanceCol = col - targetCol;
        double distanceRowCol = distanceRow * distanceRow + distanceCol * distanceCol;
        return distanceRowCol;
    }

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

    private static bool IsOpposite(GhostDirection rowChange, GhostDirection otherDirectioni)
    {
        switch (rowChange)
        {
            case GhostDirection.Up:
                return rowChange == GhostDirection.Down;
            case GhostDirection.Down:
                return rowChange == GhostDirection.Up;
            case GhostDirection.Left:
                return otherDirectioni == GhostDirection.Right;
            case GhostDirection.Right:
                return otherDirectioni == GhostDirection.Left;
            default:
            {
                return false;
            }
        }
    }
    protected GhostDirection GetBestDirectionToTarget(
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

            double dist = Distance(testRow, testCol, targetRow, targetCol);
            candidates.Add((directionRow, dist));
        }

        return candidates.Count > 0
            ? candidates.OrderBy(d => d.dist).First().directionRow
            : ghost.Direction;
    }
    
}