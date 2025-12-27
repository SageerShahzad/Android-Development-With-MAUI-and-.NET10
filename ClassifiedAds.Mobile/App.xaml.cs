using ClassifiedAds.Mobile.ViewModels;
using ClassifiedAds.Mobile.Views;

namespace ClassifiedAds.Mobile
{
    public partial class App : Application
    {
        private readonly UserAuthViewModel _authViewModel;

        public App(UserAuthViewModel authViewModel)
        {
            InitializeComponent();
            _authViewModel = authViewModel;

            // FIX: Start with a Loading Page instead of AppShell
            // This prevents the Chat from loading with a null UserID
            MainPage = new LoadingPage();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            // CRITICAL: Await the ID restoration here
            await _authViewModel.InitializeAsync();

            // Once finished, switch to the Main App
            MainPage = new AppShell();
        }
    }
}