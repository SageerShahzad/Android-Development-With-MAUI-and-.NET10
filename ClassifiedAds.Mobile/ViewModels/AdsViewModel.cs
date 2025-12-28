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
    private List<AdDTO> _allAdsBackup = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string searchText;

    // --- FILTER PROPERTIES ---
    [ObservableProperty]
    private string selectedCategory = "All";

    [ObservableProperty]
    private string selectedCountry = "All";

    [ObservableProperty]
    private decimal maxPriceFilter = 1000000; // Default high

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasFilterMessage))] // <--- ADD THIS LINE
    private string filterStatusMessage = "";

    // ADD THIS NEW PROPERTY
    public bool HasFilterMessage => !string.IsNullOrEmpty(FilterStatusMessage);

    // --- COLLECTIONS ---
    public ObservableCollection<AdDTO> Ads { get; } = new();
    public ObservableCollection<string> Categories { get; } = new();
    public ObservableCollection<string> Countries { get; } = new();

    public AdsViewModel(IAdService adService)
    {
        _adService = adService;
        // Load data immediately
        LoadAdsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadAds()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var adsList = await _adService.GetAds();
            _allAdsBackup = adsList ?? new List<AdDTO>();

            // 1. Extract Categories
            var cats = _allAdsBackup.Select(x => x.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c);
            Categories.Clear();
            Categories.Add("All");
            foreach (var c in cats) Categories.Add(c);

            // 2. Extract Countries (For the filter page)
            var countries = _allAdsBackup.Select(x => x.Country).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c);
            Countries.Clear();
            Countries.Add("All");
            foreach (var c in countries) Countries.Add(c);

            ApplyFilters();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void ApplyFilters()
    {
        FilterStatusMessage = ""; // Reset message

        // 1. Start with full list
        var query = _allAdsBackup.AsEnumerable();

        // 2. Apply Text Search
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var lower = SearchText.ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(lower) || x.City.ToLower().Contains(lower));
        }

        // 3. Apply Hard Filters (Category & Price)
        if (SelectedCategory != "All")
            query = query.Where(x => x.Category == SelectedCategory);

        if (MaxPriceFilter > 0)
            query = query.Where(x => (x.Price ?? 0) <= MaxPriceFilter);

        // 4. Apply Country Filter
        if (SelectedCountry != "All")
            query = query.Where(x => x.Country == SelectedCountry);

        var finalList = query.ToList();

        // --- SMART FALLBACK LOGIC ---
        // If the user selected a Country but got 0 results, we try to show them ads
        // from the same Category but in *other* countries.
        if (finalList.Count == 0 && SelectedCountry != "All")
        {
            FilterStatusMessage = $"No results in {SelectedCountry}. Showing similar items elsewhere.";

            // Re-run query ignoring country
            finalList = _allAdsBackup
                .Where(x => x.Category == SelectedCategory && (x.Price ?? 0) <= MaxPriceFilter)
                .ToList();
        }
        else if (finalList.Count == 0)
        {
            FilterStatusMessage = "No ads found matching your criteria.";
        }

        // 5. Update UI
        Ads.Clear();
        foreach (var ad in finalList) Ads.Add(ad);
    }

    [RelayCommand]
    private async Task OpenFilters()
    {
        // Navigate to the Filter Page
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