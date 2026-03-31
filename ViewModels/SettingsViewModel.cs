using System.ComponentModel;
using ReiskostenApp.Models;
using ReiskostenApp.Data;

namespace ReiskostenApp.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly AppRepository _repo;

    public MonthMetaRecord Meta { get; private set; }

    public SettingsViewModel()
    {
        _repo = App.Repository;
        LoadMeta();
    }

    private async void LoadMeta()
    {
        Meta = await _repo.GetOrCreateMonthMetaAsync(DateTime.Now.Year, DateTime.Now.Month);
        OnPropertyChanged(nameof(Meta));
    }

    public async Task SaveAsync()
    {
        await _repo.SaveMonthMetaAsync(Meta);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
