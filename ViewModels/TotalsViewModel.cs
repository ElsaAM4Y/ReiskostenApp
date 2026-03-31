using System.ComponentModel;
using ReiskostenApp.Data;

namespace ReiskostenApp.ViewModels;

public class TotalsViewModel : INotifyPropertyChanged
{
    private readonly AppRepository _repo;

    private int _year = DateTime.Now.Year;
    public int Year
    {
        get => _year;
        set
        {
            if (_year != value)
            {
                _year = value;
                OnPropertyChanged(nameof(Year));
                LoadTotal();
            }
        }
    }

    private int _total;
    public int Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(nameof(Total)); }
    }

    public TotalsViewModel()
    {
        _repo = App.Repository;
        LoadTotal();
    }

    private async void LoadTotal()
    {
        Total = await _repo.GetTotalCountAsync(Year);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
