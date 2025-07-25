namespace WaterBillingMobileApp.Views
{
    using WaterBillingMobileApp.Services;
    using WaterBillingMobileApp.ViewModels;

    public partial class ConsumptionHistoryPage : ContentPage
    {
        public ConsumptionHistoryPage(INavigation navigation, AuthService authService)
        {
            InitializeComponent();
            BindingContext = new ConsumptionHistoryViewModel(navigation, authService);
        }
    }

}
