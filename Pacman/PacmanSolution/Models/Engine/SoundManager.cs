using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Platform;

namespace PacmanSolution.Models;

public class SoundManager
{
    private Process? _currentProcess;
    private string? _tempFilePath;
    private bool _shouldLoop;

    /// <summary>
    /// the method use  the name isLooping which to open
    /// the resource Avalonia's asstes
    /// </summary>
    /// <param name="nameSong"></param>
    /// <param name="isLooping"></param>
    /// then create the file for use the temporal file  and play the sound
    public void PlaySound(string nameSong, bool isLooping = false)
    {
        StopSound();
        _shouldLoop = isLooping;
        try
        {
            var assetUri = new Uri("avares://PacmanSolution/Assets/Media/PacManOriginalThemeTheCantinaBand.wav");
            using var stream = AssetLoader.Open(assetUri);
            _tempFilePath = Path.Combine(Path.GetTempPath(), "pacman_temp_sound.wav");
            using (var fileStream = File.Create(_tempFilePath))
            {
                stream.CopyTo(fileStream);
            }

            PlayWithSystemPlayer();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cargando audio: {ex.Message}");
        }
    }

    /// <summary>
    /// This method is the system to sound where use the path for enable the music
    /// </summary>
    private void PlayWithSystemPlayer()
    {
        if (string.IsNullOrEmpty(_tempFilePath)) return;

        _currentProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "paplay",
                Arguments = _tempFilePath,
                CreateNoWindow = true,
                UseShellExecute = false
            },
            EnableRaisingEvents = true
        };

        _currentProcess.Exited += (s, e) =>
        {
            if (_shouldLoop) PlayWithSystemPlayer();
        };

        _currentProcess.Start();
    }

    /// <summary>
    /// the method to stop the sound when  you want
    /// </summary>
    public void StopSound()
    {
        _shouldLoop = false;
        try
        {
            if (_currentProcess != null && !_currentProcess.HasExited)
            {
                _currentProcess.Kill();
                _currentProcess.Dispose();
            }
        }
        catch
        {
            /* Ignorar errores al cerrar */
        }
    }
}