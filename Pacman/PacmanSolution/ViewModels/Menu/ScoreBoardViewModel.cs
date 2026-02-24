using System;
using System.Collections.ObjectModel;

using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PacmanSolution.Models;
using PacmanSolution.Models.Engine;
using PacmanSolution.Models.Entities;

namespace PacmanSolution.ViewModels;

public partial class ScoreBoardViewModel:ObservableObject
{
    [ObservableProperty]
    private int _score;
    [ObservableProperty]
    private int _highScore;
    [ObservableProperty]
    private string _playerName = string.Empty;
    [ObservableProperty]
    private string _saveMessage = string.Empty;
    [ObservableProperty]
    private  ManagePageChange _navigation;
    [ObservableProperty]
    private bool _isSavePanelVisible = false;
    [ObservableProperty] 
    private bool _isSaveSuccess = false;
    private readonly int _totalScore=2810;
    private int _totalScoreCherry=1500;
    private int _scoreCherry=0;
    public ObservableCollection<Score> HighScores { get; } = new();

    public ScoreBoardViewModel(ManagePageChange navigation)
    {
        _navigation = navigation;
        Score = 0;
        HighScore = 0;
        LoadHighScores();
    }

    /// <summary>
    /// The logic is while Score > HighScore  amount points
    /// </summary>
    /// <param name="result"></param>
    public void Amount(int result)
    {
        Score += result;
        if (Score > HighScore)
        {
            HighScore = Score;
            Console.WriteLine($"{result} Score: {Score}");
        }
    }
    private void LoadHighScores()
    {
        var top5 = ScoreService.LoadScores()
            .OrderByDescending(s => s.Points)
            .ToList();

        HighScores.Clear();
        foreach (var s in top5)
            HighScores.Add(s);
    }
    [RelayCommand]
    private void ShowSavePanel()
    {
        PlayerName = string.Empty;
        SaveMessage = string.Empty;
        IsSavePanelVisible = true;
    }

    [RelayCommand]
    private void CancelSave()
    {
        IsSavePanelVisible = false;
        SaveMessage = string.Empty;
    }

    [RelayCommand]
    private void SaveData()
    {
        if (string.IsNullOrWhiteSpace(PlayerName))
        {
            SaveMessage = "Please enter 3 initials";
            IsSaveSuccess = false;
            return;
        }

        string initials = PlayerName.ToUpper().Trim();

        if (initials.Length != 3)
        {
            SaveMessage = "Must be exactly 3 letters";
            IsSaveSuccess = false;
            return;
        }

        if (!initials.All(char.IsLetter))
        {
            SaveMessage = "Only letters allowed";
            IsSaveSuccess = false;
            return;
        }

        var newScore = new Score
        {
            Name = initials,
            Points = Score
        };
        var scores = ScoreService.LoadScores();
        scores.Add(newScore);
        ScoreService.SaveScore(scores);
        LoadHighScores();
        IsSaveSuccess = true;
        SaveMessage = $"{initials} saved with {Score} pts";
    }

    [RelayCommand]
    private void Navigate(string target)
    {
        Navigation.ChangePage(target);
    }
}