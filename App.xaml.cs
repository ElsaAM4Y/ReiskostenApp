using System;
using Microsoft.Maui.Controls;
using ReiskostenApp.Services;

namespace ReiskostenApp
{
    public partial class App : Application
    {
        private readonly DatabaseService _db;

        public App(DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
            MainPage = new AppShell();
        }

        public DatabaseService Repository => _db;

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                await _db.InitializeAsync();

                var settings = await _db.GetSettingsAsync() ?? new Models.AppSettings();
                await _db.SaveSettingsAsync(settings);

                ApplyThemeResources(settings.SelectedTheme);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App OnStart error: {ex}");
            }
        }

        public void ApplyThemeResources(string theme)
        {
            var isDark = string.Equals(theme, "Dark", StringComparison.OrdinalIgnoreCase);

            var primaryKey = isDark ? "DarkPrimary" : "LightPrimary";
            var backgroundKey = isDark ? "DarkBackground" : "LightBackground";
            var textKey = isDark ? "DarkText" : "LightText";
            var entryBgKey = isDark ? "DarkEntryBackground" : "LightEntryBackground";

            if (Current?.Resources == null) return;

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
