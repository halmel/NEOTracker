using System.Collections.ObjectModel;
using System.Windows.Input;
using NEOTracker.Models;
using NEOTracker.Services;

namespace NEOTracker.ViewModels
{
    public class AsteroidsViewModel : BaseViewModel
    {
        private readonly NEOApiService _neoApiService;
        private readonly IDatabaseService _databaseService;

        public ObservableCollection<Asteroid> Asteroids { get; } = new();

        public ICommand FetchAsteroidsCommand { get; }

        public AsteroidsViewModel(NEOApiService neoApiService, IDatabaseService databaseService)
        {
            _neoApiService = neoApiService;
            _databaseService = databaseService;

            FetchAsteroidsCommand = new Command(async () => await FetchAsteroidsAsync());
        }

        private async Task FetchAsteroidsAsync()
        {
            var asteroids = await _neoApiService.FetchAsteroidsAsync();
            Asteroids.Clear();
            foreach (var asteroid in asteroids)
            {
                Asteroids.Add(asteroid);
                await _databaseService.AddAsteroidAsync(asteroid);
            }
        }
    }
}
