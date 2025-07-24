using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly INavigation _navigation;

        public MainPageViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }

        [RelayCommand]
        private async Task SubmitReading()
        {
            await _navigation.PushAsync(new SubmitReadingPage());
        }

        [RelayCommand]
        private async Task ViewConsumptionHistory()
        {
            await _navigation.PushAsync(new ConsumptionHistoryPage());
        }

        [RelayCommand]
        private async Task ViewInvoices()
        {
            await _navigation.PushAsync(new InvoicesPage());
        }

        //[RelayCommand]
        //private async Task ViewRatesAndStatus()
        //{
        //    await _navigation.PushAsync(new RatesAndStatusPage());
        //}

        //[RelayCommand]
        //private async Task ViewProfile()
        //{
        //    await _navigation.PushAsync(new ProfilePage());
        //}

        //[RelayCommand]
        //private async Task ViewAbout()
        //{
        //    await _navigation.PushAsync(new AboutPage());
        //}
    }
}
