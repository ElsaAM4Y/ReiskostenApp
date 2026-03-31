using ReiskostenApp.ViewModels;

namespace ReiskostenApp.Views;

public partial class TotalsPage : ContentPage
{
    public TotalsPage(MonthTotalViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MonthTotalViewModel vm)
            await vm.LoadAsync();
    }
}
