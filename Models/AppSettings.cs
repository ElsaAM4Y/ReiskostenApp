using SQLite;

namespace ReiskostenApp.Models;

public class AppSettings
{
    [PrimaryKey]
    public int Id { get; set; } = 1;

    public decimal DefaultRatePerDay { get; set; } = 1.23m;
}
