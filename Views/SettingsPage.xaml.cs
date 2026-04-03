using System;
using Microsoft.Maui.Controls;
using ReiskostenApp.ViewModels;

namespace ReiskostenApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _vm;

        public SettingsPage(SettingsViewModel vm)
        {
            InitializeComponent();
            _vm = vm ?? throw new ArgumentNullException(nameof(vm));
            BindingContext = _vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.InitializeAsync();
            ApplyPickerTheme();
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
            System.Diagnostics.Debug.WriteLine($"[Settings] Save clicked. SelectedTheme={_vm.SelectedTheme}");
            try
            {
                await _vm.SaveAsync();
                System.Diagnostics.Debug.WriteLine($"[Settings] SaveAsync done.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Settings] SaveAsync failed: {ex}");
            }

            System.Diagnostics.Debug.WriteLine($"[Settings] Calling ApplyThemeResources with: {_vm.SelectedTheme}");
            if (Application.Current is App app)
                app.ApplyThemeResources(_vm.SelectedTheme);
            else
                System.Diagnostics.Debug.WriteLine("[Settings] Application.Current is NOT App!");

            await DisplayAlert("Saved", $"Settings saved. Theme: {_vm.SelectedTheme}", "OK");
        }
    }
}
