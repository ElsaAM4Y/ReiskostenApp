using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.Views
{
    public partial class MonthPage : ContentPage
    {
        private readonly DatabaseService _db;
        private AppSettings _settings = new();
        private int _year;
        private int _month;
        private decimal _ratePerDay;

        public ObservableCollection<DayRecordViewModel> Days { get; } = new();

        public MonthPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
            BindingContext = this;
            LoadPickers();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await EnsureInitializedAsync();

            _year = _settings.SelectedYear == 0 ? DateTime.Now.Year : _settings.SelectedYear;
            _month = _settings.SelectedMonth == 0 ? DateTime.Now.Month : _settings.SelectedMonth;
            _ratePerDay = _settings.RatePerDay;

            MonthPicker.SelectedIndex = Math.Clamp(_month - 1, 0, 11);
            YearPicker.SelectedItem = _year;

            await LoadMonthAsync(_year, _month);
        }

        private async Task EnsureInitializedAsync()
        {
            await _db.InitializeAsync();
            _settings = await _db.GetSettingsAsync() ?? new AppSettings();
            if (_settings.Id == 0) await _db.SaveSettingsAsync(_settings);
        }

        void LoadPickers()
        {
            MonthPicker.ItemsSource = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Take(12).ToList();
            var years = Enumerable.Range(DateTime.Now.Year - 5, 11).ToList();
            YearPicker.ItemsSource = years;

            MonthPicker.SelectedIndexChanged += async (s, e) =>
            {
                if (MonthPicker.SelectedIndex >= 0)
                {
                    _month = MonthPicker.SelectedIndex + 1;
                    await ChangeMonthAsync();
                }
            };

            YearPicker.SelectedIndexChanged += async (s, e) =>
            {
                if (YearPicker.SelectedItem is int selectedYear)
                {
                    _year = selectedYear;
                    await ChangeMonthAsync();
                }
            };
        }

        async Task ChangeMonthAsync()
        {
            _settings.SelectedYear = _year;
            _settings.SelectedMonth = _month;
            await _db.SaveSettingsAsync(_settings);
            await LoadMonthAsync(_year, _month);
        }

        async Task LoadMonthAsync(int year, int month)
        {
            Days.Clear();

            var existing = await _db.GetDayRecordsForMonthAsync(year, month);
            var daysInMonth = DateTime.DaysInMonth(year, month);

            for (int d = 1; d <= daysInMonth; d++)
            {
                var date = new DateTime(year, month, d);
                var rec = existing.FirstOrDefault(r => r.Date.Date == date.Date);

                var vm = new DayRecordViewModel
                {
                    Id = rec?.Id ?? 0,
                    Date = date,
                    Day = d,
                    Value = rec?.Value ?? 0,
                    Notes = rec?.Notes ?? string.Empty,
                    Amount = rec?.Amount ?? 0m
                };

                vm.ValueChanged += async (sender, args) => await OnDayValueChanged(vm);
                vm.NotesChanged += async (sender, args) => await OnDayNotesChanged(vm);

                Days.Add(vm);
            }

            DaysCollection.ItemsSource = Days;
            await UpdateMonthTotalLabel();
        }

        async Task OnDayValueChanged(DayRecordViewModel vm)
        {
            if (vm.Value < 0) vm.Value = 0;
            if (vm.Value > 2) vm.Value = 2;

            var record = new DayRecord
            {
                Id = vm.Id,
                Date = vm.Date,
                Value = vm.Value,
                Notes = vm.Notes
            };

            await _db.SaveDayRecordAsync(record, _ratePerDay);

            var saved = (await _db.GetDayRecordsForMonthAsync(vm.Date.Year, vm.Date.Month))
                        .FirstOrDefault(r => r.Date.Date == vm.Date.Date);
            if (saved != null)
            {
                vm.Id = saved.Id;
                vm.Amount = saved.Amount;
            }

            await UpdateMonthTotalLabel();
        }

        async Task OnDayNotesChanged(DayRecordViewModel vm)
        {
            var record = new DayRecord
            {
                Id = vm.Id,
                Date = vm.Date,
                Value = vm.Value,
                Notes = vm.Notes
            };

            await _db.SaveDayRecordAsync(record, _ratePerDay);
        }

        async Task UpdateMonthTotalLabel()
        {
            var meta = await _db.GetMonthMetaAsync(_year, _month);
            if (meta != null)
                MonthTotalLabel.Text = $"Total days: {meta.TotalDays} — Total amount: {meta.TotalAmount:C}";
            else
                MonthTotalLabel.Text = "No entries yet";
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            foreach (var vm in Days)
            {
                var rec = new DayRecord
                {
                    Id = vm.Id,
                    Date = vm.Date,
                    Value = vm.Value,
                    Notes = vm.Notes
                };
                await _db.SaveDayRecordAsync(rec, _ratePerDay);
            }

            await UpdateMonthTotalLabel();
            await DisplayAlert("Saved", "Month saved.", "OK");
        }
    }

    public class DayRecordViewModel : BindableObject
    {
        private int _value;
        private string _notes = string.Empty;

        public event EventHandler? ValueChanged;
        public event EventHandler? NotesChanged;

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Day { get; set; }

        public int Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                OnPropertyChanged();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                if (_notes == value) return;
                _notes = value;
                OnPropertyChanged();
                NotesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public decimal Amount { get; set; }
    }
}
