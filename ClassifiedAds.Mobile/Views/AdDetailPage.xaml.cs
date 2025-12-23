using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views;

public partial class AdDetailPage : ContentPage
{
    // Constructor Injection
    public AdDetailPage(AdDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}