using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PacmanSolution.ViewModels;

public partial class ScoreBoardPageViewModel:ObservableObject
{
    public event PropertyChangedEventHandler? PropertyChanged;
    [ObservableProperty]
    private int _score;
    [ObservableProperty]
    private int _highScore;
    private readonly int _totalScore=2590;
    private int _totalScoreCherry=1500;
    private int _scoreCherry=0;

    public ScoreBoardPageViewModel()
    {
        Score = 0;
        HighScore = 0;
    }

    public void amount(int result)
    {
        Score += result;
        if (Score > HighScore)
        {
            HighScore = Score;
            Console.WriteLine($"{result} puntos! Score: {Score}");
        }

        if (HighScore > _totalScore)
        {
            ///todas las pilldoras comidas
            Console.WriteLine("Winner");
        }
        
    }
    //[RelayCommand]
    private void AddPoints(string cellType, int points)
    {
        if (cellType is "Cherry")
        {
            Score += 100;
        }
        else if (cellType is "pellet" || cellType is "energizer")
        {
            Score = _score;
        }
    }
}