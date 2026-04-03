using ReiskostenApp.Models;
using ReiskostenApp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ReiskostenApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private AppSettings _settings = new();

        public List<string> Themes { get; } = new() { "Common", "Light", "Dark" };
        public List<string> MonthNames { get; } = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12).ToList();
        public List<int> Years { get; } = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList();

        public string SelectedTheme
        {
            get => _settings.SelectedTheme;
            set { _settings.SelectedTheme = value; OnPropertyChanged(); }
        }

        public decimal RatePerDay
        {
            get => _settings.RatePerDay;
            set { _settings.RatePerDay = value; OnPropertyChanged(); }
        }

        public int SelectedMonthIndex
        {
            get => Math.Clamp((_settings.SelectedMonth == 0 ? DateTime.Now.Month : _settings.SelectedMonth) - 1, 0, 11);
            set { _settings.SelectedMonth = value + 1; OnPropertyChanged(); }
        }

        public int SelectedYear
        {
            get => _settings.SelectedYear == 0 ? DateTime.Now.Year : _settings.SelectedYear;
            set { _settings.SelectedYear = value; OnPropertyChanged(); }
        }

        public SettingsViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _db.InitializeAsync();
                _settings = await _db.GetSettingsAsync() ?? new AppSettings();
                OnPropertyChanged(nameof(SelectedTheme));
                OnPropertyChanged(nameof(RatePerDay));
                OnPropertyChanged(nameof(SelectedMonthIndex));
                OnPropertyChanged(nameof(SelectedYear));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SettingsViewModel.InitializeAsync error: {ex}");
            }
        }

        public async Task SaveAsync()
        {
            await _db.SaveSettingsAsync(_settings);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
