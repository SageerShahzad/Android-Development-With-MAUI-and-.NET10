using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile.Views;

public partial class MessageThreadPage : ContentPage
{
    public MessageThreadPage(MessageThreadViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is MessageThreadViewModel vm)
        {
            await vm.OnDisappearing();
        }
    }
}