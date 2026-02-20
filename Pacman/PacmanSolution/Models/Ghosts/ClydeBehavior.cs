using System;
using System.Collections.ObjectModel;
using PacmanSolution.Model;

namespace PacmanSolution.Models.Ghosts;

public class ClydeBehavior:GhostBehaviorBase
{
     private const double _feardistance =  6; //huida
     private const int ScatterRow = 30;
     private const int ScatterCol = 1;

        /*public GhostDirection DecideNextDirection(
            Ghost ghost,
            Pacman pacman,
            GameMap map,
            List<Ghost>? allGhosts = null)
        {
            double dx = pacman.X - ghost.X;
            double dy = pacman.Y - ghost.Y;
            double distanceToPacman = Math.Sqrt(dx * dx + dy * dy);

            double targetX;
            double targetY;

            if (distanceToPacman < _feardistance)
            {
                //huir a la esquina abajo a la izquierda si esta cerca a pacman
                targetX = 0;
                targetY = map.Rows * 16;
            }
            else
            {
                //perseguir como blinky si esta lejos
                targetX = pacman.X;
                targetY = pacman.Y;
            }

            return GetBestDirectionToTarget(ghost, targetX, targetY, map);
        }

        private GhostDirection GetBestDirectionToTarget(Ghost ghost, double targetX, double targetY, GameMap map)
        {
            var possibleDirections = new List<(GhostDirection dir, double distance)>();

            foreach (GhostDirection dir in Enum.GetValues<GhostDirection>())
            {
                if (IsOpposite(dir, ghost.Direction)) continue;

                double testX = ghost.X;
                double testY = ghost.Y;

                switch (dir)
                {
                    case GhostDirection.Up:
                        testY -= 16;
                        break;
                    case GhostDirection.Down:
                        testY += 16;
                        break;
                    case GhostDirection.Left:
                        testX -= 16;
                        break;
                    case GhostDirection.Right:
                        testX += 16;
                        break;
                }

                if (map.CanMoveTo(testX, testY, ghost.Width, ghost.Height))
                {
                    double dx = targetX - testX;
                    double dy = targetY - testY;

                    double distance = dx * dx + dy * dy;

                    possibleDirections.Add((dir, distance));
                }
            }

            if (possibleDirections.Count > 0)
            {
                return possibleDirections.OrderBy(d => d.distance).First().dir;
            }

            return ghost.Direction;
        }

        private bool IsOpposite(GhostDirection dir1, GhostDirection dir2)
        {
            return (dir1 == GhostDirection.Up && dir2 == GhostDirection.Down) ||
                   (dir1 == GhostDirection.Down && dir2 == GhostDirection.Up) ||
                   (dir1 == GhostDirection.Left && dir2 == GhostDirection.Right) ||
                   (dir1 == GhostDirection.Right && dir2 == GhostDirection.Left);
        }
    }*/

        public GhostDirection DecideNextDirection(Ghost ghost, Pacman pacman, ObservableCollection<Entity> board)
        {
            double distanceToPacman = Math.Sqrt(
                Distance(ghost.Row, ghost.Col, pacman.Row, pacman.Col)
            );

            int targetRow;
            int targetCol;

            if (distanceToPacman < _feardistance)
            {
                // Está cerca — huir a esquina inferior izquierda
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