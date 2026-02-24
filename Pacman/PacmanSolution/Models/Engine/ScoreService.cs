using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace PacmanSolution.Models.Engine;

public class ScoreService
{
    private static readonly string FilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "PacmanSolution",
        "scores.json"
    );
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