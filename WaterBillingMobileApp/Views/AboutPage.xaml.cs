using WaterBillingMobileApp.ViewModels;

namespace WaterBillingMobileApp.Views;

/// <summary>
/// Code-behind for the About page.
/// Displays application information including version, author, and credits.
/// </summary>
public partial class AboutPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutPage"/> class.
    /// Sets up the AboutViewModel as the binding context.
    /// </summary>
    public AboutPage()
    {
        InitializeComponent();
        BindingContext = new AboutViewModel();
    }
}