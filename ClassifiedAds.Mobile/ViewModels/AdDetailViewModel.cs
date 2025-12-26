using ClassifiedAds.Mobile.RepoServices.MessageRepoService;
using ClassifiedAds.Mobile.RepoServices.UserAuthRepoService;
using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClassifiedAds.Mobile.ViewModels;

[QueryProperty(nameof(AdId), "Id")]
public partial class AdDetailViewModel : ObservableObject
{
    private readonly IAdService _adService;
    private readonly IMessageService _messageService;
    private readonly IUserAuthService _authService;

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
    [ObservableProperty] private string memberId; // The Advertiser's ID

    // UI Logic
    [ObservableProperty] private bool showLargerImage;
    [ObservableProperty] private double imageHeight = 100;

    partial void OnShowLargerImageChanged(bool value) => ImageHeight = value ? 250 : 100;
    // Inject the Message Service and Auth Service
    public AdDetailViewModel(IAdService adService, IMessageService messageService, IUserAuthService authService)
    {
        _adService = adService;
        _messageService = messageService;
        _authService = authService;
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
                MemberId = adDto.MemberId; // Crucial for messaging!
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
        // 1. Check Auth
        if (!await _authService.IsAuthenticatedAsync())
        {
            await Shell.Current.DisplayAlert("Login Required", "Please login.", "OK");
            return;
        }

        // 2. Navigate to Chat Page passing the Seller's ID
        // We pass "RecipientId" as a query parameter
        await Shell.Current.GoToAsync($"{nameof(MessageThreadPage)}?RecipientId={MemberId}");
    }

}