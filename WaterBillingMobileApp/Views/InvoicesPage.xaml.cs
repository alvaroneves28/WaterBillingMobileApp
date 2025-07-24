using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views
{
    public partial class InvoicesPage : ContentPage
    {
        public InvoicesPage()
        {
            InitializeComponent();
            BindingContext = new InvoicesViewModel(Navigation);
        }
    }
}
