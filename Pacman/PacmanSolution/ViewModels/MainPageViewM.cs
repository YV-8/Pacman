using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PacmanSolution.ViewModels;

public class MainPage: INotifyPropertyChanged
{
    private readonly MainWindowViewModel _mainWindowVM;
    public event PropertyChangedEventHandler PropertyChanged;

    public MainPage(MainWindowViewModel mainWVM)
    {
        _mainWindowVM = mainWVM;
    }
    public void GoGame()
    {
        _mainWindowVM.CurrentPage = new PagGameViewModel();
    }
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


}