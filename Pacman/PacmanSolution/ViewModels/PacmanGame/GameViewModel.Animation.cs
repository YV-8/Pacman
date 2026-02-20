using System;
using System.Linq;
using Avalonia.Threading;
using PacmanSolution.Models;

namespace PacmanSolution.ViewModels;

public partial class GameViewModel
{
    
    /// <summary>
    /// Initializes Pacman's position from the board
    /// </summary>
    private void InitializePacmanPosition()
    {
        var pacmanCell = _board.FirstOrDefault(c => c.Type == EntityType.PACMAN);
        if (pacmanCell is not null)
        {
            Pacman.Row = pacmanCell.Row;
            Pacman.Col = pacmanCell.Col;
        }
    }
    
    private void StartGameLoop()
    {
        _gameTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(150)
        };
        _gameTimer.Tick += (s, e) =>
        {
            try
            {
                Console.WriteLine($"[GameTimer] Tick {Ghosts.GetModeTimer()}");
                _pacman.UpdatePacmanSprites();
                Ghosts.GhostsTimer();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameTimer] EXCEPCIÓN: {ex.Message}\n{ex.StackTrace}");
            }
        };
        _gameTimer.Start();
        InitializePacmanPosition();
        StartMovementTimer();
    }
    
    private void StartMovementTimer()
    {
        _movementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _movementTimer.Tick += (s, e) => _pacman.GetMovePacman();
        _movementTimer.Start();
    }
}