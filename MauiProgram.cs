using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using ReiskostenApp.Constants;
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
                .UseMauiApp<App>()               // must be first
                .ConfigureFonts(fonts => { /* fonts */ });

            // CommunityToolkit after UseMauiApp
            builder.UseMauiCommunityToolkit();

            // Register DatabaseService singleton
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DbConstants.DatabaseFilename);
            builder.Services.AddSingleton(new DatabaseService(dbPath));

            // Register Views (Pages) so DI can inject DatabaseService
            builder.Services.AddTransient<ReiskostenApp.Views.MonthPage>();
            builder.Services.AddTransient<ReiskostenApp.Views.TotalsPage>();
            builder.Services.AddTransient<ReiskostenApp.Views.NotesPage>();
            builder.Services.AddTransient<ReiskostenApp.Views.SettingsPage>();

            return builder.Build();
        }
    }
}
