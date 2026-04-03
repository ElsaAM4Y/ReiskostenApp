using ReiskostenApp.Services;

namespace ReiskostenApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Load saved theme BEFORE any UI is created
        ThemeService.LoadSavedTheme();

        MainPage = new AppShell();
    }
}
