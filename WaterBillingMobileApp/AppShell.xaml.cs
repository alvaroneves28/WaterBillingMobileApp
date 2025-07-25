using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();


        Routing.RegisterRoute(nameof(SubmitReadingPage), typeof(Views.SubmitReadingPage));
        Routing.RegisterRoute(nameof(ConsumptionHistoryPage), typeof(Views.ConsumptionHistoryPage));
        Routing.RegisterRoute(nameof(InvoicesPage), typeof(Views.InvoicesPage));
        Routing.RegisterRoute(nameof(RatesAndStatusPage), typeof(Views.RatesAndStatusPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(Views.ProfilePage));
        //Routing.RegisterRoute(nameof(AboutPage), typeof(Views.AboutPage));
    }
}
