using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly DatabaseService _db;
        private AppSettings _settings = new();

        public SettingsPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
            LoadPickers();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _db.InitializeAsync();
            _settings = await _db.GetSettingsAsync() ?? new AppSettings();

            ThemePicker.ItemsSource = new[] { "System", "Light", "Dark" };
            ThemePicker.SelectedItem = string.IsNullOrWhiteSpace(_settings.SelectedTheme) ? "System" : _settings.SelectedTheme;
            RateEntry.Text = _settings.RatePerDay.ToString(CultureInfo.InvariantCulture);
            MonthPicker.SelectedIndex = Math.Clamp((_settings.SelectedMonth == 0 ? DateTime.Now.Month : _settings.SelectedMonth) - 1, 0, 11);
            YearPicker.SelectedItem = _settings.SelectedYear == 0 ? DateTime.Now.Year : _settings.SelectedYear;
        }

        void LoadPickers()
        {
            MonthPicker.ItemsSource = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12).ToList();
            var years = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList();
            YearPicker.ItemsSource = years;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _settings.SelectedTheme = ThemePicker.SelectedItem?.ToString() ?? "System";
            if (decimal.TryParse(RateEntry.Text, out var rate)) _settings.RatePerDay = rate;
            _settings.SelectedMonth = MonthPicker.SelectedIndex >= 0 ? MonthPicker.SelectedIndex + 1 : DateTime.Now.Month;
            _settings.SelectedYear = YearPicker.SelectedItem is int y ? y : DateTime.Now.Year;

            await _db.SaveSettingsAsync(_settings);

            // Apply theme using the App method so resource dictionaries are swapped
            if (Application.Current is App app)
            {
                app.ApplyThemeResources(_settings.SelectedTheme);
            }

            await DisplayAlert("Saved", "Settings saved.", "OK");
        }
    }
}
