using System;
using Microsoft.Maui.Controls;
using ReiskostenApp.ViewModels;
using ReiskostenApp.Services;

namespace ReiskostenApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _vm;

        public SettingsPage(SettingsViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var savedTheme = Preferences.Get("AppTheme", "Light");
            ThemePicker.SelectedItem = savedTheme;
        }

        void ApplyPickerTheme()
        {
            if (Application.Current?.Resources.TryGetValue("EntryBackgroundColor", out var val) == true && val is Color color)
            {
                ThemePicker.BackgroundColor = color;
                MonthPicker.BackgroundColor = color;
                YearPicker.BackgroundColor = color;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (ThemePicker.SelectedItem is string selectedTheme)
            {
                ThemeService.SetTheme(selectedTheme);
            }
            ApplyPickerTheme();
            await DisplayAlert("Saved", $"Settings saved. Theme: {_vm.SelectedTheme}", "OK");
        }
    }
}
