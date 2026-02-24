using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PacmanSolution.Models.Entities;

namespace PacmanSolution.Models.Ghosts;

public class InkyBehavior:GhostBehaviorBase
{
    // se distrae cada cierto tiempo
        private readonly Random _random = new Random();
        private int _decisionCounter = 0;
        private const int _randomDecisionUse = 240;
        public GhostDirection DecideNextDirection(Ghost ghost, Pacman pacman, GhostDirection pacmanDirection,
            Ghost blinky, ObservableCollection<Entity> board)
        {
            var (dr, dc) = DistanceDelta(pacmanDirection);
            int pivotRow = pacman.Row + dr * 2;
            int pivotCol = pacman.Col + dc * 2;

            // Duplicar el vector Blinky → pivot
            int targetRow = pivotRow + (pivotRow - blinky.Row);
            int targetCol = pivotCol + (pivotCol - blinky.Col);
            return GetBestDirectionToTarget(ghost, targetRow, targetCol, board); 
        }
}