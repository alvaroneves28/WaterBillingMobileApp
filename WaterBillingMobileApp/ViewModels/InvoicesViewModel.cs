using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Invoices page.
    /// Handles loading and displaying a list of invoices for the authenticated user.
    /// Provides functionality to view detailed information for individual invoices.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm with authenticated API access.
    /// </summary>
    public partial class InvoicesViewModel : ObservableObject
    {
        /// <summary>
        /// Authentication service for creating authenticated HTTP clients and verifying user login status.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Authenticated HTTP client for making API requests.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoicesViewModel"/> class.
        /// Sets up commands and initiates asynchronous initialization of the authenticated HTTP client.
        /// </summary>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="authService">The authentication service.</param>
        public InvoicesViewModel(IAuthService authService)
        {
            _authService = authService;
            LoadInvoicesCommand = new AsyncRelayCommand(LoadInvoicesAsync);
            ViewInvoiceDetailCommand = new AsyncRelayCommand<InvoiceDTO>(ViewInvoiceDetailAsync);
            _ = InitializeAsync();
        }

        /// <summary>
        /// Gets or sets the collection of invoices for the authenticated user.
        /// Displayed in the UI as a list of invoice summaries.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<InvoiceDTO> invoices = new();

        /// <summary>
        /// Gets or sets a value indicating whether a data loading operation is in progress.
        /// Used to show loading indicators in the UI.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets the command to view detailed information for a specific invoice.
        /// </summary>
        public IAsyncRelayCommand<InvoiceDTO> ViewInvoiceDetailCommand { get; }

        /// <summary>
        /// Gets the command to load invoices from the API.
        /// </summary>
        public IAsyncRelayCommand LoadInvoicesCommand { get; }

        /// <summary>
        /// Initializes the authenticated HTTP client and loads invoice data.
        /// Displays an error alert if authentication fails.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await _authService.CreateAuthenticatedClientAsync();
                await LoadInvoicesAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Authentication error: " + ex.Message, "OK");
            }
        }

        /// <summary>
        /// Loads invoice data from the API.
        /// Retrieves all invoices for the authenticated user and updates the Invoices collection.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Navigates to the invoice detail page for the selected invoice.
        /// </summary>
        /// <param name="invoice">The invoice to view details for.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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