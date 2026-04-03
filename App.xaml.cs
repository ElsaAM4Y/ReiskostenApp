using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Themes = ReiskostenApp.Resources.Styles;

namespace ReiskostenApp
{
    public partial class App : Application
    {
        private readonly Services.DatabaseService _db;

        private readonly IServiceProvider _services;

        public App(Services.DatabaseService db, IServiceProvider services)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _services = services;

            // Load saved theme synchronously before MainPage is set so pages render with correct colors
            var settings = _db.GetSettingsAsync().GetAwaiter().GetResult();
            ApplyThemeResources(settings?.SelectedTheme ?? "Light");

            MainPage = _services.GetRequiredService<AppShell>();
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
            System.Diagnostics.Debug.WriteLine($"[App] ApplyThemeResources called with: '{theme}'");

            ResourceDictionary dict = theme?.Trim() switch
            {
                "Dark" => new Themes.Dark(),
                _      => new Themes.Light()
            };

            System.Diagnostics.Debug.WriteLine($"[App] Dict type: {dict.GetType().Name}, key count: {dict.Count}");

            foreach (var kvp in dict)
                Current.Resources[kvp.Key] = kvp.Value;

            System.Diagnostics.Debug.WriteLine($"[App] Done applying theme keys.");
        }
    }
}
