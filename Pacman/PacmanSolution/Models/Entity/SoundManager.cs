using System;
using System.Diagnostics;
using System.IO;
using Avalonia.Platform;
using ManagedBass;

namespace PacmanSolution.Models;

public class SoundManager
{
    /*private WaveOutEvent? _outputDevice;
    private WaveFileReader? _audioFile;
    private bool _shouldLoop;

    public void PlaySound(string nameSong, bool isLooping = false)
    {
        StopSound();
        _shouldLoop = isLooping;

        try
        {
            var stream = AssetLoader.Open(
                new Uri()
            );

            // uso Naudio para asi hacer sonar esto me lo robe de su libreria
            _audioFile = new WaveFileReader(stream);
            _outputDevice = new WaveOutEvent();

            _outputDevice.Init(_audioFile);

            //utilizo el evento que tienen para suscribir que se repita si se para siga
            _outputDevice.PlaybackStopped += OnPlaybackStooped;

            _outputDevice.Play();
        }
        catch (Exception ex) 
        { 
            Console.WriteLine(ex.Message);
        }
    }

    private void OnPlaybackStooped(object sender, StoppedEventArgs e)
    {
        //si se acaba el audio se repite
        if (_shouldLoop && _audioFile != null && _outputDevice != null)
        {
            _audioFile.Position = 0;
            _outputDevice.Play();
        }
    }

    public void StopSound()
    {
        //elimina toda cosa que halla quedado del sonido anterior.
        _shouldLoop = false;
        if (_outputDevice != null)
        {
            _outputDevice.Stop();
            _outputDevice.Dispose();
            _outputDevice = null;
        }

        if (_audioFile != null)
        {
            _audioFile.Dispose();
            _audioFile = null;
        }
    }*/
    private Process? _currentProcess;
    private string? _tempFilePath;
    private bool _shouldLoop;

    public void PlaySound(string nameSong, bool isLooping = false)
    {
        StopSound();
        _shouldLoop = isLooping;

        try
        {
            // 1. Abrir el recurso desde los Assets de Avalonia
            var assetUri = new Uri("avares://PacmanSolution/Assets/Media/PacManOriginalThemeTheCantinaBand.wav");
            using var stream = AssetLoader.Open(assetUri);

            // 2. Crear un archivo temporal simple en /tmp
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

    private void PlayWithSystemPlayer()
    {
        if (string.IsNullOrEmpty(_tempFilePath)) return;

        _currentProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                // Intentamos usar 'paplay' (PulseAudio) o 'aplay' (ALSA)
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
        catch { /* Ignorar errores al cerrar */ }
    }
}
//public class SpriteManager
// {
//     
// }