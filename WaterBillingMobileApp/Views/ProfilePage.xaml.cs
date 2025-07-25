using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(AuthService authService, INavigation navigation)
    {
        InitializeComponent();
        BindingContext = new ProfileViewModel(authService, navigation);
    }
}
