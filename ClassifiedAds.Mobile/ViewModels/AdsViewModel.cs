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
    private readonly LookupService _lookupService;
    private List<AdDTO> _allAdsBackup = new();

    // Store country objects to map Name -> ID
    private List<CountryDto> _allCountryObjects = new();

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string searchText;

    // --- FILTER PROPERTIES ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CategoryDisplayText))]
    [NotifyPropertyChangedFor(nameof(CategoryTextColor))]
    private string selectedCategory;

    // Trigger City loading when Country changes
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CountryDisplayText))]
    [NotifyPropertyChangedFor(nameof(CountryTextColor))]
    private string selectedCountry;

    partial void OnSelectedCountryChanged(string value)
    {
        // Fire and forget logic to load cities
        _ = LoadCitiesForSelectedCountry();
    }

    // NEW: City Selection
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CityDisplayText))]
    [NotifyPropertyChangedFor(nameof(CityTextColor))]
    private string selectedCity;

    [ObservableProperty] private string postalCodeFilter;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(HasFilterMessage))] private string filterStatusMessage = "";

    public bool HasFilterMessage => !string.IsNullOrEmpty(FilterStatusMessage);

    // --- DISPLAY HELPERS ---
    public string CategoryDisplayText => (string.IsNullOrEmpty(SelectedCategory) || SelectedCategory == "All") ? "Select Category" : SelectedCategory;
    public Color CategoryTextColor => (string.IsNullOrEmpty(SelectedCategory) || SelectedCategory == "All") ? Colors.Gray : Colors.Black;

    public string CountryDisplayText => (string.IsNullOrEmpty(SelectedCountry) || SelectedCountry == "All") ? "Select Country" : SelectedCountry;
    public Color CountryTextColor => (string.IsNullOrEmpty(SelectedCountry) || SelectedCountry == "All") ? Colors.Gray : Colors.Black;

    // City Helpers
    public string CityDisplayText => (string.IsNullOrEmpty(SelectedCity) || SelectedCity == "All") ? "Select City" : SelectedCity;
    public Color CityTextColor => (string.IsNullOrEmpty(SelectedCity) || SelectedCity == "All") ? Colors.Gray : Colors.Black;

    // --- COLLECTIONS ---
    public ObservableCollection<AdDTO> Ads { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<string> Countries { get; } = new();
    public ObservableCollection<string> Cities { get; } = new(); // NEW Collection

    public AdsViewModel(IAdService adService, LookupService lookupService)
    {
        _adService = adService;
        _lookupService = lookupService;
        InitializeData();
    }

    private async void InitializeData()
    {
        IsBusy = true;
        var t1 = LoadAds();
        var t2 = LoadLookupData();
        await Task.WhenAll(t1, t2);
        IsBusy = false;
    }

    private async Task LoadLookupData()
    {
        var cats = await _lookupService.GetCategoriesAsync();
        // Fetch full country objects
        _allCountryObjects = await _lookupService.GetCountriesAsync();

        Categories.Clear();
        Categories.Add("All");
        foreach (var c in cats) Categories.Add(c);

        Countries.Clear();
        Countries.Add("All");
        foreach (var c in _allCountryObjects) Countries.Add(c.Name);
    }

    private async Task LoadCitiesForSelectedCountry()
    {
        Cities.Clear();
        SelectedCity = null; // Reset previous city selection

        if (string.IsNullOrEmpty(SelectedCountry) || SelectedCountry == "All") return;

        // Find ID based on Name
        var countryObj = _allCountryObjects.FirstOrDefault(c => c.Name == SelectedCountry);
        if (countryObj == null) return;

        // Fetch cities
        var cities = await _lookupService.GetCitiesByCountryAsync(countryObj.Id);

        Cities.Add("All");
        foreach (var city in cities) Cities.Add(city);
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

        // Updated City Logic: Exact match on selection
        if (SelectedCity != "All" && !string.IsNullOrEmpty(SelectedCity))
            query = query.Where(x => x.City == SelectedCity);

        if (!string.IsNullOrWhiteSpace(PostalCodeFilter))
            query = query.Where(x => x.PostalCode.ToLower().Contains(PostalCodeFilter.ToLower()));

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
        SelectedCity = null;
        PostalCodeFilter = string.Empty;
        ApplyFilters();
    }

    [RelayCommand]
    private async Task OpenFilters()
    {
        await Shell.Current.Navigation.PushModalAsync(new FilterPage(this));
    }

    
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