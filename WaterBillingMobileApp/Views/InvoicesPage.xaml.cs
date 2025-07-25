using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views
{
    public partial class InvoicesPage : ContentPage
    {
        public InvoicesPage(INavigation navigation, AuthService authService)
        {
            InitializeComponent();
            BindingContext = new InvoicesViewModel(navigation, authService);
        }
    }

}
