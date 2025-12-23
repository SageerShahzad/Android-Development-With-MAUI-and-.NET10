using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace ClassifiedAds.Mobile
{

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
			try
			{
                var builder = MauiApp.CreateBuilder();
                builder
                    .UseMauiApp<App>()
                    .UseMauiCommunityToolkit()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                        fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
                    });

                // Register Views and ViewModels
                builder.Services.AddTransient<Views.AdDetailPage>();
                builder.Services.AddTransient<ViewModels.AdDetailViewModel>();

#if DEBUG
                builder.Logging.AddDebug();
#endif

                return builder.Build();
            }
			catch (Exception ex)
			{

                // Put a breakpoint here or look at the Output window
                System.Diagnostics.Debug.WriteLine($"CRASH: {ex.Message}");
                throw;
            }
        }
    }
}
