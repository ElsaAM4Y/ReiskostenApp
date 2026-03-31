using SQLite;

namespace ReiskostenApp.Models;

public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Text { get; set; } = string.Empty;
}
