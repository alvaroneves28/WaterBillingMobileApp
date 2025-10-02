using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Reset Password page.
/// Handles password reset operations using a token received from a deep link.
/// Resolves the ResetPasswordService from dependency injection to retrieve token and email data.
/// </summary>
public partial class ResetPasswordPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordPage"/> class.
    /// Retrieves the ResetPasswordService from DI and initializes the ViewModel with reset token data.
    /// </summary>
    public ResetPasswordPage()
    {
        InitializeComponent();

        // Resolve service via dependency injection
        var service = (ResetPasswordService)App.Current.Handler.MauiContext.Services
                        .GetService(typeof(ResetPasswordService))!;

        // Associate ViewModel with service containing token and email
        BindingContext = new ResetPasswordViewModel(service);
    }
}