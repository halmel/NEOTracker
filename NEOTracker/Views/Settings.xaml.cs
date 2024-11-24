
namespace NEOTracker.Views;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		var x = new Models.Settings();
		//x.NasaKey = Preferences.Default.Get("Key", "DEMO_KEY");
		x.NasaKey = Preferences.Default.Get("Key", "zPmwhTa6grD5ahplZADOQrn7BOMvsDLUEgWHyWb5");
        BindingContext = x;
    }

    private async void OnEntryCompleted(object sender, EventArgs e)
	{
		Entry x = (Entry)sender;
        Preferences.Default.Set("Key", x.Text);
    }
}