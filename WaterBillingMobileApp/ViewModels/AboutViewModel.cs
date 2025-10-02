using CommunityToolkit.Mvvm.ComponentModel;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the About page.
    /// Provides application metadata including version, developer information, and creation date.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm for property change notifications.
    /// </summary>
    public partial class AboutViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets the current version of the application.
        /// Displayed in the AboutPage to inform users of the app version.
        /// </summary>
        [ObservableProperty]
        private string appVersion = "1.0";

        /// <summary>
        /// Gets or sets the name of the developer who created the application.
        /// </summary>
        [ObservableProperty]
        private string developer = "Álvaro Neves";

        /// <summary>
        /// Gets or sets the creation date of the application.
        /// Formatted as dd/MM/yyyy.
        /// </summary>
        [ObservableProperty]
        private string creationDate = "17/09/2025";
    }
}