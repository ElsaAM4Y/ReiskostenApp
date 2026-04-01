using System;
using System.Linq;
using Microsoft.Maui.Controls;
using ReiskostenApp.Services;

namespace ReiskostenApp
{
    public partial class App : Application
    {
        private readonly DatabaseService _db;

        // Paths must match exactly the Source strings used in App.xaml
        private const string LightThemePath = "Resources/Styles.Light.xaml";
        private const string DarkThemePath = "Resources/Styles.Dark.xaml";

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

        /// <summary>
        /// Swap the theme resource dictionary to Light or Dark.
        /// Accepts "Light", "Dark", or "System".
        /// </summary>
        public void ApplyThemeResources(string theme)
        {
            if (Current?.Resources == null) return;

            var requested = string.IsNullOrWhiteSpace(theme) ? "System" : theme;
            if (string.Equals(requested, "System", StringComparison.OrdinalIgnoreCase))
            {
                var app = Application.Current;
                var requestedThemeString = app != null ? app.RequestedTheme.ToString() : "Light";
                requested = string.Equals(requestedThemeString, "Dark", StringComparison.OrdinalIgnoreCase) ? "Dark" : "Light";
            }

            var desiredPath = string.Equals(requested, "Dark", StringComparison.OrdinalIgnoreCase)
                ? "Resources/Styles.Dark.xaml"
                : "Resources/Styles.Light.xaml";

            var merged = Current.Resources.MergedDictionaries;
            if (merged == null) return;

            var existing = merged.FirstOrDefault(d =>
                d?.Source != null &&
                (string.Equals(d.Source.OriginalString, "Resources/Styles.Light.xaml", StringComparison.OrdinalIgnoreCase)
                 || string.Equals(d.Source.OriginalString, "Resources/Styles.Dark.xaml", StringComparison.OrdinalIgnoreCase)));

            if (existing != null && string.Equals(existing.Source?.OriginalString, desiredPath, StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                var newDict = new ResourceDictionary { Source = new Uri(desiredPath, UriKind.Relative) };
                if (existing != null) merged.Remove(existing);
                merged.Add(newDict);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load theme resource '{desiredPath}': {ex}");
            }
        }

    }
}
