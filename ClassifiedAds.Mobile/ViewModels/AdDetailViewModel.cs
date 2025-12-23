using ClassifiedAds.Mobile.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;


namespace ClassifiedAds.Mobile.ViewModels;

public partial class AdDetailViewModel : ObservableObject
{
    // Using ObservableProperty automatically generates the code needed for UI updates
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string country;

    [ObservableProperty]
    private string city;

    [ObservableProperty]
    private string postalCode;

    [ObservableProperty]
    private string title;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private double price;

    [ObservableProperty]
    private string category;

    [ObservableProperty]
    private string subCategory;

    [ObservableProperty]
    private DateTime createdDate;

    [ObservableProperty]
    private string? imageUrl;

    [ObservableProperty]
    private string mainImageUrl;

    [ObservableProperty]
    private string memberId;

    public AdDetailViewModel()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        // 1. Create a sample DTO (simulating API response)
        var sampleAd = new AdDTO
        {
            Id = 101,
            Title = "Vintage 1970s Film Camera",
            Price = 150.00m,
            Description = "A beautiful, fully functional vintage film camera. Comes with original lens cap and leather strap. Perfect for students or collectors.",
            City = "London",
            Country = "UK",
            PostalCode = "SW1A 1AA",
            Category = "Electronics",
            SubCategory = "Cameras",
            CreatedDate = DateTime.Now.AddDays(-2),
            ImageUrl = "dotnet_bot.png", // Using the default image existing in MAUI project for test
            MainImageUrl = "dotnet_bot.png",
            MemberId = "user123"
        };

        // 2. Map DTO to ViewModel properties
        Id = sampleAd.Id;
        Title = sampleAd.Title;
        Price = (double)(sampleAd.Price ?? 0);
        Description = sampleAd.Description;
        City = sampleAd.City;
        Country = sampleAd.Country;
        PostalCode = sampleAd.PostalCode;
        Category = sampleAd.Category;
        SubCategory = sampleAd.SubCategory;
        CreatedDate = sampleAd.CreatedDate;
        ImageUrl = sampleAd.ImageUrl;
        MainImageUrl = sampleAd.MainImageUrl;
        MemberId = sampleAd.MemberId;
    }


}