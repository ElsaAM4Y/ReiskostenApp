using Microsoft.Maui.ApplicationModel; // for MainThread
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;
using System;
using System.Globalization;
using System.Linq;

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

            // Use exact strings the app expects
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
            // Normalize selected theme string
            var selected = ThemePicker.SelectedItem?.ToString()?.Trim() ?? "System";
            if (string.Equals(selected, "common", StringComparison.OrdinalIgnoreCase)) selected = "System";
            else if (string.Equals(selected, "dark", StringComparison.OrdinalIgnoreCase)) selected = "Dark";
            else if (string.Equals(selected, "light", StringComparison.OrdinalIgnoreCase)) selected = "Light";
            else selected = "System";

            _settings.SelectedTheme = selected;

            if (decimal.TryParse(RateEntry.Text, out var rate)) _settings.RatePerDay = rate;
            _settings.SelectedMonth = MonthPicker.SelectedIndex >= 0 ? MonthPicker.SelectedIndex + 1 : DateTime.Now.Month;
            _settings.SelectedYear = YearPicker.SelectedItem is int y ? y : DateTime.Now.Year;

            await _db.SaveSettingsAsync(_settings);

            // Apply theme on UI thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current is App app)
                    app.ApplyThemeResources(selected);
            });

            await DisplayAlert("Saved", $"Settings saved. Theme: {selected}", "OK");
        }
    }
}
