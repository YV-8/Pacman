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
            ghost.Direction = GhostDirection.Up;
            return true;
        }
        ghost.Direction = GhostDirection.Up;
        var cell = board.FirstOrDefault(e => e.Row == ghost.Row - 1 && e.Col == ghost.Col);
        
        if (cell is not null && cell.Type is not EntityType.WALL)
        {
            ghost.Row--;
            ghost.UpdateCanvasPosition();
        }

        return false;
    }
}