using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PacmanSolution.ViewModels;

public partial class MainWindowViewModel:INotifyPropertyChanged
{
    private object _currentPage;

    public object CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage != value)
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
    }

    public MainWindowViewModel()
    {
        CurrentPage = new MenuPageViewModel(this);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}