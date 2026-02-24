using System;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;
using PacmanSolution.Models.Engine;

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
                Console.WriteLine($"[GameTimer] EXCEPTION: {ex.Message}\n{ex.StackTrace}");
            }
        };
        _gameTimer.Start();
        _engine.InitPelletCount(Board);
        _engine.OnEnergizerEaten += () =>
        {
            Ghosts.SetFrightened();
            Ghosts.StartFrightenedMode();
        };
        _engine.GhostEaten += points => Score.Score += points;
        _engine.LevelComplete += () =>
        {
            _gameTimer.Stop();
            _movementTimer?.Stop();
            PauseAllTimers();
            ShowWinOverlay = true;
        };
        _engine.PacmanDied += () =>
        {
            PauseAllTimers();
            Pacman.DeathAnimation();
            //poner en letras rojas y en el panel
            //arriba de  mmi panel de ahora un mensaje de murio
        };

        Pacman.OnDeathAnimationFinished += () =>
        {
            _countLivePacman--;
            if (_countLivePacman <= 0)
            {
                ShowGameOverOverlay = true;
                return;
            }

            ResumeAllTimers();
        };
        Pacman.OnPacmanMoved += (row, col) =>
        {
            foreach (var ghost in Ghosts.Ghosts)
            {
                _engine.CollisionsToPacman(ghost, Pacman.PacmanModel, Board);
            }
        };
    }

    private void StartMovementTimer()
    {
        _movementTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300)
        };
        _movementTimer.Tick += (s, e) => _pacman.GetMovePacman();
        _movementTimer.Start();
    }

    /// <summary>
    /// pause everything all the timers
    /// </summary>
    private void PauseAllTimers()
    {
        _gameTimer?.Stop();
        _gameLoopTimer?.Stop();
        _movementTimer?.Stop();
        Ghosts.PauseFrightenedTimer();
    }

    /// <summary>
    /// Resuem all timers in the game
    /// </summary>
    private void ResumeAllTimers()
    {
        _gameTimer?.Start();
        _gameLoopTimer?.Start();
        _movementTimer?.Start();
        Ghosts.ResumeFrightenedTimer();
    }

    [RelayCommand]
    private void RestartGame()
    {
        ShowWinOverlay = false;
        ShowGameOverOverlay = false;
        Navigation.ChangePage("GamePage");
    }
    
    [RelayCommand]
    private void ShowSaveInput()
    {
        PlayerName    = string.Empty;
        SaveMessage   = string.Empty;
        IsSaveInputVisible = true;
    }
    
    [RelayCommand]
    private void CancelSave()
    {
        IsSaveInputVisible = false;
        SaveMessage        = string.Empty;
    }

    [RelayCommand]
    private void SaveScore()
    {
        // Validación
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            SaveMessage = "Ingresa 3 iniciales";
            return;
        }

        string initials = PlayerName.ToUpper().Trim();

        if (initials.Length != 3)
        {
            SaveMessage = "Deben ser exactamente 3 letras";
            return;
        }

        if (!initials.All(char.IsLetter))
        {
            SaveMessage = "Solo se permiten letras";
            return;
        }

        // Toma la puntuación actual del ScoreBoardViewModel
        // Score.Score es el int que ya tienes enlazado en el AXAML
        var newScore = new Score
        {
            Name   = initials,
            Points = Score.Score   // <-- usa la propiedad que ya tienes
        };

        var scores = ScoreService.LoadScores();
        scores.Add(newScore);
        ScoreService.SaveScore(scores);

        // Ocultar panel y limpiar
        IsSaveInputVisible = false;
        SaveMessage        = string.Empty;
    }
}