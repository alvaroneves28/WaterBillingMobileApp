using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class SubmitReadingPage : ContentPage
{
    public SubmitReadingPage()
    {
        InitializeComponent();
        BindingContext = new SubmitReadingViewModel(Navigation);
    }
}

