//using Android.App;
//using Android.Content.PM;
//using Android.OS;
//using Android.Views;

//namespace ClassifiedAds.Mobile
//{
//    [Activity(Theme = "@style/Maui.SplashTheme",
//              MainLauncher = true,
//              LaunchMode = LaunchMode.SingleTop,
//              // CRITICAL: Use AdjustPan OR AdjustResize, not both
//              WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden,
//              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
//                                    ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
//                                    ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
//    public class MainActivity : MauiAppCompatActivity
//    {
//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);

//            // Force the window to handle keyboard properly
//            Window?.SetSoftInputMode(SoftInput.AdjustPan);

//            // Optional: Make sure content is visible under status bar
//            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
//            {
//                Window?.SetStatusBarColor(Android.Graphics.Color.Transparent);
//                Window?.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
//                Window?.DecorView.SystemUiVisibility = (StatusBarVisibility)
//                    (SystemUiFlags.LayoutFullscreen | SystemUiFlags.LayoutStable);
//            }
//        }

//        protected override void OnResume()
//        {
//            base.OnResume();
//            // Re-apply on resume
//            Window?.SetSoftInputMode(SoftInput.AdjustPan);
//        }
//    }
//}


using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View; // Required for the fix

namespace ClassifiedAds.Mobile
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              // AdjustResize is required here
              WindowSoftInputMode = SoftInput.AdjustResize | SoftInput.StateHidden,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // THIS IS THE KEY FIX:
            // It forces the app to resize the content area when the keyboard pops up,
            // instead of drawing behind it.
            WindowCompat.SetDecorFitsSystemWindows(Window, true);
        }
    }
}