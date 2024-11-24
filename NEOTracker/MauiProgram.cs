using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using NEOTracker.Services;
using NEOTracker.ViewModels;
using NEOTracker.Views;

namespace NEOTracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services for Dependency Injection
        builder.Services.AddSingleton<IDatabaseService>(s => DatabaseService.GetInstance("asteroids.db"));
        builder.Services.AddSingleton<NEOApiService>();
        builder.Services.AddSingleton<AsteroidsViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
