using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Rates and Status page.
    /// Displays water tariff brackets and meter installation request statuses.
    /// Handles both authenticated (meter status) and public (tariffs) data.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm.
    /// </summary>
    public partial class RatesAndStatusViewModel : ObservableObject
    {
        /// <summary>
        /// Authenticated HTTP client for making API requests.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// Gets or sets the collection of meter installation requests and their statuses.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MeterStatusDTO> meters = new();

        /// <summary>
        /// Gets or sets the collection of water tariff brackets.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<TariffDTO> tariffs = new();

        /// <summary>
        /// Gets or sets a value indicating whether a data loading operation is in progress.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether the user has any meters.
        /// Used to show/hide the "no meters" message.
        /// </summary>
        [ObservableProperty]
        private bool hasMeters;

        /// <summary>
        /// Gets or sets a value indicating whether tariff data was loaded successfully.
        /// Used to show/hide the "no tariffs" message.
        /// </summary>
        [ObservableProperty]
        private bool hasTariffs;

        /// <summary>
        /// Gets or sets an error message to display when data loading fails.
        /// </summary>
        [ObservableProperty]
        private string errorMessage;

        /// <summary>
        /// Gets the command to reload all data (meters and tariffs).
        /// </summary>
        public IAsyncRelayCommand LoadDataCommand { get; }

        /// <summary>
        /// Authentication service for creating authenticated HTTP clients.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RatesAndStatusViewModel"/> class.
        /// Sets up commands and initiates asynchronous data loading.
        /// </summary>
        /// <param name="authService">The authentication service.</param>
        /// <param name="navigation">The navigation service.</param>
        /// <exception cref="ArgumentNullException">Thrown when authService or navigation is null.</exception>
        public RatesAndStatusViewModel(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

            // Initialize asynchronously
            _ = InitializeAsync();
        }

        /// <summary>
        /// Initializes the authenticated HTTP client and loads initial data.
        /// Displays an error alert if authentication fails.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await _authService.CreateAuthenticatedClientAsync();
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Initialization error: {ex.Message}");
                ErrorMessage = "Authentication error. Please login again.";
                await ShowAlert("Error", "Authentication error: " + ex.Message, "OK");
            }
        }

        /// <summary>
        /// Loads both meter status data and tariff data.
        /// Clears any previous error messages before loading.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Load meters (authenticated)
                await LoadMetersAsync();

                // Load tariffs (public - no authentication required)
                await LoadTariffsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadData error: {ex.Message}");
                ErrorMessage = "Failed to load data. Please try again.";
                await ShowAlert("Error", "Failed to load data: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Loads meter status data for the authenticated user.
        /// Displays warnings if meters cannot be loaded but handles "not found" gracefully.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadMetersAsync()
        {
            try
            {
                if (_httpClient == null)
                {
                    System.Diagnostics.Debug.WriteLine("HttpClient is null, recreating...");
                    _httpClient = await _authService.CreateAuthenticatedClientAsync();
                }

                var metersResponse = await _httpClient.GetAsync("Customer/meters/status");

                if (metersResponse.IsSuccessStatusCode)
                {
                    var meterList = await metersResponse.Content.ReadFromJsonAsync<List<MeterStatusDTO>>();

                    if (meterList != null && meterList.Any())
                    {
                        Meters = new ObservableCollection<MeterStatusDTO>(meterList);
                        HasMeters = true;
                        System.Diagnostics.Debug.WriteLine($"Loaded {meterList.Count} meters");
                    }
                    else
                    {
                        Meters = new ObservableCollection<MeterStatusDTO>();
                        HasMeters = false;
                        System.Diagnostics.Debug.WriteLine("No meters found");
                    }
                }
                else
                {
                    var error = await metersResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Failed to load meters: {metersResponse.StatusCode} - {error}");

                    HasMeters = false;

                    // Not having meters is not necessarily a critical error
                    if (metersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        System.Diagnostics.Debug.WriteLine("No meters available for this user");
                    }
                    else
                    {
                        await ShowAlert("Warning",
                            $"Could not load meter status: {metersResponse.StatusCode}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception loading meters: {ex.Message}");
                HasMeters = false;
                throw; // Re-throw to be caught by LoadDataAsync
            }
        }

        /// <summary>
        /// Loads publicly available tariff data.
        /// Creates a new unauthenticated HTTP client for this public endpoint.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadTariffsAsync()
        {
            HttpClient anonClient = null;

            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                anonClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://10.0.2.2:44328/api/"),
                    Timeout = TimeSpan.FromSeconds(30)
                };

                anonClient.DefaultRequestHeaders.Accept.Clear();
                anonClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                System.Diagnostics.Debug.WriteLine("Loading tariffs...");
                var tariffsResponse = await anonClient.GetAsync("Customer/tariff-brackets");

                if (tariffsResponse.IsSuccessStatusCode)
                {
                    var tariffList = await tariffsResponse.Content.ReadFromJsonAsync<List<TariffDTO>>();

                    if (tariffList != null && tariffList.Any())
                    {
                        Tariffs = new ObservableCollection<TariffDTO>(tariffList);
                        HasTariffs = true;
                        System.Diagnostics.Debug.WriteLine($"Loaded {tariffList.Count} tariffs");
                    }
                    else
                    {
                        Tariffs = new ObservableCollection<TariffDTO>();
                        HasTariffs = false;
                        System.Diagnostics.Debug.WriteLine("No tariffs found");
                    }
                }
                else
                {
                    var error = await tariffsResponse.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Failed to load tariffs: {tariffsResponse.StatusCode} - {error}");

                    HasTariffs = false;
                    await ShowAlert("Error",
                        $"Failed to load tariffs: {tariffsResponse.StatusCode}\n{error}", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception loading tariffs: {ex.Message}");
                HasTariffs = false;
                throw; // Re-throw to be caught by LoadDataAsync
            }
            finally
            {
                anonClient?.Dispose();
            }
        }

        /// <summary>
        /// Displays an alert dialog with the specified title, message, and button text.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message content.</param>
        /// <param name="button">The button text.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShowAlert(string title, string message, string button)
        {
            try
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert(title, message, button);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Alert: {title} - {message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ShowAlert error: {ex.Message}");
            }
        }
    }
}