// MauiProgram.cs
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using ReiskostenApp.Services;
using System.IO;

namespace ReiskostenApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()               // must come before toolkit chaining
                .ConfigureFonts(fonts => { /* add fonts if needed */ });

            // CommunityToolkit after UseMauiApp
            builder.UseMauiCommunityToolkit();

            // Register DatabaseService singleton
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "reiskosten.db3");
            builder.Services.AddSingleton(new DatabaseService(dbPath));

            // Register Views so DI can construct them
            builder.Services.AddTransient<ReiskostenApp.Views.MonthPage>();
            builder.Services.AddTransient<ReiskostenApp.Views.TotalsPage>();
            builder.Services.AddTransient<ReiskostenApp.Views.NotesPage>();
            builder.Services.AddTransient<ReiskostenApp.Views.SettingsPage>();

            // Register ViewModels if you prefer DI for them
            builder.Services.AddTransient<ReiskostenApp.ViewModels.MonthTotalViewModel>();
            builder.Services.AddTransient<ReiskostenApp.ViewModels.TotalsViewModel>();
            builder.Services.AddTransient<ReiskostenApp.ViewModels.NotesViewModel>();
            builder.Services.AddTransient<ReiskostenApp.ViewModels.SettingsViewModel>();

            return builder.Build();
        }
    }
}
