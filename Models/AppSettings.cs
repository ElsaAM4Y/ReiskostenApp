using SQLite;

namespace ReiskostenApp.Models;

[Table("AppSettings")]
public class AppSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1; // singleton row

    // Persisted UI state
    public int SelectedYear { get; set; } = DateTime.Now.Year;
    public int SelectedMonth { get; set; } = DateTime.Now.Month;

    // Theme: "System", "Light", "Dark"
    public string SelectedTheme { get; set; } = "System";

    // Rate per day (default)
    public decimal RatePerDay { get; set; } = 1.23m;
}
