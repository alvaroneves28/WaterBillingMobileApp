using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class RatesAndStatusPage : ContentPage
{
    public RatesAndStatusPage(AuthService authService, INavigation navigation)
    {
        InitializeComponent();

        BindingContext = new RatesAndStatusViewModel(authService, navigation);

        if (BindingContext is RatesAndStatusViewModel vm)
        {
            _ = vm.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
