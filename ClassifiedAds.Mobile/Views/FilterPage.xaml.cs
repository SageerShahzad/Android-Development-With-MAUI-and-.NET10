using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views;

public partial class FilterPage : ContentPage
{
    private readonly AdsViewModel _viewModel;

    public FilterPage(AdsViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = _viewModel;
    }

    private async void OnApplyClicked(object sender, EventArgs e)
    {
        // Execute the ApplyFilter logic in the ViewModel
        _viewModel.ApplyFiltersCommand.Execute(null);

        // Close the popup
        await Navigation.PopModalAsync();
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        // Close the popup without applying
        await Navigation.PopModalAsync();
    }
}