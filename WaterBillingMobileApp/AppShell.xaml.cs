using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp;

/// <summary>
/// Shell container for application navigation.
/// Registers routes and provides navigation commands for authenticated and anonymous features.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppShell"/> class.
    /// Registers all navigation routes for the application.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();

        // Register navigation routes for all pages
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(MeterReadingPage), typeof(MeterReadingPage));
        Routing.RegisterRoute(nameof(ConsumptionHistoryPage), typeof(ConsumptionHistoryPage));
        Routing.RegisterRoute(nameof(InvoicesPage), typeof(InvoicesPage));
        Routing.RegisterRoute(nameof(RatesAndStatusPage), typeof(RatesAndStatusPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
        Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
        Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));

        BindingContext = this;
    }

    /// <summary>
    /// Gets the command to navigate to the anonymous meter request page.
    /// Allows non-authenticated users to request meter installation.
    /// </summary>
    public Command GoToAnonymousRequestCommand => new Command(async () =>
    {
        try
        {
            await Shell.Current.Navigation.PushAsync(new AnonymousRequestPage());
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
        }
    });
}