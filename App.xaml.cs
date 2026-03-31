using ReiskostenApp.Data;
using ReiskostenApp.Models;

namespace ReiskostenApp;

public partial class App : Application
{
    public static AppRepository Repository { get; private set; }
    public static AppState State { get; private set; }

    public App(AppRepository repo, AppState state)
    {
        InitializeComponent();

        Repository = repo;
        State = state;

        MainPage = new AppShell();
    }

}
