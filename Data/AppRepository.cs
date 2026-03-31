using SQLite;
using ReiskostenApp.Models;

namespace ReiskostenApp.Data;

public class AppRepository
{
    private readonly SQLiteAsyncConnection _db;

    public AppRepository(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        Initialize().Wait();
    }

    private async Task Initialize()
    {
        await _db.CreateTableAsync<DayRecord>();
        await _db.CreateTableAsync<NoteRecord>();
        await _db.CreateTableAsync<MonthMetaRecord>();
    }

    // -----------------------------
    // DAY RECORDS
    // -----------------------------
    public async Task<List<DayRecord>> GetMonthAsync(int year, int month)
    {
        return await _db.Table<DayRecord>()
            .Where(x => x.Year == year && x.Month == month)
            .ToListAsync();
    }

    public async Task SaveDayAsync(DayRecord record)
    {
        if (record.Id == 0)
            await _db.InsertAsync(record);
        else
            await _db.UpdateAsync(record);
    }

    // -----------------------------
    // NOTES
    // -----------------------------
    public async Task<List<NoteRecord>> GetNotesAsync()
    {
        return await _db.Table<NoteRecord>()
            .OrderByDescending(x => x.Created)
            .ToListAsync();
    }

    public async Task SaveNoteAsync(NoteRecord note)
    {
        if (note.Id == 0)
            await _db.InsertAsync(note);
        else
            await _db.UpdateAsync(note);
    }

    public async Task DeleteNoteAsync(NoteRecord note)
    {
        await _db.DeleteAsync(note);
    }

    // -----------------------------
    // MONTH META
    // -----------------------------
    public async Task<MonthMetaRecord> GetOrCreateMonthMetaAsync(int year, int month)
    {
        var meta = await _db.Table<MonthMetaRecord>()
            .Where(x => x.Year == year && x.Month == month)
            .FirstOrDefaultAsync();

        if (meta == null)
        {
            meta = new MonthMetaRecord
            {
                Year = year,
                Month = month,
                Total = 0,
                Comment = ""
            };

            await _db.InsertAsync(meta);
        }

        return meta;
    }

    public async Task SaveMonthMetaAsync(MonthMetaRecord meta)
    {
        if (meta.Id == 0)
            await _db.InsertAsync(meta);
        else
            await _db.UpdateAsync(meta);
    }

    // -----------------------------
    // TOTALS
    // -----------------------------
    public async Task<int> GetTotalCountAsync(int year)
    {
        var all = await _db.Table<DayRecord>()
            .Where(x => x.Year == year)
            .ToListAsync();

        return all.Sum(x => x.Value);
    }
}
