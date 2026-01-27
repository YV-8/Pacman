using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PacmanSolution.ViewModels;

public class MainPageViewM: INotifyPropertyChanged
{
    private readonly MainWindowViewModel _mainWindowVM;
    public event PropertyChangedEventHandler PropertyChanged;

    public MainPageViewM(MainWindowViewModel mainWVM)
    {
        _mainWindowVM = mainWVM;
    }
    public void GoGame()
    {
        _mainWindowVM.CurrentPage = new GamePageViewModel();
    }

    public void GoMainMenu()
    {
        _mainWindowVM.CurrentPage = new MenuPageViewModel();
    }
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}