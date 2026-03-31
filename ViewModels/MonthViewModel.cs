using System.Collections.ObjectModel;
using System.ComponentModel;
using ReiskostenApp.Models;

namespace ReiskostenApp.ViewModels;

public class MonthViewModel : INotifyPropertyChanged
{
    public ObservableCollection<DayEntry> Days { get; set; }
        = new ObservableCollection<DayEntry>();

    public string Title => DateTime.Now.ToString("MMMM yyyy");

    private int _total;
    public int Total
    {
        get => _total;
        set { _total = value; OnPropertyChanged(nameof(Total)); }
    }

    public MonthViewModel()
    {
        LoadDays();
    }

    private void LoadDays()
    {
        Days.Clear();

        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            Days.Add(new DayEntry
            {
                Date = new DateTime(year, month, day),
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
