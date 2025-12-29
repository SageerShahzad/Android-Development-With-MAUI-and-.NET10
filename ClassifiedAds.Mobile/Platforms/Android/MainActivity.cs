using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;  // Required for SoftInput

namespace ClassifiedAds.Mobile
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateHidden,  // StateHidden prevents auto-popup on load
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}