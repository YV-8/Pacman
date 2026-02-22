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
        var pacmanCell = Board.FirstOrDefault(c => c.Type == EntityType.PACMAN);
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
        Pacman.OnEnergizerEaten += () => Ghosts.SetFrightened();// suscribe por para el energizantes
        Pacman.OnPacmanMoved += (row, col) =>
        {
            foreach (var ghost in Ghosts.Ghosts)
                _engine.CollisionsToPacman(ghost, Pacman.PacmanModel, Ghosts.GetModeTimer());
        };
        _engine.PacmanDied += () =>
        {
            _gameTimer.Stop();
            _movementTimer?.Stop();
            Pacman.DeathAnimation();
        };

        Pacman.OnDeathAnimationFinished += () =>
        {
            _countLivePacman--;
            _gameTimer.Start();
            _movementTimer?.Start();
        };
        
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
    private void CheckWinCondition()
    {
        if (!Board.Any(e => e.IsActive))
        {
            Console.WriteLine("¡Nivel Completado!");
            // Aquí disparas la lógica de siguiente nivel o victoria
        }
    }
}