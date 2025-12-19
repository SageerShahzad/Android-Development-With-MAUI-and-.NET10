using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.ViewModels;
using ClassifiedAds.Mobile.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Ensure Nuget Package 'CommunityToolkit.Maui' is installed
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 1. Register Services
            builder.Services.AddSingleton<AdsService>();

            // 2. Register ViewModels
            builder.Services.AddTransient<AdsViewModel>();

            // 3. Register Views
            builder.Services.AddTransient<AdsPage>();

            // 4. Remove Android Underline (Handler Mapping)
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}