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
    public class TotalsViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private MonthMetaRecord? _selected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<MonthMetaRecord> MonthMetas { get; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand DeleteCommand { get; }

        public TotalsViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            RefreshCommand = new Command(async () => await LoadAsync());
            DeleteCommand = new Command<MonthMetaRecord>(async (m) => await DeleteAsync(m));
        }

        public MonthMetaRecord? Selected
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public async Task LoadAsync()
        {
            await _db.InitializeAsync();
            var metas = await _db.GetAllMonthMetaAsync();
            MonthMetas.Clear();
            foreach (var m in metas) MonthMetas.Add(m);
        }

        public async Task DeleteAsync(MonthMetaRecord? meta)
        {
            if (meta == null) return;
            await _db.DeleteMonthMetaAsync(meta);
            MonthMetas.Remove(meta);
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
