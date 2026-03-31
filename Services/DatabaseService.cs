// Services/DatabaseService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using ReiskostenApp.Models;

namespace ReiskostenApp.Services
{
    /// <summary>
    /// Simple SQLite-backed data access service using sqlite-net-pcl.
    /// Register as a singleton in MauiProgram and inject into pages/viewmodels.
    /// </summary>
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        /// <summary>
        /// Create DatabaseService with a full path to the sqlite file.
        /// Example: Path.Combine(FileSystem.AppDataDirectory, "reiskosten.db3")
        /// </summary>
        public DatabaseService(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentException("dbPath must be provided", nameof(dbPath));

            // Recommended flags for async multi-threaded usage in mobile apps
            var flags = SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache;
            _db = new SQLiteAsyncConnection(dbPath, flags);
        }

        /// <summary>
        /// Ensure tables exist. Idempotent.
        /// Call once at app startup (App.OnStart) or before first use.
        /// </summary>
        public Task InitializeAsync()
        {
            return Task.WhenAll(
                _db.CreateTableAsync<AppSettings>(),
                _db.CreateTableAsync<DayRecord>(),
                _db.CreateTableAsync<MonthMetaRecord>(),
                _db.CreateTableAsync<NoteRecord>()
            );
        }

        // -----------------------
        // Settings
        // -----------------------
        public Task<AppSettings?> GetSettingsAsync()
        {
            return _db.Table<AppSettings>().FirstOrDefaultAsync();
        }

        public Task<int> SaveSettingsAsync(AppSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            // If Id == 0 insert, otherwise update (InsertOrReplaceAsync also works)
            return _db.InsertOrReplaceAsync(settings);
        }

        // -----------------------
        // DayRecord operations
        // -----------------------
        public Task<List<DayRecord>> GetDayRecordsForMonthAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            return _db.Table<DayRecord>()
                      .Where(r => r.Date >= start && r.Date < end)
                      .OrderBy(r => r.Date)
                      .ToListAsync();
        }

        /// <summary>
        /// Save a day record and recalculate amount using provided ratePerDay.
        /// Also updates month meta row.
        /// </summary>
        public async Task SaveDayRecordAsync(DayRecord record, decimal ratePerDay)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            // Ensure Date has no time component for consistent queries
            record.Date = record.Date.Date;

            // Recalculate amount
            record.Amount = record.Value * ratePerDay;

            if (record.Id == 0)
                await _db.InsertAsync(record);
            else
                await _db.UpdateAsync(record);

            // Recalculate and persist month meta
            await UpdateMonthMetaAsync(record.Date.Year, record.Date.Month, ratePerDay);
        }

        public Task<int> DeleteDayRecordAsync(DayRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            return _db.DeleteAsync(record);
        }

        // -----------------------
        // Month meta operations
        // -----------------------
        public Task<MonthMetaRecord?> GetMonthMetaAsync(int year, int month)
        {
            return _db.Table<MonthMetaRecord>()
                      .Where(m => m.Year == year && m.Month == month)
                      .FirstOrDefaultAsync();
        }

        public Task<List<MonthMetaRecord>> GetAllMonthMetaAsync()
        {
            return _db.Table<MonthMetaRecord>()
                      .OrderByDescending(m => m.Year)
                      .ThenByDescending(m => m.Month)
                      .ToListAsync();
        }

        public Task<int> DeleteMonthMetaAsync(MonthMetaRecord meta)
        {
            if (meta == null) throw new ArgumentNullException(nameof(meta));
            return _db.DeleteAsync(meta);
        }

        /// <summary>
        /// Recalculate totals for the given month and persist a MonthMetaRecord.
        /// Creates the meta row if missing, otherwise updates it.
        /// </summary>
        public async Task UpdateMonthMetaAsync(int year, int month, decimal ratePerDay)
        {
            var days = await GetDayRecordsForMonthAsync(year, month);
            var totalDays = days.Sum(d => d.Value);
            var totalAmount = days.Sum(d => d.Amount);

            var meta = await GetMonthMetaAsync(year, month);
            if (meta == null)
            {
                meta = new MonthMetaRecord
                {
                    Year = year,
                    Month = month,
                    TotalDays = totalDays,
                    TotalAmount = totalAmount,
                    RatePerDay = ratePerDay,
                    Total = totalDays
                };
                await _db.InsertAsync(meta);
            }
            else
            {
                meta.TotalDays = totalDays;
                meta.TotalAmount = totalAmount;
                meta.RatePerDay = ratePerDay;
                meta.Total = totalDays;
                await _db.UpdateAsync(meta);
            }
        }

        // -----------------------
        // Notes
        // -----------------------
        public Task<List<NoteRecord>> GetNotesAsync()
        {
            return _db.Table<NoteRecord>()
                      .OrderByDescending(n => n.Updated)
                      .ToListAsync();
        }

        public async Task SaveNoteAsync(NoteRecord note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            var now = DateTime.UtcNow;
            note.Updated = now;
            if (note.Id == 0)
            {
                note.Created = now;
                await _db.InsertAsync(note);
            }
            else
            {
                await _db.UpdateAsync(note);
            }
        }

        public Task<int> DeleteNoteAsync(NoteRecord note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));
            return _db.DeleteAsync(note);
        }

        // -----------------------
        // Utility helpers (optional)
        // -----------------------
        /// <summary>
        /// Convenience: ensure DB file exists and return its path.
        /// Not required if you pass the path from MauiProgram.
        /// </summary>
        public static string EnsureDatabasePath(string filename = "reiskosten.db3")
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return Path.Combine(folder, filename);
        }
    }
}
