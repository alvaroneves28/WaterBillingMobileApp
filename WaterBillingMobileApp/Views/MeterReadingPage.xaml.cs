using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the Meter Reading page.
/// Allows authenticated users to submit new water consumption readings for their meters.
/// </summary>
public partial class MeterReadingPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MeterReadingPage"/> class.
    /// </summary>
    /// <param name="viewModel">The SubmitReadingViewModel containing meter data and submission logic.</param>
    public MeterReadingPage(SubmitReadingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}