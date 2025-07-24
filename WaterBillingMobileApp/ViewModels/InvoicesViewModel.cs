using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class InvoicesViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private HttpClient _httpClient;

        public InvoicesViewModel(INavigation navigation)
        {
            _navigation = navigation;
            LoadInvoicesCommand = new AsyncRelayCommand(LoadInvoicesAsync);
            _ = InitializeAsync();
        }

        [ObservableProperty]
        private ObservableCollection<InvoiceDTO> invoices = new();

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand LoadInvoicesCommand { get; }

        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await AuthService.CreateAuthenticatedClientAsync();
                await LoadInvoicesAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Authentication error: " + ex.Message, "OK");
            }
        }

        private async Task LoadInvoicesAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var invoicesList = await _httpClient.GetFromJsonAsync<List<InvoiceDTO>>("Customer/invoices");
                Invoices = new ObservableCollection<InvoiceDTO>(invoicesList);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load invoices: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
