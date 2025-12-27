using ClassifiedAds.Mobile.RepoServices.MessageRepoService;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.Views; // Ensure this using exists for LoginPage
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClassifiedAds.Mobile.ViewModels;

[QueryProperty(nameof(AdId), "Id")]
public partial class AdDetailViewModel : ObservableObject
{
    private readonly IAdService _adService;
    private readonly IMessageService _messageService;
    private readonly IUserAuthService _authService;
    private readonly IServiceProvider _serviceProvider; // 1. Add Service Provider field

    // Receives the ID from Navigation
    [ObservableProperty]
    private int adId;

    partial void OnAdIdChanged(int value) => LoadAdData(value);

    // Display Properties
    [ObservableProperty] private int id;
    [ObservableProperty] private string title;
    [ObservableProperty] private string description;
    [ObservableProperty] private double price;
    [ObservableProperty] private string city;
    [ObservableProperty] private string country;
    [ObservableProperty] private string mainImageUrl;
    [ObservableProperty] private string category;
    [ObservableProperty] private DateTime createdDate;
    [ObservableProperty] private string memberId;

    // UI Logic
    [ObservableProperty] private bool showLargerImage;
    [ObservableProperty] private double imageHeight = 100;

    partial void OnShowLargerImageChanged(bool value) => ImageHeight = value ? 250 : 100;

    // 2. Inject IServiceProvider into the constructor
    public AdDetailViewModel(
        IAdService adService,
        IMessageService messageService,
        IUserAuthService authService,
        IServiceProvider serviceProvider)
    {
        _adService = adService;
        _messageService = messageService;
        _authService = authService;
        _serviceProvider = serviceProvider;
    }

    private async void LoadAdData(int idToLoad)
    {
        try
        {
            var adDto = await _adService.GetAdById(idToLoad);
            if (adDto != null)
            {
                Id = adDto.Id;
                Title = adDto.Title;
                Price = (double)(adDto.Price ?? 0);
                Description = adDto.Description;
                City = adDto.City;
                Country = adDto.Country;
                Category = adDto.Category;
                CreatedDate = adDto.CreatedDate;
                MemberId = adDto.MemberId;
                MainImageUrl = !string.IsNullOrEmpty(adDto.MainImageUrl) ? adDto.MainImageUrl : "dotnet_bot.png";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ContactSeller()
    {
        // 1. Check if user is logged in
        if (!await _authService.IsAuthenticatedAsync())
        {
            // 3. Resolve LoginPage using the injected provider
            var loginPage = _serviceProvider.GetService<LoginPage>();

            if (loginPage != null)
            {
                await Shell.Current.Navigation.PushModalAsync(loginPage);
            }
            return;
        }

        // 2. Navigate to Chat Page passing the Seller's ID
        await Shell.Current.GoToAsync($"{nameof(MessageThreadPage)}?RecipientId={MemberId}");
    }
}