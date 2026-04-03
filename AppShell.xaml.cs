using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using ReiskostenApp.Views;

namespace ReiskostenApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for navigation
            Routing.RegisterRoute(nameof(MonthPage), typeof(MonthPage));
            Routing.RegisterRoute(nameof(TotalsPage), typeof(TotalsPage));
            Routing.RegisterRoute(nameof(NotesPage), typeof(NotesPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }

    }
}
