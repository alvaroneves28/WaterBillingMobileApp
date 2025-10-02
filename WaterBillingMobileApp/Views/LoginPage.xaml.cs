using WaterBillingMobileApp.ViewModel;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Login page.
/// Handles user authentication and provides navigation to password recovery and meter request pages.
/// </summary>
public partial class LoginPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// Sets up the LoginViewModel as the binding context.
    /// </summary>
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}