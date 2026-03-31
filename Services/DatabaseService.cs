using ReiskostenApp.Constants;
using ReiskostenApp.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath, DbConstants.Flags);
    }

    public async Task InitializeAsync()
    {
        // Create tables
        await _db.CreateTableAsync<AppSettings>();
        await _db.CreateTableAsync<DayRecord>();
        await _db.CreateTableAsync<MonthMetaRecord>();
        await _db.CreateTableAsync<NoteRecord>();
    }

    // Example: settings
    public Task<AppSettings> GetSettingsAsync() => _db.Table<AppSettings>().FirstOrDefaultAsync();
    public Task<int> SaveSettingsAsync(AppSettings s) => _db.InsertOrReplaceAsync(s);

    // DayRecord operations
    public Task<List<DayRecord>> GetDayRecordsForMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);
        return _db.Table<DayRecord>()
                  .Where(r => r.Date >= start && r.Date < end)
                  .OrderBy(r => r.Date)
                  .ToListAsync();
    }

    public async Task SaveDayRecordAsync(DayRecord record)
    {
        if (record.Id == 0) await _db.InsertAsync(record);
        else await _db.UpdateAsync(record);
    }

    public Task<int> DeleteDayRecordAsync(DayRecord record) => _db.DeleteAsync(record);

    // Month meta and notes similar to above...
}
