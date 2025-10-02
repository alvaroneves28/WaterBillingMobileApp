namespace WaterBillingMobileApp.Views
{
    using WaterBillingMobileApp.Interfaces;
    using WaterBillingMobileApp.ViewModels;

    /// <summary>
    /// Code-behind for the Consumption History page.
    /// Displays historical water consumption records for the authenticated user's meters.
    /// </summary>
    public partial class ConsumptionHistoryPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumptionHistoryPage"/> class.
        /// </summary>
        /// <param name="navigation">The navigation service for page navigation.</param>
        /// <param name="authService">The authentication service for authenticated API access.</param>
        public ConsumptionHistoryPage(IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new ConsumptionHistoryViewModel(authService);
        }
    }
}