using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
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
        private string _selectedTheme = "System";
        private decimal _ratePerDay;
        private int _selectedMonth;
        private int _selectedYear;
        private bool _isBusy;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> ThemeOptions { get; } = new ObservableCollection<string> { "System", "Light", "Dark" };
        public ObservableCollection<string> Months { get; } = new ObservableCollection<string>();
        public ObservableCollection<int> Years { get; } = new ObservableCollection<int>();

        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }

        public SettingsViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));

            // Populate months and years
            foreach (var m in CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12))
                Months.Add(m);

            var start = DateTime.Now.Year - 5;
            for (int y = start; y <= start + 10; y++)
                Years.Add(y);

            LoadCommand = new Command(async () => await LoadAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            ResetCommand = new Command(async () => await ResetAsync());
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set { if (_isBusy == value) return; _isBusy = value; OnPropertyChanged(); }
        }

        public string SelectedTheme
        {
            get => _selectedTheme;
            set { if (_selectedTheme == value) return; _selectedTheme = value; OnPropertyChanged(); }
        }

        public decimal RatePerDay
        {
            get => _ratePerDay;
            set { if (_ratePerDay == value) return; _ratePerDay = value; OnPropertyChanged(); }
        }

        public int SelectedMonth
        {
            get => _selectedMonth;
            set { if (_selectedMonth == value) return; _selectedMonth = value; OnPropertyChanged(); }
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set { if (_selectedYear == value) return; _selectedYear = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Load settings from database into the view model.
        /// Safe to call multiple times.
        /// </summary>
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await _db.InitializeAsync(); // idempotent
                _settings = await _db.GetSettingsAsync() ?? new AppSettings();

                SelectedTheme = string.IsNullOrWhiteSpace(_settings.SelectedTheme) ? "System" : _settings.SelectedTheme;
                RatePerDay = _settings.RatePerDay;
                SelectedMonth = _settings.SelectedMonth == 0 ? DateTime.Now.Month : _settings.SelectedMonth;
                SelectedYear = _settings.SelectedYear == 0 ? DateTime.Now.Year : _settings.SelectedYear;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Persist current settings to the database and apply theme resources.
        /// Does not change OS-level theme; it calls App.ApplyThemeResources so DynamicResource updates immediately.
        /// </summary>
        public async Task SaveAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;

                _settings.SelectedTheme = SelectedTheme ?? "System";
                _settings.RatePerDay = RatePerDay;
                _settings.SelectedMonth = SelectedMonth;
                _settings.SelectedYear = SelectedYear;

                await _db.SaveSettingsAsync(_settings);

                // Apply theme resources immediately (App.ApplyThemeResources must exist in App.xaml.cs)
                if (Application.Current is App app)
                {
                    app.ApplyThemeResources(_settings.SelectedTheme);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Reset settings to defaults and persist.
        /// </summary>
        public async Task ResetAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                _settings = new AppSettings
                {
                    SelectedTheme = "System",
                    RatePerDay = 0m,
                    SelectedMonth = DateTime.Now.Month,
                    SelectedYear = DateTime.Now.Year
                };

                SelectedTheme = _settings.SelectedTheme;
                RatePerDay = _settings.RatePerDay;
                SelectedMonth = _settings.SelectedMonth;
                SelectedYear = _settings.SelectedYear;

                await _db.SaveSettingsAsync(_settings);

                if (Application.Current is App app)
                {
                    app.ApplyThemeResources(_settings.SelectedTheme);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
