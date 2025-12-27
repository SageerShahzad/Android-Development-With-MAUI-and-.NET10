using ClassifiedAds.Mobile.ViewModels;
using System.ComponentModel;

namespace ClassifiedAds.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly UserAuthViewModel _viewModel;

    public LoginPage(UserAuthViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Subscribe to ViewModel changes
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Unsubscribe to prevent memory leaks
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // If the user successfully logs in, close this modal page automatically
        if (e.PropertyName == nameof(UserAuthViewModel.IsLoggedIn))
        {
            if (_viewModel.IsLoggedIn)
            {
                await Navigation.PopModalAsync();
            }
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}