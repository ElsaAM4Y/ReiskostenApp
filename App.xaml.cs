using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;
using System;

namespace ReiskostenApp
{
    public partial class App : Application
    {
        private readonly DatabaseService _db;

        // DI will inject DatabaseService (registered in MauiProgram)
        public App(DatabaseService db)
        {
            InitializeComponent(); // ensure App.xaml x:Class matches namespace/class
            _db = db ?? throw new ArgumentNullException(nameof(db));

            // Temporary loading page while DB initializes
            MainPage = new NavigationPage(new ContentPage
            {
                Title = "Loading",
                Content = new ActivityIndicator { IsRunning = true, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }
            });
        }

        // Expose repository for legacy code that referenced App.Repository
        public DatabaseService Repository => _db;

        protected override async void OnStart()
        {
            base.OnStart();

            // Initialize DB (creates tables and ensures AppSettings row)
            await _db.InitializeAsync();

            // Load or create settings
            var settings = await _db.GetSettingsAsync() ?? new AppSettings();
            await _db.SaveSettingsAsync(settings);

            // Apply theme resources (System/Light/Dark)
            ApplyThemeResources(settings.SelectedTheme);

            // Set OS-level theme behavior
            if (string.Equals(settings.SelectedTheme, "Light", StringComparison.OrdinalIgnoreCase))
                Microsoft.Maui.Controls.Application.Current.UserAppTheme = OSAppTheme.Light;
            else if (string.Equals(settings.SelectedTheme, "Dark", StringComparison.OrdinalIgnoreCase))
                Microsoft.Maui.Controls.Application.Current.UserAppTheme = OSAppTheme.Dark;
            else
                Microsoft.Maui.Controls.Application.Current.UserAppTheme = OSAppTheme.Unspecified;

            // Resolve start page from DI (fallback to manual construction)
            var monthPage = Microsoft.Maui.Hosting.MauiApplication.Current?.Services?.GetService<ReiskostenApp.Views.MonthPage>()
                            ?? new ReiskostenApp.Views.MonthPage(_db);

            MainPage = new NavigationPage(monthPage)
            {
                BarBackgroundColor = (Color)Current.Resources["PrimaryColor"],
                BarTextColor = (Color)Current.Resources["TextColor"]
            };
        }

        public void ApplyThemeResources(string theme)
        {
            var isDark = string.Equals(theme, "Dark", StringComparison.OrdinalIgnoreCase);

            var primaryKey = isDark ? "DarkPrimary" : "LightPrimary";
            var backgroundKey = isDark ? "DarkBackground" : "LightBackground";
            var textKey = isDark ? "DarkText" : "LightText";
            var entryBgKey = isDark ? "DarkEntryBackground" : "LightEntryBackground";

            if (Current.Resources.ContainsKey(primaryKey)
                && Current.Resources.ContainsKey(backgroundKey)
                && Current.Resources.ContainsKey(textKey)
                && Current.Resources.ContainsKey(entryBgKey))
            {
                Current.Resources["PrimaryColor"] = Current.Resources[primaryKey];
                Current.Resources["BackgroundColor"] = Current.Resources[backgroundKey];
                Current.Resources["TextColor"] = Current.Resources[textKey];
                Current.Resources["EntryBackgroundColor"] = Current.Resources[entryBgKey];
            }
        }
    }
}
