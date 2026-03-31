// ViewModels/NotesViewModel.cs
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
    public class NotesViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _db;
        private bool _isBusy;
        private NoteRecord? _selectedNote;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<NoteRecord> Notes { get; } = new();

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public NotesViewModel(DatabaseService db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));

            LoadCommand = new Command(async () => await LoadAsync());
            AddCommand = new Command(async () => await AddNoteAsync());
            SaveCommand = new Command<NoteRecord>(async n => await SaveNoteAsync(n));
            DeleteCommand = new Command<NoteRecord>(async n => await DeleteNoteAsync(n));
        }

        public bool IsBusy
        {
            get => _isBusy;
            private set { if (_isBusy == value) return; _isBusy = value; OnPropertyChanged(); }
        }

        public NoteRecord? SelectedNote
        {
            get => _selectedNote;
            set { if (_selectedNote == value) return; _selectedNote = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Loads notes from the database into the observable collection.
        /// Safe to call multiple times.
        /// </summary>
        public async Task LoadAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await _db.InitializeAsync(); // idempotent
                var notes = await _db.GetNotesAsync();
                Notes.Clear();
                if (notes != null)
                {
                    foreach (var n in notes)
                        Notes.Add(n);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Adds a new empty note and selects it.
        /// </summary>
        public async Task AddNoteAsync()
        {
            var note = new NoteRecord
            {
                Title = "New note",
                Content = string.Empty,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            await _db.SaveNoteAsync(note);
            Notes.Insert(0, note);
            SelectedNote = note;
        }

        /// <summary>
        /// Saves changes to an existing or new note.
        /// </summary>
        public async Task SaveNoteAsync(NoteRecord? note)
        {
            if (note == null) return;
            note.Updated = DateTime.UtcNow;
            await _db.SaveNoteAsync(note);

            // Refresh collection item (ensure latest values)
            var idx = Notes.IndexOf(note);
            if (idx >= 0)
            {
                Notes[idx] = note;
            }
            else
            {
                Notes.Insert(0, note);
            }
        }

        /// <summary>
        /// Deletes the provided note from the database and collection.
        /// </summary>
        public async Task DeleteNoteAsync(NoteRecord? note)
        {
            if (note == null) return;
            await _db.DeleteNoteAsync(note);
            if (Notes.Contains(note)) Notes.Remove(note);
            if (SelectedNote == note) SelectedNote = null;
        }
    }
}
