using ClassifiedAds.Mobile.Views;

namespace ClassifiedAds.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Allow navigation from List -> Detail
        Routing.RegisterRoute(nameof(AdDetailPage), typeof(AdDetailPage));
        // ADD THIS LINE
        Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));
    }
}