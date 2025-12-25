using ClassifiedAds.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassifiedAds.Mobile.ViewModels;

[QueryProperty(nameof(AdId), "Id")]
public partial class AdDetailViewModel : ObservableObject
{
    private readonly IAdService _adService;

    // Receives the ID from Navigation
    [ObservableProperty]
    private int adId;

    partial void OnAdIdChanged(int value)
    {
        LoadAdData(value);
    }

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

    public AdDetailViewModel(IAdService adService)
    {
        _adService = adService;
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
                MainImageUrl = !string.IsNullOrEmpty(adDto.MainImageUrl) ? adDto.MainImageUrl : "dotnet_bot";
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
        }
    }
}