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
        TotalCount = await _repo.GetTotalCountAsync(Year, Month);

        var meta = await _repo.GetOrCreateMonthMetaAsync(Year, Month);

        // Standaardvergoeding instellen als er nog niets is ingevuld
        if (meta.RatePerDay == 0)
            meta.RatePerDay = 1.23m;

        RatePerDay = meta.RatePerDay;
        Submitted = meta.Submitted;

        OnPropertyChanged(nameof(TotalAmount));
    }


    private async Task SaveAsync()
    {
        var meta = new MonthMeta
        {
            Year = Year,
            Month = Month,
            RatePerDay = RatePerDay,
            Submitted = Submitted
        };

        await _repo.SaveMonthMetaAsync(meta);
    }
}
