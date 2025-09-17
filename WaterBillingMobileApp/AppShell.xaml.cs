using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rotas
        Routing.RegisterRoute(nameof(MeterReadingPage), typeof(Views.MeterReadingPage));
        Routing.RegisterRoute(nameof(ConsumptionHistoryPage), typeof(Views.ConsumptionHistoryPage));
        Routing.RegisterRoute(nameof(InvoicesPage), typeof(Views.InvoicesPage));
        Routing.RegisterRoute(nameof(RatesAndStatusPage), typeof(Views.RatesAndStatusPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(Views.ProfilePage));
        Routing.RegisterRoute(nameof(AboutPage), typeof(Views.AboutPage));

        // Definir o BindingContext para os comandos
        BindingContext = this;
    }

    // Comando para navegar para a página de pedido anónimo
    public Command GoToAnonymousRequestCommand => new Command(async () =>
    {
        try
        {
            await Shell.Current.Navigation.PushAsync(new Views.AnonymousRequestPage());
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
        }
    });
}