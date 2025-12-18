using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClassifiedAds.Mobile.ViewModels;

public class CategoryItem
{
    public string Title { get; set; }
    public string ImageSource { get; set; }
}

// Partial is REQUIRED for source generators to work
public partial class CategoriesViewModel : BaseViewModel
{
    // 1. Define lowercase field with attribute
    [ObservableProperty]
    bool isSearching; // Generator creates public bool IsSearching

    [ObservableProperty]
    string searchText; // Generator creates public string SearchText

    public ObservableCollection<CategoryItem> Categories { get; set; }
    public ObservableCollection<string> SearchResults { get; set; }

    public CategoriesViewModel()
    {
        Categories = new ObservableCollection<CategoryItem>
        {
            new CategoryItem { Title = "Brake System", ImageSource = "dotnet_bot.png" },
            new CategoryItem { Title = "Filters", ImageSource = "dotnet_bot.png" },
            new CategoryItem { Title = "Engine", ImageSource = "dotnet_bot.png" },
            new CategoryItem { Title = "Oils and Fluids", ImageSource = "dotnet_bot.png" },
            new CategoryItem { Title = "Suspension", ImageSource = "dotnet_bot.png" },
            new CategoryItem { Title = "Body", ImageSource = "dotnet_bot.png" }
        };

        SearchResults = new ObservableCollection<string>();
    }

    [RelayCommand]
    void StartSearch()
    {
        IsSearching = true; // Now this property exists
        SearchResults.Clear();
        SearchResults.Add("Brake pads");
        SearchResults.Add("Oil filter 5w30");
    }

    [RelayCommand]
    void CancelSearch()
    {
        IsSearching = false;
        SearchText = string.Empty;
    }

    [RelayCommand]
    void PerformSearch(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return;
        SearchResults.Clear();
        SearchResults.Add($"Result for '{query}'");
    }

    [RelayCommand]
    void PostAd()
    {
        Console.WriteLine("Navigating to Post Ad");
    }
}