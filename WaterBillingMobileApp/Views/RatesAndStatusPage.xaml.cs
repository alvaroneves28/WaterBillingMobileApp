using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Rates and Status page.
/// Displays water tariff brackets and meter installation request statuses.
/// Automatically loads data upon initialization.
/// </summary>
public partial class RatesAndStatusPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RatesAndStatusPage"/> class.
    /// Sets up the ViewModel and triggers data loading.
    /// </summary>
    /// <param name="authService">The authentication service for authenticated API access.</param>
    /// <param name="navigation">The navigation service for page navigation.</param>
    public RatesAndStatusPage(IAuthService authService)
    {
        InitializeComponent();
        BindingContext = new RatesAndStatusViewModel(authService);

        // Trigger data loading after ViewModel initialization
        if (BindingContext is RatesAndStatusViewModel vm)
        {
            _ = vm.LoadDataCommand.ExecuteAsync(null);
        }
    }
}