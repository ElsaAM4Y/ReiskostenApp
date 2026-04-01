using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

namespace ReiskostenApp.ViewModels
{
    public class NotesViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private NoteRecord? _selected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<NoteRecord> Notes { get; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public NotesViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            RefreshCommand = new Command(async () => await LoadAsync());
            AddCommand = new Command(async () => await AddAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            DeleteCommand = new Command(async () => await DeleteAsync());
        }

        public NoteRecord? Selected
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
            var notes = await _db.GetNotesAsync();
            Notes.Clear();
            foreach (var n in notes) Notes.Add(n);
        }

        public async Task AddAsync()
        {
            var now = DateTime.UtcNow;
            var note = new NoteRecord { Title = "New note", Content = string.Empty, Created = now, Updated = now };
            await _db.SaveNoteAsync(note);
            Notes.Insert(0, note);
            Selected = note;
        }

        public async Task SaveAsync()
        {
            if (Selected == null) return;
            Selected.Updated = DateTime.UtcNow;
            await _db.SaveNoteAsync(Selected);
            await LoadAsync();
        }

        public async Task DeleteAsync()
        {
            if (Selected == null) return;
            await _db.DeleteNoteAsync(Selected);
            Notes.Remove(Selected);
            Selected = null;
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
