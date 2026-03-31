using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReiskostenApp.Data;
using ReiskostenApp.Models;
using ReiskostenApp.ViewModels;
using System.Collections.ObjectModel;

namespace ReiskostenApp.ViewModels;

public partial class MonthViewModel : ObservableObject
{
    //public IRelayCommand OpenSettingsCommand => SharedCommands.OpenSettingsCommand;

    private readonly AppRepository _repo;
    private readonly AppState _state;

    [ObservableProperty]
    private int year;

    [ObservableProperty]
    private int month;

    [ObservableProperty]
    private int daysWorked;

    [ObservableProperty]
    private ObservableCollection<DayEntryViewModel> days = new();

    public IRelayCommand LoadCommand { get; }
    public IRelayCommand SaveCommand { get; }
    public IRelayCommand<DayEntryViewModel> ClearNotesCommand { get; }

    public MonthViewModel(AppRepository repo, AppState state)
    {
        _repo = repo;
        _state = state;

        Year = state.SelectedYear == 0 ? DateTime.Now.Year : state.SelectedYear;
        Month = state.SelectedMonth == 0 ? DateTime.Now.Month : state.SelectedMonth;

        LoadCommand = new RelayCommand(async () => await LoadAsync());
        SaveCommand = new RelayCommand(async () => await SaveAsync());
        ClearNotesCommand = new RelayCommand<DayEntryViewModel>((day) => day.Notes = string.Empty);
    }

    public async Task LoadAsync()
    {
        Days.Clear();

        var items = await _repo.GetMonthEntriesAsync(Year, Month);

        for (int d = 1; d <= DateTime.DaysInMonth(Year, Month); d++)
        {
            var date = new DateTime(Year, Month, d);
            var existing = items.FirstOrDefault(x => x.Date == date.ToString("yyyy-MM-dd"));

            Days.Add(new DayEntryViewModel
            {
                Day = d,
                Count = existing?.Count ?? 0,
                Notes = existing?.Description ?? "",
                Date = date
            });
        }

        DaysWorked = Days.Sum(x => x.Count);

        // 🔥 BELANGRIJK: maand opslaan voor Totalenpagina
        _state.SelectedYear = Year;
        _state.SelectedMonth = Month;
    }

    private async Task SaveAsync()
    {
        foreach (var d in Days)
        {
            var model = new VolunteerDay
            {
                Date = d.Date.ToString("yyyy-MM-dd"),
                Count = d.Count,
                Description = d.Notes
            };

            await _repo.SaveDayAsync(model);
        }

        DaysWorked = Days.Sum(x => x.Count);
    }
}
