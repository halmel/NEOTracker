using Microsoft.Data.Sqlite;
using NEOTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NEOTracker.Services
{
    public class DatabaseService : IDatabaseService
    {
        private static DatabaseService _instance;
        private static readonly object _lock = new object();
        private readonly string _databasePath;

        public static DatabaseService GetInstance(string databaseName)
        {
            lock (_lock)
            {
                return _instance ??= new DatabaseService(databaseName);
            }
        }

        private DatabaseService(string databaseName)
        {
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(folderPath);
            _databasePath = Path.Combine(folderPath, databaseName);
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using var connection = new SqliteConnection($"Data Source={_databasePath}");
                connection.Open();

                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText =
                @"
                CREATE TABLE IF NOT EXISTS Asteroids (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    NeoReferenceId TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    NasaJplUrl TEXT NOT NULL,
                    AbsoluteMagnitudeH REAL NOT NULL,
                    EstimatedDiameterMinKm REAL NOT NULL,
                    EstimatedDiameterMaxKm REAL NOT NULL,
                    IsPotentiallyHazardous INTEGER NOT NULL,
                    CloseApproachDate TEXT NOT NULL,
                    CloseApproachDateFull TEXT,
                    RelativeVelocityKmPerSec REAL NOT NULL,
                    MissDistanceKm REAL NOT NULL,
                    OrbitingBody TEXT NOT NULL
                )
                ";
                createTableCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
            }
        }

        public async Task AddAsteroidAsync(Asteroid asteroid)
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText =
            @"
                INSERT INTO Asteroids (
                    NeoReferenceId, Name, NasaJplUrl, AbsoluteMagnitudeH,
                    EstimatedDiameterMinKm, EstimatedDiameterMaxKm,
                    IsPotentiallyHazardous, CloseApproachDate, CloseApproachDateFull,
                    RelativeVelocityKmPerSec, MissDistanceKm, OrbitingBody
                ) VALUES (
                    $neoReferenceId, $name, $nasaJplUrl, $absoluteMagnitudeH,
                    $estimatedDiameterMinKm, $estimatedDiameterMaxKm,
                    $isPotentiallyHazardous, $closeApproachDate, $closeApproachDateFull,
                    $relativeVelocityKmPerSec, $missDistanceKm, $orbitingBody
                )
            ";

            insertCommand.Parameters.AddWithValue("$neoReferenceId", asteroid.NeoReferenceId);
            insertCommand.Parameters.AddWithValue("$name", asteroid.Name);
            insertCommand.Parameters.AddWithValue("$nasaJplUrl", asteroid.NasaJplUrl);
            insertCommand.Parameters.AddWithValue("$absoluteMagnitudeH", asteroid.AbsoluteMagnitudeH);
            insertCommand.Parameters.AddWithValue("$estimatedDiameterMinKm", asteroid.EstimatedDiameterMinKm);
            insertCommand.Parameters.AddWithValue("$estimatedDiameterMaxKm", asteroid.EstimatedDiameterMaxKm);
            insertCommand.Parameters.AddWithValue("$isPotentiallyHazardous", asteroid.IsPotentiallyHazardous ? 1 : 0);
            insertCommand.Parameters.AddWithValue("$closeApproachDate", asteroid.CloseApproachDate);
            insertCommand.Parameters.AddWithValue("$closeApproachDateFull", asteroid.CloseApproachDateFull ?? string.Empty);
            insertCommand.Parameters.AddWithValue("$relativeVelocityKmPerSec", asteroid.RelativeVelocityKmPerSec);
            insertCommand.Parameters.AddWithValue("$missDistanceKm", asteroid.MissDistanceKm);
            insertCommand.Parameters.AddWithValue("$orbitingBody", asteroid.OrbitingBody);

            await insertCommand.ExecuteNonQueryAsync();
        }

        public async Task<List<Asteroid>> GetAsteroidsAsync()
        {
            var asteroids = new List<Asteroid>();

            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText =
            @"
                SELECT NeoReferenceId, Name, NasaJplUrl, AbsoluteMagnitudeH,
                       EstimatedDiameterMinKm, EstimatedDiameterMaxKm,
                       IsPotentiallyHazardous, CloseApproachDate, CloseApproachDateFull,
                       RelativeVelocityKmPerSec, MissDistanceKm, OrbitingBody
                FROM Asteroids
            ";

            using var reader = await selectCommand.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var asteroid = new Asteroid
                {
                    NeoReferenceId = reader.GetString(0),
                    Name = reader.GetString(1),
                    NasaJplUrl = reader.GetString(2),
                    AbsoluteMagnitudeH = reader.GetDouble(3),
                    EstimatedDiameterMinKm = reader.GetDouble(4),
                    EstimatedDiameterMaxKm = reader.GetDouble(5),
                    IsPotentiallyHazardous = reader.GetInt32(6) == 1,
                    CloseApproachDate = reader.GetString(7),
                    CloseApproachDateFull = reader.IsDBNull(8) ? null : reader.GetString(8),
                    RelativeVelocityKmPerSec = reader.GetDouble(9),
                    MissDistanceKm = reader.GetDouble(10),
                    OrbitingBody = reader.GetString(11)
                };
                asteroids.Add(asteroid);
            }

            return asteroids;
        }

        public async Task DeleteAllAsteroidsAsync()
        {
            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            await connection.OpenAsync();

            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM Asteroids";
            await deleteCommand.ExecuteNonQueryAsync();
        }
    }
}
