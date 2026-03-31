// App.xaml.cs
using System;
using Microsoft.Maui.Controls;
using ReiskostenApp.Services;

namespace ReiskostenApp
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly DatabaseService _db;

        public App(DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));

            // Temporary loading page while DB initializes
            MainPage = new NavigationPage(new ContentPage
            {
                Title = "Loading",
                Content = new ActivityIndicator { IsRunning = true, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center }
            });
        }

        // Expose repository for legacy code that expects App.Repository
        public DatabaseService Repository => _db;

        protected override async void OnStart()
        {
            base.OnStart();

            await _db.InitializeAsync();

            var settings = await _db.GetSettingsAsync() ?? new Models.AppSettings();
            await _db.SaveSettingsAsync(settings);

            // Apply theme resources (keeps System/Light/Dark behavior optional)
            ApplyThemeResources(settings.SelectedTheme);

            // Set start page via DI if available, otherwise construct manually
            var monthPage = Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services?.GetService<Views.MonthPage>()
                            ?? new Views.MonthPage(_db);

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
