using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.ViewModels
{
    public partial class MonthTotalViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private int _year;
        private int _month;
        private int _totalDays;
        private decimal _totalAmount;
        private decimal _ratePerDay;

        public event PropertyChangedEventHandler? PropertyChanged;

        public MonthTotalViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public int Year
        {
            get => _year;
            set { if (_year == value) return; _year = value; OnPropertyChanged(); }
        }

        public int Month
        {
            get => _month;
            set { if (_month == value) return; _month = value; OnPropertyChanged(); }
        }

        public int TotalDays
        {
            get => _totalDays;
            private set { if (_totalDays == value) return; _totalDays = value; OnPropertyChanged(); }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            private set { if (_totalAmount == value) return; _totalAmount = value; OnPropertyChanged(); }
        }

        public decimal RatePerDay
        {
            get => _ratePerDay;
            private set { if (_ratePerDay == value) return; _ratePerDay = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Load totals for the given year/month and current rate from settings.
        /// Safe to call multiple times; uses DatabaseService async APIs.
        /// </summary>
        public async Task LoadAsync(int year, int month)
        {
            Year = year;
            Month = month;

            // Ensure DB is initialized (idempotent)
            await _db.InitializeAsync();

            // Load settings to get the rate per day (fallback to 0)
            var settings = await _db.GetSettingsAsync() ?? new AppSettings();
            RatePerDay = settings.RatePerDay;

            // Try to read stored month meta first
            var meta = await _db.GetMonthMetaAsync(year, month);
            if (meta != null)
            {
                TotalDays = meta.TotalDays;
                TotalAmount = meta.TotalAmount;
                // keep RatePerDay in sync with stored meta if present
                if (meta.RatePerDay > 0) RatePerDay = meta.RatePerDay;
                return;
            }

            // If no meta row exists, compute from day records
            var days = await _db.GetDayRecordsForMonthAsync(year, month);
            TotalDays = 0;
            TotalAmount = 0m;
            foreach (var d in days)
            {
                TotalDays += d.Value;
                TotalAmount += d.Amount;
            }
        }
    }
}
