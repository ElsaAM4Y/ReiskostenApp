using SQLite;
using System;

namespace ReiskostenApp.Models
{
    [Table("NoteRecord")]
    public class NoteRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // Provide both names so older code and new code compile
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        // Optional clearer aliases (not mapped)
        [Ignore]
        public DateTime CreatedAt { get => Created; set => Created = value; }
        [Ignore]
        public DateTime UpdatedAt { get => Updated; set => Updated = value; }
    }
}
