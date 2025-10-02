using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class InvoicesViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private readonly IAuthService _authService;
        private HttpClient _httpClient;

        public InvoicesViewModel(INavigation navigation, IAuthService authService)
        {
            _navigation = navigation;
            _authService = authService;

            LoadInvoicesCommand = new AsyncRelayCommand(LoadInvoicesAsync);
            ViewInvoiceDetailCommand = new AsyncRelayCommand<InvoiceDTO>(ViewInvoiceDetailAsync);
            _ = InitializeAsync();
        }

        [ObservableProperty]
        private ObservableCollection<InvoiceDTO> invoices = new();

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand<InvoiceDTO> ViewInvoiceDetailCommand { get; }

        public IAsyncRelayCommand LoadInvoicesCommand { get; }

        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await _authService.CreateAuthenticatedClientAsync(); // Usar instância
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
                Invoices = new ObservableCollection<InvoiceDTO>(invoicesList ?? new());
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

        private async Task ViewInvoiceDetailAsync(InvoiceDTO invoice)
        {
            if (invoice == null) return;

            try
            {
                await Shell.Current.Navigation.PushAsync(new Views.InvoiceDetailPage(invoice));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
        }
    }
}
