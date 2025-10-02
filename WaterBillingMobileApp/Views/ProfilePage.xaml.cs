using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Profile page.
/// Allows users to view and edit their profile information including personal details,
/// email, password, and profile photo.
/// </summary>
public partial class ProfilePage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProfilePage"/> class.
    /// </summary>
    /// <param name="authService">The authentication service for authenticated API access.</param>
    /// <param name="navigation">The navigation service for page navigation.</param>
    public ProfilePage(IAuthService authService)
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel(authService);
    }
}