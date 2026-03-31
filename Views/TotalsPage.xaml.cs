using System;
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
            var metas = await _db.GetAllMonthMetaAsync();
            TotalsCollection.ItemsSource = metas;
        }
    }
}
