using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
        BindingContext = new ForgotPasswordViewModel();
    }
}