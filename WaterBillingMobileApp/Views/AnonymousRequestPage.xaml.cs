using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Anonymous Request page.
/// Allows non-authenticated users to request a new water meter installation.
/// Displays public tariff information and handles meter request form submission.
/// </summary>
public partial class AnonymousRequestPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousRequestPage"/> class.
    /// Sets up the AnonymousRequestViewModel as the binding context.
    /// </summary>
    public AnonymousRequestPage()
    {
        InitializeComponent();
        BindingContext = new AnonymousRequestViewModel();
    }
}