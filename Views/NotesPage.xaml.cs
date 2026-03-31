using ReiskostenApp.Views;

namespace ReiskostenApp.Views;

public partial class NotesPage : ContentPage
{
    private NotesViewModel Vm => (NotesViewModel)BindingContext;

    public NotesPage(NotesViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.LoadAsync();
    }
}
