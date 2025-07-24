using WaterBillingMobileApp.ViewModel;

namespace WaterBillingMobileApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginViewModel();
    }
}
