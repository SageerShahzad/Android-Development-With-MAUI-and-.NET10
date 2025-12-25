using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views;

public partial class AdsPage : ContentPage
{
    public AdsPage(AdsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}