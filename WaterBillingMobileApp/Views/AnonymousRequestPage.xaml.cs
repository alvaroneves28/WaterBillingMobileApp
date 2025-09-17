using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class AnonymousRequestPage : ContentPage
{
    public AnonymousRequestPage()
    {
        InitializeComponent();
        BindingContext = new AnonymousRequestViewModel();
    }
}