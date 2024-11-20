using Microsoft.Maui.Controls;
using NEOTracker.Services;

namespace NEOTracker.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly NEOApiService _apiService;

        public MainPage()
        {
            InitializeComponent();
            _databaseService = DatabaseService.GetInstance("NEOTracker.db");
            _apiService = new NEOApiService();
        }

        private async void FetchAndSaveAsteroids(object sender, EventArgs e)
        {
            var asteroids = await _apiService.FetchAsteroidsAsync();
            foreach (var asteroid in asteroids)
            {
                await _databaseService.AddAsteroidAsync(asteroid);
            }

            await DisplayAlert("Success", "Asteroids fetched and saved!", "OK");
        }

        private async void ShowSavedAsteroids(object sender, EventArgs e)
        {
            var asteroids = await _databaseService.GetAsteroidsAsync();
            AsteroidsListView.ItemsSource = asteroids;
        }
    }
}
