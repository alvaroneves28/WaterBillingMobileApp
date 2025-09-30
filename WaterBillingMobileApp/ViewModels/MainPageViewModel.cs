using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Services;
using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private readonly AuthService _authService;

        public IAsyncRelayCommand LogoutCommand { get; }

        public MainPageViewModel(INavigation navigation, AuthService authService)
        {
            _navigation = navigation;
            _authService = authService;
            _ = CheckForNewInvoicesAsync();
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
        }

        private async Task LogoutAsync()
        {
            try
            {
               
                Preferences.Remove("AuthToken"); 
                Preferences.Remove("UserEmail");
                Preferences.Remove("UserName");

                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logout failed: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Unable to logout. Please try again.", "OK");
            }
        }

        [ObservableProperty]
        private bool hasNewInvoices;

        [ObservableProperty]
        private int newInvoicesCount;

        private async Task CheckForNewInvoicesAsync()
        {
            try
            {
                var httpClient = await _authService.CreateAuthenticatedClientAsync();
                var unreadInvoices = await httpClient.GetFromJsonAsync<List<InvoiceDTO>>("Customer/invoices/unread");

                NewInvoicesCount = unreadInvoices?.Count ?? 0;
                HasNewInvoices = NewInvoicesCount > 0;
            }
            catch (Exception ex)
            {
                // Log do erro se necessário
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar novas faturas: {ex.Message}");
            }
        }


        [RelayCommand]
        private async Task SubmitReading()
        {
            var submitReadingViewModel = new SubmitReadingViewModel(_navigation, _authService);
            await _navigation.PushAsync(new MeterReadingPage(submitReadingViewModel));
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

        [RelayCommand]
        private async Task ViewAbout()
        {
            await _navigation.PushAsync(new AboutPage());
        }
    }
}
