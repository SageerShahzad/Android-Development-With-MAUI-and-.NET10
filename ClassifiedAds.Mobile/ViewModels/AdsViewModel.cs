using ClassifiedAds.Mobile.Models;
using ClassifiedAds.Mobile.Services;
using ClassifiedAds.Mobile.Views; // Ensure this matches where AdDetailPage is
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClassifiedAds.Mobile.ViewModels;

public partial class AdsViewModel : ObservableObject
{
    private readonly IAdService _adService;

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<AdDTO> Ads { get; } = new();

    public AdsViewModel(IAdService adService)
    {
        _adService = adService;
        LoadAds();
    }

    [RelayCommand]
    private async Task LoadAds()
    {
        if (IsBusy) return;
        IsBusy = true;

        var adsList = await _adService.GetAds();

        Ads.Clear();
        foreach (var ad in adsList)
        {
            Ads.Add(ad);
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task GoToDetails(AdDTO ad)
    {
        if (ad == null) return;
        // Navigate to AdDetailPage and pass the "Id"
        await Shell.Current.GoToAsync($"{nameof(AdDetailPage)}?Id={ad.Id}");
    }
}