using System;
using System.Linq;
using Microsoft.Maui.Controls;
using ReiskostenApp.Models;
using ReiskostenApp.Services;

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

        private void OnAddClicked(object sender, EventArgs e)
        {
            _selected = null;
            NoteTitle.Text = string.Empty;
            NoteContent.Text = string.Empty;
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
                _selected = new NoteRecord { Created = DateTime.UtcNow };

            _selected.Title = NoteTitle.Text ?? string.Empty;
            _selected.Content = NoteContent.Text ?? string.Empty;
            _selected.Updated = DateTime.UtcNow;

            await _db.SaveNoteAsync(_selected);
            var notes = await _db.GetNotesAsync();
            NotesCollection.ItemsSource = notes;
            // _selected already has the updated Id after SaveNoteAsync
            await DisplayAlert("Opgeslagen", "Aantekening opgeslagen.", "OK");
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
