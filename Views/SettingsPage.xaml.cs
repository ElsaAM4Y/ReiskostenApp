using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using ReiskostenApp.Models;
using ReiskostenApp.Services;
using System.Globalization;

namespace ReiskostenApp.Views;

public partial class SettingsPage : ContentPage
{
    private readonly DatabaseService _db;
    private AppSettings _settings;

    public SettingsPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;

        ThemePicker.ItemsSource = new List<string> { "System", "Light", "Dark" };
        MonthPicker.ItemsSource = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12).ToList();
        YearPicker.ItemsSource = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _settings = await _db.GetSettingsAsync() ?? new AppSettings();

        ThemePicker.SelectedItem = _settings.SelectedTheme ?? "System";
        RateEntry.Text = _settings.RatePerDay.ToString(CultureInfo.InvariantCulture);
        MonthPicker.SelectedIndex = Math.Max(0, (_settings.SelectedMonth == 0 ? DateTime.Now.Month : _settings.SelectedMonth) - 1);
        YearPicker.SelectedItem = _settings.SelectedYear == 0 ? DateTime.Now.Year : _settings.SelectedYear;
    }

    private async void OnSaveSettings(object sender, EventArgs e)
    {
        if (_settings == null) _settings = new AppSettings();

        _settings.SelectedTheme = ThemePicker.SelectedItem?.ToString() ?? "System";
        if (decimal.TryParse(RateEntry.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate))
            _settings.RatePerDay = rate;

        if (MonthPicker.SelectedIndex >= 0)
            _settings.SelectedMonth = MonthPicker.SelectedIndex + 1;
        if (YearPicker.SelectedItem is int y)
            _settings.SelectedYear = y;

        await _db.SaveSettingsAsync(_settings);

        // Apply theme resources immediately via App
        if (Application.Current is App app)
        {
            app.ApplyThemeResources(_settings.SelectedTheme);
            if (_settings.SelectedTheme == "Light")
                Application.Current.UserAppTheme = OSAppTheme.Light;
            else if (_settings.SelectedTheme == "Dark")
                Application.Current.UserAppTheme = OSAppTheme.Dark;
            else
                Application.Current.UserAppTheme = OSAppTheme.Unspecified;
        }

        await DisplayAlert("Saved", "Settings saved.", "OK");
    }
}
