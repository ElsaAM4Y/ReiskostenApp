using SQLite;
using ReiskostenApp.Models;

namespace ReiskostenApp.Data;

public class AppRepository
{
    private readonly SQLiteAsyncConnection _db;

    public AppRepository(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);

        _db.CreateTableAsync<VolunteerDay>().Wait();
        _db.CreateTableAsync<MonthMeta>().Wait();
        _db.CreateTableAsync<Note>().Wait();
    }

    // -----------------------------
    // VolunteerDay (maandgegevens)
    // -----------------------------

    public async Task<List<VolunteerDay>> GetMonthEntriesAsync(int year, int month)
    {
        string prefix = $"{year:D4}-{month:D2}-";

        return await _db.Table<VolunteerDay>()
                        .Where(x => x.Date.StartsWith(prefix))
                        .ToListAsync();
    }

    public async Task SaveDayAsync(VolunteerDay day)
    {
        var existing = await _db.Table<VolunteerDay>()
                                .Where(x => x.Date == day.Date)
                                .FirstOrDefaultAsync();

        if (existing == null)
        {
            await _db.InsertAsync(day);
        }
        else
        {
            existing.Count = day.Count;
            existing.Description = day.Description;
            await _db.UpdateAsync(existing);
        }
    }

    public async Task<int> GetTotalCountAsync(int year, int month)
    {
        string prefix = $"{year:D4}-{month:D2}-";

        var items = await _db.Table<VolunteerDay>()
                             .Where(x => x.Date.StartsWith(prefix))
                             .ToListAsync();

        return items.Sum(x => x.Count);
    }

    // -----------------------------
    // MonthMeta (vergoeding + ingediend)
    // -----------------------------

    public async Task<MonthMeta> GetOrCreateMonthMetaAsync(int year, int month)
    {
        var meta = await _db.Table<MonthMeta>()
                            .Where(x => x.Year == year && x.Month == month)
                            .FirstOrDefaultAsync();

        if (meta == null)
        {
            meta = new MonthMeta
            {
                Year = year,
                Month = month,
                RatePerDay = 0m,
                Submitted = false
            };

            await _db.InsertAsync(meta);
        }

        return meta;
    }

    public async Task SaveMonthMetaAsync(MonthMeta meta)
    {
        var existing = await _db.Table<MonthMeta>()
                                .Where(x => x.Year == meta.Year && x.Month == meta.Month)
                                .FirstOrDefaultAsync();

        if (existing == null)
        {
            await _db.InsertAsync(meta);
        }
        else
        {
            existing.RatePerDay = meta.RatePerDay;
            existing.Submitted = meta.Submitted;
            await _db.UpdateAsync(existing);
        }
    }

    // -----------------------------
    // Notes (aantekeningenpagina)
    // -----------------------------

    public async Task<List<Note>> GetNotesAsync()
    {
        return await _db.Table<Note>()
                        .OrderByDescending(n => n.CreatedAt)
                        .ToListAsync();
    }

    public async Task SaveNoteAsync(Note note)
    {
        if (note.Id == 0)
        {
            note.CreatedAt = DateTime.Now;
            await _db.InsertAsync(note);
        }
        else
        {
            await _db.UpdateAsync(note);
        }
    }

    public async Task DeleteNoteAsync(Note note)
    {
        if (note != null)
            await _db.DeleteAsync(note);
    }
}
