using ReiskostenApp.ViewModels;
using ReiskostenApp.Resources.Styles;
// Add the following using directive if your theme classes are in a different namespace
// using ReiskostenApp.Themes;

namespace ReiskostenApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

        }

        void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            string selected = picker.SelectedItem as string;
            Theme theme = selected == "Donker" ? Theme.Dark : Theme.Light;

            ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries != null)
            {
                mergedDictionaries.Clear();

                switch (theme)
                {
                    case Theme.Dark:
                        mergedDictionaries.Add(new DarkTheme()); // <-- Ensure DarkTheme is defined in your project
                        break;
                    case Theme.Light:
                    default:
                        mergedDictionaries.Add(new LightTheme()); // <-- Ensure LightTheme is defined in your project
                        break;
                }
                statusLabel.Text = $"{theme.ToString()} theme geladen.";
            }
        }

    }
}
