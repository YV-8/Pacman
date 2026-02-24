using System.Collections.ObjectModel;
using System.Linq;

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
            if (ghost.State != GhostState.FRIGHTENED)
            {
                ghost.State = GhostState.NORMAL;
            }
            ghost.HunterMode = GhostHunterMode.Scatter;
            ghost.Direction = GhostDirection.Left;
            ghost.Row = ExitRow;
            ghost.UpdateCanvasPosition();
            return true;
        }
        if (ghost.Col is not DoorCol)
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