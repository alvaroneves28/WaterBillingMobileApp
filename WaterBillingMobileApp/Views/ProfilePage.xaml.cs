using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(IAuthService authService, INavigation navigation)
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel(authService, navigation);
    }
}
