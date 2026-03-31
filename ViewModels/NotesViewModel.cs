using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ReiskostenApp.Data;
using ReiskostenApp.Models;

namespace ReiskostenApp.Views;

public partial class NotesViewModel : ObservableObject
{
    //public IRelayCommand OpenSettingsCommand => SharedCommands.OpenSettingsCommand;

    private readonly AppRepository _repo;

    [ObservableProperty]
    private ObservableCollection<Note> notes = new();

    [ObservableProperty]
    private string newNoteText;

    public IRelayCommand AddNoteCommand { get; }
    public IRelayCommand<Note> DeleteNoteCommand { get; }

    public NotesViewModel(AppRepository repo)
    {
        _repo = repo;

        AddNoteCommand = new RelayCommand(async () => await AddNoteAsync());
        DeleteNoteCommand = new RelayCommand<Note>(async (note) => await DeleteNoteAsync(note));
    }

    public async Task LoadAsync()
    {
        Notes.Clear();
        var items = await _repo.GetNotesAsync();

        foreach (var n in items)
            Notes.Add(n);
    }

    private async Task AddNoteAsync()
    {
        if (string.IsNullOrWhiteSpace(NewNoteText))
            return;

        var note = new Note
        {
            Text = NewNoteText,
            CreatedAt = DateTime.Now
        };

        await _repo.SaveNoteAsync(note);
        Notes.Add(note);

        NewNoteText = string.Empty;
    }

    private async Task DeleteNoteAsync(Note note)
    {
        if (note == null)
            return;

        await _repo.DeleteNoteAsync(note);
        Notes.Remove(note);
    }
}
