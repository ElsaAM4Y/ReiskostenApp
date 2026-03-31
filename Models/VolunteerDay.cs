using SQLite;

namespace ReiskostenApp.Models;

public class VolunteerDay
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // yyyy-MM-dd
    public string Date { get; set; } = string.Empty;

    public int Count { get; set; }

    public string Description { get; set; } = string.Empty;
}
