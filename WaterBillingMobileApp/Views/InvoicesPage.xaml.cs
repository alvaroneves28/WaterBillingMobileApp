using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views
{
    public partial class InvoicesPage : ContentPage
    {
        public InvoicesPage(INavigation navigation, IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new InvoicesViewModel(navigation, authService);
        }
    }

}
