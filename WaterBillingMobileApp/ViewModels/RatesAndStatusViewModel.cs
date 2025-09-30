using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Services;
using System.Net.Http.Headers;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class RatesAndStatusViewModel : ObservableObject
    {
        private HttpClient _httpClient;

        [ObservableProperty]
        private ObservableCollection<MeterStatusDTO> meters = new();

        [ObservableProperty]
        private ObservableCollection<TariffDTO> tariffs = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool hasMeters;

        [ObservableProperty]
        private bool hasTariffs;

        [ObservableProperty]
        private string errorMessage;

        public IAsyncRelayCommand LoadDataCommand { get; }

        private readonly AuthService _authService;
        private readonly INavigation _navigation;

        public RatesAndStatusViewModel(AuthService authService, INavigation navigation)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

            // Inicializar de forma assíncrona
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                // CORRIGIDO: underscore em vez de asterisco
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

        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                // Carregar Meters (autenticado)
                await LoadMetersAsync();

                // Carregar Tariffs (público - não requer autenticação)
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

                    // Se não houver meters, não é necessariamente um erro crítico
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
                throw; // Re-throw para ser capturado pelo LoadDataAsync
            }
        }

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
                throw; // Re-throw para ser capturado pelo LoadDataAsync
            }
            finally
            {
                anonClient?.Dispose();
            }
        }

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