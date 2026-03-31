using SQLite;

namespace ReiskostenApp.Models;

public class MonthMeta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int Year { get; set; }

    public int Month { get; set; } // 1-12

    public decimal RatePerDay { get; set; }

    public bool Submitted { get; set; }
}
