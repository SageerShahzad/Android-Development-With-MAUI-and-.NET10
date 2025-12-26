using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views;

public partial class EditProfilePage : ContentPage
{
    public EditProfilePage(EditProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}