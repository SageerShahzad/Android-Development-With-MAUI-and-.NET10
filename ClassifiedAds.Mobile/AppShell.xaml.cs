using ClassifiedAds.Mobile.Views;

namespace ClassifiedAds.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
       
        Routing.RegisterRoute(nameof(AdDetailPage), typeof(AdDetailPage));
    
        Routing.RegisterRoute(nameof(EditProfilePage), typeof(EditProfilePage));

        Routing.RegisterRoute(nameof(MessageThreadPage), typeof(MessageThreadPage));
    }
}