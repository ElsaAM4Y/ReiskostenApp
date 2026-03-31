using Microsoft.Maui.Controls;

namespace ReiskostenApp.Views;

public partial class TotalsPage : ContentPage
{
    private readonly DatabaseService _db;

    public TotalsPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }
}
