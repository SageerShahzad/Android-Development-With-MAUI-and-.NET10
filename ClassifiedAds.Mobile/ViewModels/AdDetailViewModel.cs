using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClassifiedAds.Mobile.ViewModels;

public partial class AdDetailViewModel : ObservableObject
{
    private readonly IAdService _adService;

    // ... Properties ...
    [ObservableProperty] private int id;
    [ObservableProperty] private string title;
    [ObservableProperty] private string description;
    [ObservableProperty] private double price;
    [ObservableProperty] private string city;
    [ObservableProperty] private string country;
    [ObservableProperty] private string mainImageUrl;
    [ObservableProperty] private string imageUrl;
    [ObservableProperty] private string category;
    [ObservableProperty] private DateTime createdDate;
    [ObservableProperty] private string memberId;

    [ObservableProperty] private bool showLargerImage;
    [ObservableProperty] private double imageHeight = 100;

    partial void OnShowLargerImageChanged(bool value)
    {
        ImageHeight = value ? 250 : 100;
    }

    public AdDetailViewModel(IAdService adService)
    {
        _adService = adService;
        LoadAdData(1);
    }

    private async void LoadAdData(int adId)
    {
        try
        {
            var adDto = await _adService.GetAdById(adId);

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

                // --- FIX STARTS HERE ---

                // 1. Properly map the string (don't set it to string.Empty)
                ImageUrl = adDto.ImageUrl ?? "dotnet_bot"; // Fallback if null

                // 2. Logic for MainImageUrl
                // If API sends full URL (http...), use it. 
                // If API sends filename (camera.png), use it.
                // If API sends null, fallback to default.
                MainImageUrl = !string.IsNullOrEmpty(adDto.MainImageUrl)
                    ? adDto.MainImageUrl
                    : "dotnet_bot";

                // --- FIX ENDS HERE ---
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading ad: {ex.Message}");
        }
    }
}