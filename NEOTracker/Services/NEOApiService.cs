﻿using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NEOTracker.Models;
using System.Collections.Generic;


namespace NEOTracker.Services
{
    public class NEOApiService
    {
        private readonly HttpClient _httpClient;

        public NEOApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Asteroid>> FetchAsteroidsAsync()
        {
                         if (!Preferences.Default.ContainsKey("Key"))
            {
                Preferences.Default.Set("Key", "DEMO_KEY");
            }
            var apiKey = "zPmwhTa6grD5ahplZADOQrn7BOMvsDLUEgWHyWb5";
            //var apiKey = Preferences.Default.Get("Key", "DEMO_KEY");
            var currentDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var qurrentDate = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
            var apiUrl = $"https://api.nasa.gov/neo/rest/v1/feed?start_date={qurrentDate}&end_date={currentDate}&api_key={apiKey}";

                         var response = await _httpClient.GetAsync(apiUrl);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                await Application.Current.MainPage.DisplayAlert("Bad response", "Check the api key", "OK");
                return new List<Asteroid>();
            }
            response.EnsureSuccessStatusCode();

                         var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonDocument>(jsonResponse);

            var asteroids = new List<Asteroid>();

                         if (data.RootElement.TryGetProperty("near_earth_objects", out var neoObjects) &&
                neoObjects.ValueKind == JsonValueKind.Object)
            {
                foreach (var dateProperty in neoObjects.EnumerateObject())
                {
                    foreach (var asteroidElement in dateProperty.Value.EnumerateArray())
                    {
                                                 var asteroid = new Asteroid
                        {
                            NeoReferenceId = asteroidElement.GetProperty("neo_reference_id").GetString(),
                            Name = asteroidElement.GetProperty("name").GetString(),
                            NasaJplUrl = asteroidElement.GetProperty("nasa_jpl_url").GetString(),
                            AbsoluteMagnitudeH = asteroidElement.GetProperty("absolute_magnitude_h").GetDouble(),
                            EstimatedDiameterMinKm = asteroidElement.GetProperty("estimated_diameter")
                                .GetProperty("kilometers")
                                .GetProperty("estimated_diameter_min")
                                .GetDouble(),
                            EstimatedDiameterMaxKm = asteroidElement.GetProperty("estimated_diameter")
                                .GetProperty("kilometers")
                                .GetProperty("estimated_diameter_max")
                                .GetDouble(),
                            IsPotentiallyHazardous = asteroidElement.GetProperty("is_potentially_hazardous_asteroid").GetBoolean()
                        };

                                                 if (asteroidElement.TryGetProperty("close_approach_data", out var closeApproaches) &&
                            closeApproaches.ValueKind == JsonValueKind.Array)
                        {
                            var nextApproach = closeApproaches.EnumerateArray()
                                .OrderBy(ca => DateTime.Parse(ca.GetProperty("close_approach_date").GetString()))
                                .FirstOrDefault();

                            if (nextApproach.ValueKind != JsonValueKind.Undefined)
                            {
                                asteroid.CloseApproachDate = nextApproach.GetProperty("close_approach_date").GetString();
                                asteroid.CloseApproachDateFull = nextApproach.GetProperty("close_approach_date_full").GetString();

                                                                 var relativeVelocity = nextApproach.GetProperty("relative_velocity");
                                var velocityString = relativeVelocity.GetProperty("kilometers_per_second").GetString();
                                asteroid.RelativeVelocityKmPerSec = Convert.ToDouble(velocityString);

                                                                 var missDistance = nextApproach.GetProperty("miss_distance");
                                var missDistanceKmString = missDistance.GetProperty("kilometers").GetString();
                                asteroid.MissDistanceKm = Convert.ToDouble(missDistanceKmString);


                                                                 asteroid.OrbitingBody = nextApproach.GetProperty("orbiting_body").GetString();
                            }
                        }

                        asteroids.Add(asteroid);
                    }
                }
            }

            return asteroids;
        }



    }
}
