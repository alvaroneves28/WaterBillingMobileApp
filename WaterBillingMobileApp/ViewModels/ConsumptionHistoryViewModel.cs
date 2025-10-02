using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Consumption History page.
    /// Handles loading and displaying historical water consumption data for the authenticated user.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm with authenticated API access.
    /// </summary>
    public partial class ConsumptionHistoryViewModel : ObservableObject
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
        /// Initializes a new instance of the <see cref="ConsumptionHistoryViewModel"/> class.
        /// Sets up commands and initiates asynchronous initialization of the authenticated HTTP client.
        /// </summary>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="authService">The authentication service.</param>
        public ConsumptionHistoryViewModel(IAuthService authService)
        {
            _authService = authService;
            LoadHistoryCommand = new AsyncRelayCommand(LoadHistoryAsync);
            _ = InitializeAsync();
        }

        /// <summary>
        /// Gets or sets the collection of consumption history records.
        /// Displayed in the UI as a list of historical meter readings.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<ConsumptionHistoryDTO> consumptionHistory = new();

        /// <summary>
        /// Gets or sets a value indicating whether a data loading operation is in progress.
        /// Used to show loading indicators in the UI.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets the command to load consumption history data from the API.
        /// </summary>
        public IAsyncRelayCommand LoadHistoryCommand { get; }

        /// <summary>
        /// Initializes the authenticated HTTP client and loads consumption history data.
        /// Displays an error alert if authentication fails.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await _authService.CreateAuthenticatedClientAsync();
                await LoadHistoryAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Authentication error: " + ex.Message, "OK");
            }
        }

        /// <summary>
        /// Loads consumption history data from the API.
        /// Retrieves all historical consumption records for the authenticated user's meters.
        /// Updates the ConsumptionHistory collection with the retrieved data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadHistoryAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                var history = await _httpClient.GetFromJsonAsync<List<ConsumptionHistoryDTO>>("Customer/consumptions/history");
                ConsumptionHistory = new ObservableCollection<ConsumptionHistoryDTO>(history);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load consumption history: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}