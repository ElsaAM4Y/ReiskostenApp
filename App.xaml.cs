using System;
using System.Linq;
using Microsoft.Maui.Controls;

namespace ReiskostenApp
{
    public partial class App : Application
    {
        private readonly Services.DatabaseService _db;

        // Paths must match exactly the Source strings used in App.xaml
        private const string LightThemePath = "Resources/Styles/Light.xaml";
        private const string DarkThemePath = "Resources/Styles/Dark.xaml";

        public App(Services.DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
            MainPage = new AppShell();
        }

        public Services.DatabaseService Repository => _db;

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
        /// If "Common" is used in your UI as the neutral option, treat it the same as removing theme overrides.
        /// </summary>
        public void ApplyThemeResources(string theme)
        {
            if (Current?.Resources == null) return;

            var requested = string.IsNullOrWhiteSpace(theme) ? "System" : theme.Trim();

            // Accept "Common" as neutral (no theme override)
            if (string.Equals(requested, "Common", StringComparison.OrdinalIgnoreCase))
            {
                // Remove any existing theme dictionaries and leave only shared styles
                Application.Current?.Dispatcher?.Dispatch(() =>
                {
                    var merged = Current.Resources.MergedDictionaries;
                    if (merged == null) return;

                    var existing = merged.FirstOrDefault(d =>
                        d?.Source != null &&
                        (string.Equals(d.Source.OriginalString, LightThemePath, StringComparison.OrdinalIgnoreCase)
                         || string.Equals(d.Source.OriginalString, DarkThemePath, StringComparison.OrdinalIgnoreCase)));

                    if (existing != null) merged.Remove(existing);
                });

                return;
            }

            if (string.Equals(requested, "System", StringComparison.OrdinalIgnoreCase))
            {
                var app = Application.Current;
                var requestedThemeString = app != null ? app.RequestedTheme.ToString() : "Light";
                requested = string.Equals(requestedThemeString, "Dark", StringComparison.OrdinalIgnoreCase) ? "Dark" : "Light";
            }

            var desiredPath = string.Equals(requested, "Dark", StringComparison.OrdinalIgnoreCase)
                ? DarkThemePath
                : LightThemePath;

            // Use the app dispatcher to ensure UI-thread execution and correct window context
            Application.Current?.Dispatcher?.Dispatch(() =>
            {
                var merged = Current.Resources.MergedDictionaries;
                if (merged == null) return;

                // Find any existing theme dictionary (Light or Dark)
                var existing = merged.FirstOrDefault(d =>
                    d?.Source != null &&
                    (string.Equals(d.Source.OriginalString, LightThemePath, StringComparison.OrdinalIgnoreCase)
                     || string.Equals(d.Source.OriginalString, DarkThemePath, StringComparison.OrdinalIgnoreCase)));

                // If desired already loaded, nothing to do
                if (existing != null && string.Equals(existing.Source?.OriginalString, desiredPath, StringComparison.OrdinalIgnoreCase))
                    return;

                try
                {
                    var newDict = new ResourceDictionary { Source = new Uri(desiredPath, UriKind.Relative) };

                    if (existing != null) merged.Remove(existing);

                    // Add new theme dictionary after shared Styles.xaml (assumes shared Styles.xaml is already present)
                    merged.Add(newDict);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load theme resource '{desiredPath}': {ex}");
                }
            });
        }
    }
}
