using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp
{
    /// <summary>
    /// Main application class for the Water Billing Mobile App.
    /// Handles application initialization and deep link navigation for password reset functionality.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Sets up the AppShell as the main page for navigation.
        /// </summary>
        public App()
        {
            InitializeComponent();

            // MainPage must always be AppShell for Shell navigation to work
            MainPage = new AppShell();
        }

        /// <summary>
        /// Handles deep link URLs from external sources (e.g., email password reset links).
        /// Parses the URL, extracts token and email, and navigates to the appropriate page.
        /// </summary>
        /// <param name="url">The deep link URL to handle.</param>
        /// <remarks>
        /// Currently supports the "reset-password" deep link for password recovery.
        /// Format: waterbilling://reset-password?token=TOKEN&email=EMAIL
        /// </remarks>
        public void HandleDeepLink(string url)
        {
            try
            {
                var uri = new Uri(url);

                // Handle password reset deep links
                if (uri.Host == "reset-password")
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    var token = query["token"];
                    var email = query["email"];

                    // Store token and email in the service for the ResetPasswordPage to access
                    var service = (ResetPasswordService)App.Current.Handler.MauiContext.Services.GetService(typeof(ResetPasswordService));
                    service.Token = token;
                    service.Email = email;

                    // Navigate to the ResetPasswordPage on the main thread
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        if (Shell.Current != null)
                            await Shell.Current.GoToAsync($"/{nameof(ResetPasswordPage)}");
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling deep link: {ex.Message}");
            }
        }
    }
}