using SQLite;

namespace ReiskostenApp.Models;

public class MonthMetaRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }

    public int Total { get; set; }
    public string Comment { get; set; } = string.Empty;

    public bool Submitted { get; set; }
    public decimal RatePerDay { get; set; }
}
