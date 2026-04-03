using System;
using System.Linq;
using Microsoft.Maui.Controls;
using ReiskostenApp.Services;

namespace ReiskostenApp.Views
{
    public partial class TotalsPage : ContentPage
    {
        private readonly DatabaseService _db;

        public TotalsPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _db.InitializeAsync();

            var settings = await _db.GetSettingsAsync();
            var rate = settings?.RatePerDay ?? 0m;

            var metas = await _db.GetAllMonthMetaAsync();

            // Recalculate TotalAmount using current rate so it stays in sync
            foreach (var m in metas)
                m.TotalAmount = m.TotalDays * rate;

            TotalsCollection.ItemsSource = metas
                .Select(m => new
                {
                    MonthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m.Month),
                    m.Year,
                    m.TotalDays,
                    TotalAmount = m.TotalDays * rate
                })
                .ToList();
        }
    }
}
