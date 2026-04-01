using Microsoft.Maui.Controls;

namespace ReiskostenApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(Views.MonthPage), typeof(Views.MonthPage));
            Routing.RegisterRoute(nameof(Views.TotalsPage), typeof(Views.TotalsPage));
            Routing.RegisterRoute(nameof(Views.NotesPage), typeof(Views.NotesPage));
            Routing.RegisterRoute(nameof(Views.SettingsPage), typeof(Views.SettingsPage));
        }
    }
}
