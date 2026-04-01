using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private AppSettings _settings = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand SaveCommand { get; }
        public string[] ThemeOptions { get; } = new[] { "System", "Light", "Dark" };

        public SettingsViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            SaveCommand = new Command(async () => await SaveAsync());
        }

        public string SelectedTheme
        {
            get => _settings.SelectedTheme ?? "System";
            set
            {
                if (_settings.SelectedTheme == value) return;
                _settings.SelectedTheme = value;
                OnPropertyChanged(nameof(SelectedTheme));
            }
        }

        public decimal RatePerDay
        {
            get => _settings.RatePerDay;
            set
            {
                if (_settings.RatePerDay == value) return;
                _settings.RatePerDay = value;
                OnPropertyChanged(nameof(RatePerDay));
            }
        }

        public int SelectedMonth
        {
            get => _settings.SelectedMonth;
            set
            {
                if (_settings.SelectedMonth == value) return;
                _settings.SelectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
            }
        }

        public int SelectedYear
        {
            get => _settings.SelectedYear;
            set
            {
                if (_settings.SelectedYear == value) return;
                _settings.SelectedYear = value;
                OnPropertyChanged(nameof(SelectedYear));
            }
        }

        public async Task LoadAsync()
        {
            await _db.InitializeAsync();
            _settings = await _db.GetSettingsAsync() ?? new AppSettings();
            OnPropertyChanged(string.Empty); // refresh all bindings
        }

        public async Task SaveAsync()
        {
            await _db.SaveSettingsAsync(_settings);

            // Apply theme immediately via App if available
            if (Application.Current is App app)
            {
                app.ApplyThemeResources(_settings.SelectedTheme);
            }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
