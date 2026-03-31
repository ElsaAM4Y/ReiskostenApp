using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;
using ReiskostenApp.ViewModels;

namespace ReiskostenApp.Views
{
    public partial class MonthPage : ContentPage
    {
        private readonly DatabaseService _db;
        private AppSettings _settings;
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

            // Ensure DB and settings are ready
            await EnsureInitializedAsync();

            _year = _settings.SelectedYear == 0 ? DateTime.Now.Year : _settings.SelectedYear;
            _month = _settings.SelectedMonth == 0 ? DateTime.Now.Month : _settings.SelectedMonth;
            _ratePerDay = _settings.RatePerDay;

            // Set pickers safely if ItemsSource already set
            if (MonthPicker.ItemsSource != null)
                MonthPicker.SelectedIndex = Math.Clamp(_month - 1, 0, 11);

            if (YearPicker.ItemsSource != null)
            {
                if (YearPicker.ItemsSource is System.Collections.IList years && years.Contains(_year))
                    YearPicker.SelectedItem = _year;
                else
                    YearPicker.SelectedItem = _year;
            }

            await LoadMonthAsync(_year, _month);
        }

        private async Task EnsureInitializedAsync()
        {
            // Initialize DB tables if not already done
            await _db.InitializeAsync();

            // Load or create settings
            _settings = await _db.GetSettingsAsync() ?? new AppSettings();
            if (_settings.Id == 0) // ensure persisted singleton
                await _db.SaveSettingsAsync(_settings);
        }

        void LoadPickers()
        {
            // Month names and years
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
            // Persist selection
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
            // Clamp allowed values (0..2)
            if (vm.Value < 0) vm.Value = 0;
            if (vm.Value > 2) vm.Value = 2;

            var record = new DayRecord
            {
                Id = vm.Id,
                Date = vm.Date,
                Value = vm.Value,
                Notes = vm.Notes
            };

            // Save and recalc amount using current rate
            await _db.SaveDayRecordAsync(record, _ratePerDay);

            // Refresh saved values (id, amount)
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
            // Save notes change without altering value/amount
            var record = new DayRecord
            {
                Id = vm.Id,
                Date = vm.Date,
                Value = vm.Value,
                Notes = vm.Notes
            };

            await _db.SaveDayRecordAsync(record, _ratePerDay);

            // No need to refresh totals unless value changed, but keep meta consistent
            await UpdateMonthTotalLabel();
        }

        async Task UpdateMonthTotalLabel()
        {
            var meta = await _db