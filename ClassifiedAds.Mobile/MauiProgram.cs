using CommunityToolkit.Maui;

using Microsoft.Extensions.Logging;

using ClassifiedAds.Mobile.Services;     // Add this

using ClassifiedAds.Mobile.Repositories; // Add this



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



        // 1. REGISTER REPOSITORIES & SERVICES

        builder.Services.AddSingleton<IAdRepository, AdRepository>();

        builder.Services.AddSingleton<IAdService, AdService>();



        // 2. REGISTER VIEWS & VIEWMODELS

        builder.Services.AddTransient<Views.AdDetailPage>();

        builder.Services.AddTransient<ViewModels.AdDetailViewModel>();



        // 3. CONFIGURE HTTP CLIENT (The Magic Logic)

        builder.Services.AddHttpClient("AdsApi", client =>

        {

            // Determine URL based on platform

            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android

                ? "http://10.0.2.2:5000"   // Android Emulator looking at PC

                : "http://localhost:5000"; // Windows/Mac



            client.BaseAddress = new Uri(baseUrl);

            client.Timeout = TimeSpan.FromSeconds(30);

        });



#if DEBUG

        builder.Logging.AddDebug();

#endif



        return builder.Build();

    }

}