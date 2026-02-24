using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PacmanSolution.Models.Entities;

namespace PacmanSolution.Models.Engine;

public abstract class ScoreService
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PacmanSolution",
        "scores.json"
    );
    /// <summary>
    /// Loads the list of scores from the JSON file
    /// Returns an empty list if the file does not exist or an error occurs
    /// </summary>
    /// <returns></returns>
    public static List<Score> LoadScores()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new List<Score>();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Score>>(json) ?? new List<Score>();
        }
        catch
        {
            return new List<Score>();
        }
    }

    /// <summary>
    /// Saves the provided list of scores to the JSON file
    /// Creates the directory if it does not exist
    /// Logs an error message to the console if saving fails
    /// </summary>
    /// <param name="scores"></param>
    public static void SaveScore(List<Score> scores)
    {
        try
        {
            string? dir = Path.GetDirectoryName(FilePath);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(scores, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving scores: {ex.Message}");
        }
    }
}