using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainPageViewModel(Navigation); 
    }
}
