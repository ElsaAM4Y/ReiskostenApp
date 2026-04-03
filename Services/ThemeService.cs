using Microsoft.Maui.Controls;
using System.Linq;

namespace ReiskostenApp.Services
{
    public static class ThemeService
    {
        public static void SetTheme(string theme)
        {
            // Save selected theme
            Preferences.Set("AppTheme", theme);

            // Create the new theme dictionary
            ResourceDictionary newTheme = theme switch
            {
                "Dark" => new ReiskostenApp.Resources.Styles.Dark(),
                _ => new ReiskostenApp.Resources.Styles.Light()
            };

            var merged = Application.Current.Resources.MergedDictionaries;

            // Find existing theme dictionary
            var existingTheme = merged.FirstOrDefault(d =>
                d is ReiskostenApp.Resources.Styles.Light ||
                d is ReiskostenApp.Resources.Styles.Dark);

            // Replace only the theme dictionary
            if (existingTheme != null)
                merged.Remove(existingTheme);

            merged.Add(newTheme);

            // Update icon resource based on theme
            switch (theme)
            {
                case "Dark":
                    Application.Current.Resources["WebsiteIconSource"] =
                        Application.Current.Resources["WebsiteIconSourceDark"];
                    break;

                default:
                    Application.Current.Resources["WebsiteIconSource"] = "website.svg";
                    break;
            }
        }

        public static void LoadSavedTheme()
        {
            SetTheme(Preferences.Get("AppTheme", "Light"));
        }
    }
}
