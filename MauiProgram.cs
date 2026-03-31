using CommunityToolkit.Maui;
using ReiskostenApp.Data;
using ReiskostenApp.Models;
using ReiskostenApp.ViewModels;
using ReiskostenApp.Views;

namespace ReiskostenApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // -------------------------
        //  Database pad
        // -------------------------
        string dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "reiskosten.db3"
        );

        // -------------------------
        //  Singletons
        // -------------------------
        builder.Services.AddSingleton(new AppRepository(dbPath));
        builder.Services.AddSingleton<AppState>();

        // -------------------------
        //  ViewModels
        // -------------------------
        builder.Services.AddTransient<MonthViewModel>();
        builder.Services.AddTransient<MonthTotalViewModel>();
        builder.Services.AddTransient<NotesViewModel>();
        //builder.Services.AddTransient<SettingsViewModel>();

        // -------------------------
        //  Pages
        // -------------------------
        builder.Services.AddTransient<MonthPage>();
        builder.Services.AddTransient<TotalsPage>();
        builder.Services.AddTransient<NotesPage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder.Build();
    }
}
