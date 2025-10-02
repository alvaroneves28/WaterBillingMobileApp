using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp;

public partial class MainPage : ContentPage
{
    private readonly IAuthService _authService;

    public MainPage(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        BindingContext = new MainPageViewModel(Navigation, _authService);
    }
}
