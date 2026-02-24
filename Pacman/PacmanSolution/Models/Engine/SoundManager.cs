using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform;

namespace PacmanSolution.Models;

public class SoundManager
{
    private string? _tempFilePath;
    private bool _shouldLoop;
    private static SoundManager? _instance;
    private Process? _currentProcess;

    public SoundManager()
    {
        
    }
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
        Task.Run(() =>
        {
            try
            {
                var assetUri = new Uri($"avares://PacmanSolution/Assets/Media/{nameSong}.wav");
                using var stream = AssetLoader.Open(assetUri);
                _tempFilePath = Path.Combine(Path.GetTempPath(), "pacman_current_audio.wav");
                using (var fileStream = new FileStream(_tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyTo(fileStream);
                    fileStream.Flush();
                }

                if (File.Exists(_tempFilePath))
                {
                    PlayWithSystemPlayer();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error load audio {nameSong}: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// This method is the system to sound where use the path for enable the music
    /// </summary>
    private void PlayWithSystemPlayer()
    {
        if (string.IsNullOrEmpty(_tempFilePath)) return;
        try
        {
            _currentProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "paplay",
                    Arguments = $"--client-name=PacmanGame \"{_tempFilePath}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };
        
            _currentProcess.Exited += (s, e) =>
            {
                if (_shouldLoop) 
                {
                    System.Threading.Thread.Sleep(100); 
                    PlayWithSystemPlayer();
                }
            };

            _currentProcess.Start();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error playing audio {_tempFilePath}: {ex.Message}");
        }
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