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

            // Initialize DB and load saved theme before MainPage is set
            _db.InitializeAsync().GetAwaiter().GetResult();
            var settings = _db.GetSettingsAsync().GetAwaiter().GetResult();
            if (settings == null)
            {
                settings = new Models.AppSettings();
                _db.SaveSettingsAsync(settings).GetAwaiter().GetResult();
            }
            ApplyThemeResources(settings.SelectedTheme ?? "Light");

            MainPage = _services.GetRequiredService<AppShell>();

            // Apply flyout theme after MainPage is set
            //if (MainPage is AppShell shell &&
            //    Current.Resources.TryGetValue("TabBarBackgroundColor", out var bg) && bg is Color bgColor &&
            //    Current.Resources.TryGetValue("TabBarTitleColor", out var tc) && tc is Color titleColor &&
            //    Current.Resources.TryGetValue("TabBarUnselectedColor", out var uc) && uc is Color unselectedColor)
            //    shell.ApplyFlyoutTheme(bgColor, titleColor, unselectedColor);
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

            //System.Diagnostics.Debug.WriteLine($"[App] Dict type: {dict.GetType().Name}, key count: {dict.Count}");

            //foreach (var kvp in dict)
            //    Current.Resources[kvp.Key] = kvp.Value;

            //if (MainPage is Shell shell &&
            //    Current.Resources.TryGetValue("TabBarBackgroundColor", out var bg) && bg is Color bgColor &&
            //  Current.Resources.TryGetValue("PrimaryTextColor", out var tc) && tc is Color titleColor) ;
            //    Current.Resources.TryGetValue("TabBarUnselectedColor", out var uc) && uc is Color unselectedColor)
            //{
            //    if (shell is AppShell appShell)
            //        appShell.ApplyFlyoutTheme(bgColor, titleColor, unselectedColor);
            //}

            //System.Diagnostics.Debug.WriteLine($"[App] Done applying theme keys.");
        }
    }
}
