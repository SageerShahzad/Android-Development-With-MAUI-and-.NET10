using ClassifiedAds.Mobile.Views;

namespace ClassifiedAds.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register any routes for pages not in the TabBar (e.g., SearchPage)
        // Routing.RegisterRoute(nameof(FilterPage), typeof(FilterPage));

        Routing.RegisterRoute(nameof(AdDetailPage), typeof(AdDetailPage));

        Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));

        Routing.RegisterRoute(nameof(MessageThreadPage), typeof(MessageThreadPage));



    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        // Option A: Navigate to a dedicated Search Page
        // await GoToAsync(nameof(FilterPage));

        // Option B: For now, we can show an alert or navigate to Home
        await DisplayAlert("Search", "Global Search Tapped", "OK");
    }

    private async void OnAccountClicked(object sender, EventArgs e)
    {
        // Navigate to the 'Account' tab defined in XAML Route="Account"
        // The "//" prefix indicates absolute routing (switching tabs)
        await Current.GoToAsync("//Account");
    }
}

