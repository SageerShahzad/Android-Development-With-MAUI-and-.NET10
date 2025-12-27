using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ClassifiedAds.Mobile
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,  LaunchMode = LaunchMode.SingleTop,
        WindowSoftInputMode = Android.Views.SoftInput.AdjustResize,
          ConfigurationChanges = ConfigChanges.ScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}


//, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)