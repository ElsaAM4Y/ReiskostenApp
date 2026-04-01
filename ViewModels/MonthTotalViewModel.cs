using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.ViewModels
{
    public class MonthTotalViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private int _year;
        private int _month;
        private decimal _ratePerDay;
        private int _totalDays;
        private decimal _totalAmount;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<DayRecord> Days { get; } = new();

        public ICommand SaveCommand { get; }
        public ICommand RefreshCommand { get; }

        public MonthTotalViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            SaveCommand = new Command(async () => await SaveAsync());
            RefreshCommand = new Command(async () => await LoadAsync(_year, _month));
        }

        public int Year
        {
            get => _year;
            set
            {
                if (_year == value) return;
                _year = value;
                OnPropertyChanged(nameof(Year));
            }
        }

        public int Month
        {
            get => _month;
            set
            {
                if (_month == value) return;
                _month = value;
                OnPropertyChanged(nameof(Month));
            }
        }

        public decimal RatePerDay
        {
            get => _ratePerDay;
            set
            {
                if (_ratePerDay == value) return;
                _ratePerDay = value;
                OnPropertyChanged(nameof(RatePerDay));
            }
        }

        public int TotalDays
        {
            get => _totalDays;
            private set
            {
                if (_totalDays == value) return;
                _totalDays = value;
                OnPropertyChanged(nameof(TotalDays));
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            private set
            {
                if (_totalAmount == value) return;
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public async Task LoadAsync(int year, int month)
        {
            Year = year;
            Month = month;

            await _db.InitializeAsync();
            var records = await _db.GetDayRecordsForMonthAsync(year, month);

            Days.Clear();
            foreach (var r in records.OrderBy(r => r.Date))
                Days.Add(r);

            await UpdateTotalsAsync();
        }

        public async Task SaveAsync()
        {
            foreach (var r in Days)
            {
                // Ensure date normalized and amount recalculated by service
                await _db.SaveDayRecordAsync(r, RatePerDay);
            }

            await UpdateTotalsAsync();
        }

        private async Task UpdateTotalsAsync()
        {
            var meta = await _db.GetMonthMetaAsync(Year, Month);
            if (meta != null)
            {
                TotalDays = meta.TotalDays;
                TotalAmount = meta.TotalAmount;
            }
            else
            {
                TotalDays = Days.Sum(d => d.Value);
                TotalAmount = Days.Sum(d => d.Amount);
            }
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
