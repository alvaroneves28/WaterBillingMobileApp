namespace WaterBillingMobileApp.Views
{
    using WaterBillingMobileApp.Interfaces;
    using WaterBillingMobileApp.ViewModels;

    public partial class ConsumptionHistoryPage : ContentPage
    {
        public ConsumptionHistoryPage(INavigation navigation, IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new ConsumptionHistoryViewModel(navigation, authService);
        }
    }

}
