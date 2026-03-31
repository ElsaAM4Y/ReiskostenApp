using System.Collections.ObjectModel;
using System.ComponentModel;
using ReiskostenApp.Models;
using ReiskostenApp.Data;

namespace ReiskostenApp.ViewModels;

public class NotesViewModel : INotifyPropertyChanged
{
    private readonly AppRepository _repo;

    public ObservableCollection<NoteRecord> Notes { get; set; }
        = new ObservableCollection<NoteRecord>();

    public NotesViewModel()
    {
        _repo = App.Repository;
        LoadNotes();
    }

    private async void LoadNotes()
    {
        Notes.Clear();
        var items = await _repo.GetNotesAsync();
        foreach (var n in items)
            Notes.Add(n);
    }

    public async Task SaveAsync(NoteRecord note)
    {
        await _repo.SaveNoteAsync(note);
        LoadNotes();
    }

    public async Task DeleteAsync(NoteRecord note)
    {
        await _repo.DeleteNoteAsync(note);
        LoadNotes();
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
