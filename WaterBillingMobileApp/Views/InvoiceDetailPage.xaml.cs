using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Invoice Detail page.
/// Displays comprehensive details of a specific invoice including consumption data,
/// tariff breakdown, and payment information.
/// </summary>
public partial class InvoiceDetailPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceDetailPage"/> class.
    /// </summary>
    /// <param name="invoice">The invoice summary to display detailed information for.</param>
    public InvoiceDetailPage(InvoiceDTO invoice)
    {
        InitializeComponent();
        BindingContext = new InvoiceDetailViewModel(invoice);
    }
}