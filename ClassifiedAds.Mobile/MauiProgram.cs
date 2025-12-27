using ClassifiedAds.Mobile.RepoServices.MemberRepoService;
using ClassifiedAds.Mobile.RepoServices.MessageRepoService;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using ClassifiedAds.Mobile.Repositories;
using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.ViewModels;
using ClassifiedAds.Mobile.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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

        // 1. HTTP Client
        builder.Services.AddHttpClient("AdsApi", client =>
        {
            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://localhost:5000"
                : "https://localhost:5001";

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // 2. Authentication (MUST BE SINGLETON for UserAuthViewModel to keep state)
        builder.Services.AddSingleton<IUserAuthRepository, UserAuthRepository>();
        builder.Services.AddSingleton<IUserAuthService, UserAuthService>();

        // IMPORTANT: Changed to Singleton so Profile Data persists across pages
        builder.Services.AddSingleton<UserAuthViewModel>();
        builder.Services.AddSingleton<UserAuthPage>();

        // 3. Members & Profile (NEW - CAUSES CRASH IF MISSING)
        builder.Services.AddSingleton<IMemberRepository, MemberRepository>();
        builder.Services.AddSingleton<IMemberService, MemberService>();
        builder.Services.AddTransient<EditProfileViewModel>();
        builder.Services.AddTransient<EditProfilePage>();

        // 4. Ads
        builder.Services.AddSingleton<IAdRepository, AdRepository>();
        builder.Services.AddSingleton<IAdService, AdService>();
        builder.Services.AddTransient<AdsViewModel>();
        builder.Services.AddTransient<AdsPage>();
        builder.Services.AddTransient<AdDetailViewModel>();
        builder.Services.AddTransient<AdDetailPage>();

        // In MauiProgram.cs
        builder.Services.AddSingleton<SignalRService>(); // Add this line
        // Add these lines in your CreateMauiApp method
        builder.Services.AddSingleton<IMessageRepository, MessageRepository>();
        builder.Services.AddSingleton<IMessageService, MessageService>();

        builder.Services.AddTransient<MessageThreadViewModel>();
        builder.Services.AddTransient<MessageThreadPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}