using ReiskostenApp.Data;
using ReiskostenApp.Models;
using ReiskostenApp.ViewModels;

namespace ReiskostenApp.Views;

public partial class MonthPage : ContentPage
{
    private MonthViewModel vm;

    public MonthPage(AppRepository repo, AppState state)
    {
        InitializeComponent();
        vm = new MonthViewModel(repo, state);
        BindingContext = vm;
    }

    void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        vm.RecalculateTotal();
    }
}
