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
        subscribeEvents();
        InitializePacmanPosition();
        StartMovementTimer();
    }

    private void subscribeEvents()
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
        _engine.OnEnergizerEaten += () =>
        {
            Ghosts.SetFrightened();
            Ghosts.StartFrightenedMode();
        };
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
            //poner en letras rojas y en el panel arriba de  mmi panel de ahora un mensaje de murio
        };

        Pacman.OnDeathAnimationFinished += () =>
        {
            _countLivePacman--;
            _gameTimer.Start();
            _movementTimer?.Start();
        };
        _engine.InitPelletCount(Board);
        _engine.LevelComplete += () =>
        {
            _gameTimer.Stop();
            _movementTimer?.Stop();
            Console.WriteLine("¡Nivel completado!");
            // aquí navegas a pantalla de victoria
            Navigation.ChangePage("WinScreen");
        };
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