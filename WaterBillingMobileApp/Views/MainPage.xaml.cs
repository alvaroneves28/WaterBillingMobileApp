using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp;

/// <summary>
/// Code-behind for the Main Dashboard page.
/// Provides navigation to all major application features and displays new invoice notifications.
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// Authentication service for user session management and authenticated API access.
    /// </summary>
    private readonly IAuthService _authService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainPage"/> class.
    /// </summary>
    /// <param name="authService">The authentication service.</param>
    public MainPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
        BindingContext = new MainPageViewModel(Navigation, _authService);
    }
}