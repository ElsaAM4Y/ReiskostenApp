using SQLite;

namespace ReiskostenApp.Models;

public class NoteRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
}
