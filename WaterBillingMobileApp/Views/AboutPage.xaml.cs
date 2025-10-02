using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class AboutPage : ContentPage
{
	public AboutPage()
	{
		InitializeComponent();
        BindingContext = new AboutViewModel();
    }
}