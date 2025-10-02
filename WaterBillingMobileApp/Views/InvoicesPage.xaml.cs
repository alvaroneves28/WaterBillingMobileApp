using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views
{
    /// <summary>
    /// Code-behind for the Invoices page.
    /// Displays a list of invoices for the authenticated user with options to view detailed information.
    /// </summary>
    public partial class InvoicesPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesPage"/> class.
        /// </summary>
        /// <param name="navigation">The navigation service for page navigation.</param>
        /// <param name="authService">The authentication service for authenticated API access.</param>
        public InvoicesPage(IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new InvoicesViewModel(authService);
        }
    }
}