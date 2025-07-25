using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class SubmitReadingPage : ContentPage
{
    public SubmitReadingPage(INavigation navigation, AuthService authService)
    {
        InitializeComponent();
        BindingContext = new SubmitReadingViewModel(navigation, authService);
    }
}


