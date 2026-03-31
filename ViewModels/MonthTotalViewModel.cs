using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReiskostenApp.Data;
using ReiskostenApp.Models;

namespace ReiskostenApp.ViewModels;

public partial class MonthTotalViewModel : ObservableObject
{
    //public IRelayCommand OpenSettingsCommand => SharedCommands.OpenSettingsCommand;

    private readonly AppRepository _repo;
    private readonly AppState _state;

    [ObservableProperty]
    private int year;

    [ObservableProperty]
    private int month;

    [ObservableProperty]
    private int totalCount;

    [ObservableProperty]
    private decimal ratePerDay;

    [ObservableProperty]
    private bool submitted;

    public string MonthLabel => $"{Month:00}-{Year}";
    public decimal TotalAmount => TotalCount * RatePerDay;

    public IRelayCommand SaveCommand { get; }

    public MonthTotalViewModel(AppRepository repo, AppState state)
    {
        _repo = repo;
        _state = state;

        // 🔥 Haal maand op die in MonthViewModel gekozen is
        Year = state.SelectedYear;
        Month = state.SelectedMonth;

        SaveCommand = new RelayCommand(async () => await SaveAsync());
    }

    public async Task LoadAsync()
    {
        // Get all records for the selected year and month
        var records = await _repo.GetMonthAsync(Year, Month);

        // Calculate the total for the month
        TotalCount = records.Sum(r => r.Value);

        var meta = await _repo.GetOrCreateMonthMetaAsync(Year, Month);

        // Set default rate if not set
        if (meta.RatePerDay == 0)
            meta.RatePerDay = 1.23m;

        RatePerDay = meta.RatePerDay;
        Submitted = meta.Submitted;

        OnPropertyChanged(nameof(TotalAmount));
    }


    private async Task SaveAsync()
    {
        var meta = new MonthMetaRecord
        {
            Year = Year,
            Month = Month,
            RatePerDay = RatePerDay,
            Submitted = Submitted
        };

        await _repo.SaveMonthMetaAsync(meta);
    }

    // Example: Dictionary with month as key and total as value
    public Dictionary<int, decimal> MonthlyTotals { get; set; }

    public MonthTotalViewModel(IEnumerable<DayRecord> records)
    {
        // Group expenses by month and sum the totals
        MonthlyTotals = records
            .GroupBy(r => new DateTime(r.Year, r.Month, r.Day).Month)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(r => r.Amount)
            );
    }

    // Optionally, get total for a specific month
    public decimal GetTotalForMonth(int month)
    {
        return MonthlyTotals.TryGetValue(month, out var total) ? total : 0m;
    }
}


