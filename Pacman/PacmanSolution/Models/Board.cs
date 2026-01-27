using System.Collections.Generic;

namespace PacmanSolution.Models;

public class Board
{
    private int _boardWidth;
    private int _boardHeight;
    private List<Ghost> _ghosts;
    private Pacman _pacman;
}