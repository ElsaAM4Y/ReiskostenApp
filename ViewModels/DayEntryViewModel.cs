using CommunityToolkit.Mvvm.ComponentModel;

namespace ReiskostenApp.ViewModels;

public partial class DayEntryViewModel : ObservableObject
{
    // Dagnummer (1 t/m 31)
    [ObservableProperty]
    private int day;

    // Aantal keren gewerkt op deze dag
    [ObservableProperty]
    private int count;

    // Aantekeningen voor deze dag
    [ObservableProperty]
    private string notes;

    // Volledige datum (nodig voor opslaan)
    public DateTime Date { get; set; }

    // Label voor UI
    public string DayLabel => day.ToString("00");
}
