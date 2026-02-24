using System;
using System.Collections.ObjectModel;
using PacmanSolution.Models.Entities;

namespace PacmanSolution.Models.Ghosts;

public class ClydeBehavior:GhostBehaviorBase
{
     private const double _feardistance =  6; //huida
     private const int ScatterRow = 30;
     private const int ScatterCol = 1;
    
        public GhostDirection DecideNextDirection(Ghost ghost, Pacman pacman, ObservableCollection<Entity> board)
        {
            double distanceToPacman = Math.Sqrt(
                Distance(ghost.Row, ghost.Col, pacman.Row, pacman.Col)
            );

            int targetRow;
            int targetCol;

            if (distanceToPacman < _feardistance)
            {
                // Está cerca  huir a esquina inferior izquierda
                targetRow = ScatterRow;
                targetCol = ScatterCol;
            }
            else
            {
                targetRow = pacman.Row;
                targetCol = pacman.Col;
            }

            return GetBestDirectionToTarget(ghost, targetRow, targetCol, board);
        }
}