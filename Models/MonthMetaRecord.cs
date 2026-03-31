using SQLite;

namespace ReiskostenApp.Models
{
    [Table("MonthMetaRecord")]
    public class MonthMetaRecord
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        // Keep both names for compatibility
        public int Total { get; set; }           // legacy name if used elsewhere
        public int TotalDays { get; set; }      // clearer name

        public decimal TotalAmount { get; set; }
        public string Comment { get; set; } = string.Empty;
        public bool Submitted { get; set; }
        public decimal RatePerDay { get; set; }

        // Keep Total in sync if you update TotalDays programmatically
        public void SyncTotals()
        {
            if (TotalDays != Total) Total = TotalDays;
        }
    }
}
