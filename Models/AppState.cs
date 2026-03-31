namespace ReiskostenApp.Models;

public class AppState
{
    public int SelectedYear { get; set; }
    public int SelectedMonth { get; set; }
    public string SelectedTheme { get; set; } = "System";
}
