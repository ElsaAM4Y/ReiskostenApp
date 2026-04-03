using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace ReiskostenApp
{
    public partial class AppShell : Shell
    {
        public AppShell(IServiceProvider services)
        {
            InitializeComponent();

            MonthContent.ContentTemplate    = new DataTemplate(() => services.GetRequiredService<Views.MonthPage>());
            TotalsContent.ContentTemplate   = new DataTemplate(() => services.GetRequiredService<Views.TotalsPage>());
            NotesContent.ContentTemplate    = new DataTemplate(() => services.GetRequiredService<Views.NotesPage>());
            SettingsContent.ContentTemplate = new DataTemplate(() => services.GetRequiredService<Views.SettingsPage>());

            Routing.RegisterRoute("MonthPage",    typeof(Views.MonthPage));
            Routing.RegisterRoute("TotalsPage",   typeof(Views.TotalsPage));
            Routing.RegisterRoute("NotesPage",    typeof(Views.NotesPage));
            Routing.RegisterRoute("SettingsPage", typeof(Views.SettingsPage));
        }
    }
}
