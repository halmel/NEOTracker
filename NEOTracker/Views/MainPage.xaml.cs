﻿using Microsoft.Maui.Controls;
using NEOTracker.Services;
using System.Collections.ObjectModel;

namespace NEOTracker.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private readonly NEOApiService _apiService;
        private bool _isActionsVisible = false;

                 private readonly ObservableCollection<Asteroid> _asteroids;

        public MainPage()
        {
            InitializeComponent();

            _databaseService = DatabaseService.GetInstance("NEOTracker.db");
            _apiService = new NEOApiService();

            _asteroids = new ObservableCollection<Asteroid>();
            AsteroidsCollectionView.ItemsSource = _asteroids;          }
        private void ToggleActionsVisibility(object sender, EventArgs e)
        {
            _isActionsVisible = !_isActionsVisible;
            ActionsList.IsVisible = _isActionsVisible;
        }

                 private async void FetchAndSaveAsteroids(object sender, EventArgs e)
        {
            try
            {
                var asteroids = await _apiService.FetchAsteroidsAsync();
                foreach (var asteroid in asteroids)
                {
                    await _databaseService.AddAsteroidAsync(asteroid);
                }

                if (asteroids.Count != 0)
                {
                    await DisplayAlert("Success", "Asteroids fetched and saved!", "OK");
                }
                else
                {
                    await DisplayAlert("No Data", "No new asteroids found to save.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

                 private async void ShowSavedAsteroids(object sender, EventArgs e)
        {
            try
            {
                var savedAsteroids = await _databaseService.GetAsteroidsAsync();

                                 _asteroids.Clear();
                foreach (var asteroid in savedAsteroids)
                {
                    _asteroids.Add(asteroid);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void DeleteDB(object sender, EventArgs e)
        {
            try
            {
                await _databaseService.ClearDatabaseAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}
