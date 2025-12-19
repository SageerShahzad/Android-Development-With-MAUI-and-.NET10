using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views
{
    public partial class AdsPage : ContentPage
    {
        private readonly AdsViewModel _viewModel;

        public AdsPage(AdsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Load data when page appears
            await _viewModel.LoadAdsAsync();
        }
    }
}