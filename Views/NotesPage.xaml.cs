using Microsoft.Maui.Controls;
using ReiskostenApp.Services;

namespace ReiskostenApp.Views;

public partial class NotesPage : ContentPage
{
    private readonly DatabaseService _db;

    public NotesPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }
}
