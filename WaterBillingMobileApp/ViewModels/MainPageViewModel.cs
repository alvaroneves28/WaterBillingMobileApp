using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterBillingMobileApp.Views;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private readonly AuthService _authService;

        public MainPageViewModel(INavigation navigation, AuthService authService)
        {
            _navigation = navigation;
            _authService = authService;
        }

        [RelayCommand]
        private async Task SubmitReading()
        {
            await _navigation.PushAsync(new SubmitReadingPage(_navigation, _authService));
        }

        [RelayCommand]
        private async Task ViewConsumptionHistory()
        {
            await _navigation.PushAsync(new ConsumptionHistoryPage(_navigation, _authService));
        }

        [RelayCommand]
        private async Task ViewInvoices()
        {
            await _navigation.PushAsync(new InvoicesPage(_navigation, _authService));
        }

        [RelayCommand]
        private async Task ViewRatesAndStatus()
        {
            await _navigation.PushAsync(new RatesAndStatusPage(_authService, _navigation));
        }

        [RelayCommand]
        private async Task ViewProfile()
        {
            await _navigation.PushAsync(new ProfilePage(_authService, _navigation));
        }
    }
}
