using System.Collections.ObjectModel;
using PacmanSolution.Model;

namespace PacmanSolution.Models.Ghosts;

public class PinkyBehavior: GhostBehaviorBase
{
    private const int AheadCells = 4;

    public GhostDirection DecideNextDirection(Ghost ghost,Pacman pacman, GhostDirection pacmanDirection, ObservableCollection<Entity> board)
    {
        // 4 celdas adelante de Pacman según su dirección
        var (dr, dc) = DistanceDelta(pacmanDirection);
        int targetRow = pacman.Row + dr * AheadCells;
        int targetCol = pacman.Col + dc * AheadCells;

        return GetBestDirectionToTarget(ghost, targetRow, targetCol, board);
    }
}