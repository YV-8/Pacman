using System.Collections.ObjectModel;
using PacmanSolution.Models.Entities;

namespace PacmanSolution.Models.Ghosts;

public class BlinkyBehavior:GhostBehaviorBase
{
     public GhostDirection DecideNextDirection(Ghost ghost, Pacman pacman, ObservableCollection<Entity> board)
     {
          return GetBestDirectionToTarget(ghost, pacman.Row, pacman.Col, board);
     }
       
}