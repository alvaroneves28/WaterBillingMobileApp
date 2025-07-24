namespace WaterBillingMobileApp.Views
{
    using WaterBillingMobileApp.ViewModels;

    public partial class ConsumptionHistoryPage : ContentPage
    {
        public ConsumptionHistoryPage()
        {
            InitializeComponent();
            BindingContext = new ConsumptionHistoryViewModel(Navigation);
        }
    }
}
