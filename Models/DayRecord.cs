using SQLite;

namespace ReiskostenApp.Models;

[Table("DayRecord")]
public class DayRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Canonical date for the record
    public DateTime Date { get; set; }

    // Value is the user input (1 or 2)
    public int Value { get; set; }

    // Stored amount (Value * RatePerDay) - recalculated on save/update
    public decimal Amount { get; set; }

    public string Notes { get; set; } = string.Empty;

    // Convenience computed properties (not mapped by SQLite)
    [Ignore]
    public int Year => Date.Year;
    [Ignore]
    public int Month => Date.Month;
    [Ignore]
    public int Day => Date.Day;
}
