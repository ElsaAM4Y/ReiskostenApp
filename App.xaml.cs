using ReiskostenApp.Models;

namespace ReiskostenApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; set; }

    private readonly AppState _state;

    public App(AppState state)
    {
        InitializeComponent();
        _state = state;

        //// Startthema toepassen
        //Theme.ThemeManager.ApplySavedTheme();

        //// OS-thema wijzigingen volgen
        //RequestedThemeChanged += OnRequestedThemeChanged;

        // 🔥 BELANGRIJK: dit voorkomt jouw foutmelding
        MainPage = new AppShell();
    }

    //private void OnRequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
    //{
    //    if (_state.SelectedTheme == "System")
    //    {
    //        ThemeManager.Apply("System");
    //    }
    //}
}
