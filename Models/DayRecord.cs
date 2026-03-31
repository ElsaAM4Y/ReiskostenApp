using SQLite;

namespace ReiskostenApp.Models;

public class DayRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public int Value { get; set; }
    public string Notes { get; set; } = string.Empty;
}
