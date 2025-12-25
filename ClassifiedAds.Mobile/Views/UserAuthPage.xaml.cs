using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views;

public partial class UserAuthPage : ContentPage
{
    private readonly UserAuthViewModel _viewModel;

    public UserAuthPage(UserAuthViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Check auth status every time the page appears
        if (_viewModel != null)
        {
            await _viewModel.InitializeAsync();
        }
    }
}