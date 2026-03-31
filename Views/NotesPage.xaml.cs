using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;
using System;
using System.Linq;

namespace ReiskostenApp.Views
{
    public partial class NotesPage : ContentPage
    {
        private readonly DatabaseService _db;
        private NoteRecord? _selected;

        public NotesPage(DatabaseService db)
        {
            InitializeComponent();
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _db.InitializeAsync();
            var notes = await _db.GetNotesAsync();
            NotesCollection.ItemsSource = notes;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            var note = new NoteRecord { Title = "New note", Content = string.Empty, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
            await _db.SaveNoteAsync(note);
            var notes = await _db.GetNotesAsync();
            NotesCollection.ItemsSource = notes;
            SelectNote(note);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is NoteRecord note)
            {
                SelectNote(note);
            }
        }

        private void SelectNote(NoteRecord note)
        {
            _selected = note;
            NoteTitle.Text = note.Title;
            NoteContent.Text = note.Content;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (_selected == null)
            {
                await DisplayAlert("No note", "Select or add a note first.", "OK");
                return;
            }

            _selected.Title = NoteTitle.Text ?? string.Empty;
            _selected.Content = NoteContent.Text ?? string.Empty;
            _selected.Updated = DateTime.UtcNow;

            await _db.SaveNoteAsync(_selected);
            var notes = await _db.GetNotesAsync();
            NotesCollection.ItemsSource = notes;
            await DisplayAlert("Saved", "Note saved.", "OK");
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_selected == null) return;
            await _db.DeleteNoteAsync(_selected);
            var notes = await _db.GetNotesAsync();
            NotesCollection.ItemsSource = notes;
            _selected = null;
            NoteTitle.Text = string.Empty;
            NoteContent.Text = string.Empty;
        }
    }
}
