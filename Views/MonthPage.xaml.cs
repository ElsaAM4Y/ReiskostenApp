using ReiskostenApp.ViewModels;
using ReiskostenApp.Views;

namespace ReiskostenApp.Views;

public partial class MonthPage : ContentPage
{
    private MonthViewModel Vm => (MonthViewModel)BindingContext;

    public MonthPage(MonthViewModel vm)
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
