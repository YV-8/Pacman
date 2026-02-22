using System.Collections.ObjectModel;
using System.Linq;
using PacmanSolution.Model;

namespace PacmanSolution.Models.Ghosts;

public class HouseBehavior:GhostBehaviorBase
{
    private const int DoorRow = 12;
    private const int DoorCol = 13;
    private const int ExitRow = 11;
    private const int ExitCol = 13;
    public bool TryExit(Ghost ghost, ObservableCollection<Entity> board)
    {
        if (ghost.Row <= DoorRow)
        {
            ghost.State = GhostState.NORMAL;
            ghost.HunterMode = GhostHunterMode.Scatter;
            ghost.Direction = GhostDirection.Left;
            ghost.Row = ExitRow;
            return true;
        }
        /*if (ghost.Col is not DoorCol)
        {
            int colDir = ghost.Col < DoorCol ? 1 : -1;
            var centerCell = board.FirstOrDefault(e => e.Row == ghost.Row && e.Col == ghost.Col + colDir);
            if (centerCell is not null && centerCell.Type is not EntityType.WALL)
            {
                if (centerCell is not null && centerCell.Type is not EntityType.WALL)
                {
                    ghost.Direction = colDir > 0 ? GhostDirection.Right : GhostDirection.Left;
                    ghost.Col += colDir;
                    ghost.UpdateCanvasPosition();
                }
            }
            return false;
        }
        ghost.Direction = GhostDirection.Up;
        var cell = board.FirstOrDefault(e => e.Row == ghost.Row - 1 && e.Col == ghost.Col);
        
        if (cell is not null && cell.Type is not EntityType.WALL)
        {
            ghost.Row--;
            ghost.UpdateCanvasPosition();
        }*/
        if (ghost.Col != DoorCol)
        {
            int colDir = ghost.Col < DoorCol ? 1 : -1;
            ghost.Direction = colDir > 0 ? GhostDirection.Right : GhostDirection.Left;
            ghost.Col += colDir;
            ghost.UpdateCanvasPosition();
            return false;
        }
        // Subir directamente — dentro de la casa no hay paredes arriba
        ghost.Direction = GhostDirection.Up;
        ghost.Row--;
        ghost.UpdateCanvasPosition();

        return false;
    }
}