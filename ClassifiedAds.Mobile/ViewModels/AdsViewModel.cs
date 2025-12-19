using ClassifiedAds.Mobile.Dtos;
using ClassifiedAds.Mobile.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClassifiedAds.Mobile.ViewModels
{
    public partial class AdsViewModel : ObservableObject
    {
        private readonly AdsService _adsService;

        [ObservableProperty]
        private bool _isLoading;

        // Collection bound to the UI
        public ObservableCollection<AdDTO> Ads { get; } = new();

        public AdsViewModel(AdsService adsService)
        {
            _adsService = adsService;
        }

        [RelayCommand]
        public async Task LoadAdsAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Ads.Clear();

                var adsList = await _adsService.GetAdsAsync();

                foreach (var ad in adsList)
                {
                    // Basic handling for empty images if null
                    if (string.IsNullOrEmpty(ad.MainImageUrl))
                        ad.MainImageUrl = "placeholder_image.png";

                    Ads.Add(ad);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}