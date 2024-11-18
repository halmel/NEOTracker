using System.Net.Http;
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

        public async Task<List<Asteroid>> FetchAsteroidsAsync(string apiUrl)
        {
            // Fetch data from the API
            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            // Read the JSON response
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonDocument>(jsonResponse);

            var asteroids = new List<Asteroid>();

            try
            {
                // Directly access the root properties of the JSON response
                var root = data.RootElement;

                // Create an Asteroid object
                var asteroid = new Asteroid
                {
                    NeoReferenceId = root.GetProperty("neo_reference_id").GetString(),
                    Name = root.GetProperty("name").GetString(),
                    NasaJplUrl = root.GetProperty("nasa_jpl_url").GetString(),
                    AbsoluteMagnitudeH = root.GetProperty("absolute_magnitude_h").GetDouble(),
                    EstimatedDiameterMinKm = root.GetProperty("estimated_diameter")
                                                    .GetProperty("kilometers")
                                                    .GetProperty("estimated_diameter_min")
                                                    .GetDouble(),
                    EstimatedDiameterMaxKm = root.GetProperty("estimated_diameter")
                                                    .GetProperty("kilometers")
                                                    .GetProperty("estimated_diameter_max")
                                                    .GetDouble(),
                    IsPotentiallyHazardous = root.GetProperty("is_potentially_hazardous_asteroid").GetBoolean()
                };

                // Handle close approach data
                if (root.TryGetProperty("close_approach_data", out var closeApproachArray) &&
                    closeApproachArray.ValueKind == JsonValueKind.Array && closeApproachArray.GetArrayLength() > 0)
                {
                    var closeApproach = closeApproachArray[0]; // Take the first close approach entry

                    asteroid.CloseApproachDate = closeApproach.GetProperty("close_approach_date").GetString();
                    asteroid.CloseApproachDateFull = closeApproach.GetProperty("close_approach_date_full").GetString();
                    asteroid.RelativeVelocityKmPerSec = closeApproach.GetProperty("relative_velocity")
                                                                     .GetProperty("kilometers_per_second").GetDouble();
                    asteroid.MissDistanceKm = closeApproach.GetProperty("miss_distance")
                                                           .GetProperty("kilometers").GetDouble();
                    asteroid.OrbitingBody = closeApproach.GetProperty("orbiting_body").GetString();
                }

                // Add the asteroid object to the list
                asteroids.Add(asteroid);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Error: Missing expected key in the JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error occurred while parsing: {ex.Message}");
            }

            return asteroids;
        }
    }
}
