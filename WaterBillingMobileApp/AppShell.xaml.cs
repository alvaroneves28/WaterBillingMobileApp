using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rotas
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
