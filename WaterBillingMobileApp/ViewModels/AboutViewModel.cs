using CommunityToolkit.Mvvm.ComponentModel;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string appVersion = "1.0";

        [ObservableProperty]
        private string developer = "Álvaro Neves";

        [ObservableProperty]
        private string creationDate = "17/09/2025";
    }
}
