using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp;

public partial class MainPage : ContentPage
{
    private readonly AuthService _authService;

    public MainPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        BindingContext = new MainPageViewModel(Navigation, _authService);
    }
}
