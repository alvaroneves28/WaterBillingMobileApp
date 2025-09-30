using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace WaterBillingMobileApp;

[Activity(Theme = "@style/Maui.SplashTheme",
          MainLauncher = true,
          LaunchMode = LaunchMode.SingleTop,
          ConfigurationChanges = ConfigChanges.ScreenSize |
                                ConfigChanges.Orientation |
                                ConfigChanges.UiMode |
                                ConfigChanges.ScreenLayout |
                                ConfigChanges.SmallestScreenSize |
                                ConfigChanges.Density,
    Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
              DataScheme = "waterbilling",
              DataHost = "reset-password")]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        HandleDeepLink(Intent);
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        Intent = intent;
        HandleDeepLink(intent);
    }

    private void HandleDeepLink(Intent intent)
    {
        var action = intent?.Action;
        var data = intent?.Data;

        if (action == Intent.ActionView && data != null)
        {
            var url = data.ToString();
            System.Diagnostics.Debug.WriteLine($"Deep link received: {url}");

            // Passar para o App processar
            MainThread.BeginInvokeOnMainThread(() =>
            {
                (Microsoft.Maui.Controls.Application.Current as App)?.HandleDeepLink(url);
            });
        }
    }
}