using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Forgot Password page.
/// Allows users to request password recovery instructions via email.
/// </summary>
public partial class ForgotPasswordPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordPage"/> class.
    /// Sets up the ForgotPasswordViewModel as the binding context.
    /// </summary>
    public ForgotPasswordPage()
    {
        InitializeComponent();
        BindingContext = new ForgotPasswordViewModel();
    }
}