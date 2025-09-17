using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class InvoiceDetailPage : ContentPage
{
    public InvoiceDetailPage(InvoiceDTO invoice)
    {
        InitializeComponent();
        BindingContext = new InvoiceDetailViewModel(invoice);
    }
}