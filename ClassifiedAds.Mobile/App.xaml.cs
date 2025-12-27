using ClassifiedAds.Mobile.ViewModels;

namespace ClassifiedAds.Mobile
{
    public partial class App : Application
    {
        // Inject the ViewModel into the App constructor
        public App(UserAuthViewModel authViewModel)
        {
            InitializeComponent();

            MainPage = new AppShell();

            // CRITICAL FIX: Initialize Auth State immediately on App Start
            // This ensures CurrentUserId is set before the user navigates anywhere.
            authViewModel.InitializeAsync();
        }
    }
}