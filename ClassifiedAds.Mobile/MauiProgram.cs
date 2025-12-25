using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.Repositories;
using ClassifiedAds.Mobile.Views;
using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile;

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

        // 1. Services & Repos
        builder.Services.AddSingleton<IAdRepository, AdRepository>();
        builder.Services.AddSingleton<IAdService, AdService>();

        // 2. ViewModels & Pages
        builder.Services.AddTransient<AdsViewModel>();
        builder.Services.AddTransient<AdsPage>(); // The List Page

        builder.Services.AddTransient<AdDetailViewModel>();
        builder.Services.AddTransient<AdDetailPage>(); // The Detail Page

        // 3. HttpClient
        builder.Services.AddHttpClient("AdsApi", client =>
        {
            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5000"
                : "https://localhost:5001";

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}