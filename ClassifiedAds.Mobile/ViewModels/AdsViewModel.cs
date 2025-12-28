using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ClassifiedAds.Mobile.ViewModels;

public partial class AdsViewModel : ObservableObject
{
    private readonly IAdService _adService;
    private readonly LookupService _lookupService; // Inject the new service
    private List<AdDTO> _allAdsBackup = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string searchText;

    // --- FILTER PROPERTIES ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CategoryDisplayText))]
    [NotifyPropertyChangedFor(nameof(CategoryTextColor))]
    private string selectedCategory;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CountryDisplayText))]
    [NotifyPropertyChangedFor(nameof(CountryTextColor))]
    private string selectedCountry;

    [ObservableProperty]
    private string cityFilter;

    [ObservableProperty]
    private string postalCodeFilter;

    // REMOVED: MaxPriceFilter, MaxPriceLimit

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasFilterMessage))]
    private string filterStatusMessage = "";

    public bool HasFilterMessage => !string.IsNullOrEmpty(FilterStatusMessage);

    // --- DISPLAY HELPERS ---
    public string CategoryDisplayText =>
        (string.IsNullOrEmpty(SelectedCategory) || SelectedCategory == "All") ? "Select Category" : SelectedCategory;

    public Color CategoryTextColor =>
        (string.IsNullOrEmpty(SelectedCategory) || SelectedCategory == "All") ? Colors.Gray : Colors.Black;

    public string CountryDisplayText =>
        (string.IsNullOrEmpty(SelectedCountry) || SelectedCountry == "All") ? "Select Country" : SelectedCountry;

    public Color CountryTextColor =>
        (string.IsNullOrEmpty(SelectedCountry) || SelectedCountry == "All") ? Colors.Gray : Colors.Black;

    // --- COLLECTIONS ---
    public ObservableCollection<AdDTO> Ads { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<string> Countries { get; } = new();

    public AdsViewModel(IAdService adService, LookupService lookupService)
    {
        _adService = adService;
        _lookupService = lookupService;

        // Fire and forget: Load both data streams concurrently
        InitializeData();
    }

    private async void InitializeData()
    {
        IsBusy = true;
        // Run API calls in parallel for speed
        var t1 = LoadAds();
        var t2 = LoadLookupData();
        await Task.WhenAll(t1, t2);
        IsBusy = false;
    }

    private async Task LoadLookupData()
    {
        // Fetch Categories and Countries from your API endpoints
        var cats = await _lookupService.GetCategoriesAsync();
        var countries = await _lookupService.GetCountriesAsync();

        Categories.Clear();
        Categories.Add("All");
        foreach (var c in cats) Categories.Add(c);

        Countries.Clear();
        Countries.Add("All");
        foreach (var c in countries) Countries.Add(c);
    }

    [RelayCommand]
    private async Task LoadAds()
    {
        try
        {
            var adsList = await _adService.GetAds();
            _allAdsBackup = adsList ?? new List<AdDTO>();
            ApplyFilters();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
    }

    [RelayCommand]
    public void ApplyFilters()
    {
        FilterStatusMessage = "";
        var query = _allAdsBackup.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var lower = SearchText.ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(lower) || x.Description.ToLower().Contains(lower));
        }

        if (SelectedCategory != "All" && !string.IsNullOrEmpty(SelectedCategory))
            query = query.Where(x => x.Category == SelectedCategory);

        if (SelectedCountry != "All" && !string.IsNullOrEmpty(SelectedCountry))
            query = query.Where(x => x.Country == SelectedCountry);

        if (!string.IsNullOrWhiteSpace(CityFilter))
            query = query.Where(x => x.City.ToLower().Contains(CityFilter.ToLower()));

        if (!string.IsNullOrWhiteSpace(PostalCodeFilter))
            query = query.Where(x => x.PostalCode.ToLower().Contains(PostalCodeFilter.ToLower()));

        // REMOVED: MaxPrice check

        var finalList = query.ToList();

        if (finalList.Count == 0)
        {
            FilterStatusMessage = "No exact matches found. Showing all items.";
            finalList = _allAdsBackup.ToList();
        }

        Ads.Clear();
        foreach (var ad in finalList) Ads.Add(ad);
    }

    [RelayCommand]
    private void ResetFilters()
    {
        SelectedCategory = "All";
        SelectedCountry = "All";
        CityFilter = string.Empty;
        PostalCodeFilter = string.Empty;
        ApplyFilters();
    }

    [RelayCommand]
    private async Task OpenFilters()
    {
        await Shell.Current.Navigation.PushModalAsync(new FilterPage(this));
    }
    
    // ... Other navigation commands ...

    
    [RelayCommand]
    private void SelectCategory(string category)
    {
        SelectedCategory = category;
        ApplyFilters();
    }

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    [RelayCommand]
    private async Task GoToDetails(AdDTO ad)
    {
        if (ad == null) return;
        await Shell.Current.GoToAsync($"{nameof(AdDetailPage)}?Id={ad.Id}");
    }
}