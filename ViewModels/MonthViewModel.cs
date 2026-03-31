using System.Collections.ObjectModel;
using System.ComponentModel;
using ReiskostenApp.Data;
using ReiskostenApp.Models;

namespace ReiskostenApp.ViewModels;

public class MonthViewModel : INotifyPropertyChanged
{
    private readonly AppRepository _repo;
    private readonly AppState _state;

    public ObservableCollection<DayEntry> Days { get; set; }
        = new ObservableCollection<DayEntry>();

    public int Year => _state.SelectedYear;
    public int Month => _state.SelectedMonth;

    public string Title => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

    private int _total;
    public int Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(nameof(Total)); }
    }

    public MonthViewModel(AppRepository repo, AppState state)
    {
        _repo = repo;
        _state = state;

        LoadDays();
    }

    private void LoadDays()
    {
        Days.Clear();

        int daysInMonth = DateTime.DaysInMonth(Year, Month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            Days.Add(new DayEntry
            {
                Date = new DateTime(Year, Month, day),
                Value = 0,
                Notes = string.Empty
            });
        }

        RecalculateTotal();
    }

    public void RecalculateTotal()
    {
        Total = Days.Sum(d => d.Value);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
