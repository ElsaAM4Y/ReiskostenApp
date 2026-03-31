using ReiskostenApp.ViewModels;

namespace ReiskostenApp.Views;

public partial class MonthPage : ContentPage
{
    private readonly MonthViewModel vm;

    public MonthPage()
    {
        InitializeComponent();
        vm = new MonthViewModel();
        BindingContext = vm;
    }

    void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        vm.RecalculateTotal();
    }
}
