using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

public partial class MeterReadingPage : ContentPage
{
    public MeterReadingPage(SubmitReadingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}