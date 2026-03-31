// ViewModels/TotalsViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.ViewModels
{
    public class TotalsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private bool _isBusy;
        private MonthMetaRecord? _selectedMeta;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<MonthMetaRecord> MonthMetas { get; } = new();

        public ICommand LoadCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectCommand { get; }

        public TotalsViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));

            LoadCommand = new Command(async () => await LoadAsync());
            RefreshCommand = new Command(async () => await LoadAsync());
            DeleteCommand = new Command<MonthMetaRecord>(async m => await DeleteAsync(m));
            SelectCommand = new Command<MonthMetaRecord>(m => SelectedMeta = m);
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set { if (_isBusy == value) return; _isBusy = value; OnPropertyChanged(); }
        }

        public MonthMetaRecord? SelectedMeta
        {
            get => _selectedMeta;
            set { if (_selectedMeta == value) return; _selectedMeta = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Loads all month meta records from the database into the observable collection.
        /// Safe to call multiple times.
        /// </summary>
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await _db.InitializeAsync();
                var metas = await _db.GetAllMonthMetaAsync();
                MonthMetas.Clear();
                if (metas != null)
                {
                    foreach (var m in metas)
                        MonthMetas.Add(m);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Deletes the provided month meta and refreshes the list.
        /// </summary>
        public async Task DeleteAsync(MonthMetaRecord? meta)
        {
            if (meta == null) return;
            await _db.DeleteMonthMetaAsync(meta);
            if (MonthMetas.Contains(meta)) MonthMetas.Remove(meta);
            if (SelectedMeta == meta) SelectedMeta = null;
        }
    }
}
