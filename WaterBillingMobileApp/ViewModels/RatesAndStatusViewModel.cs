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

        public IAsyncRelayCommand LoadDataCommand { get; }

        private readonly AuthService _authService;
        private readonly INavigation _navigation;

        public RatesAndStatusViewModel(AuthService authService, INavigation navigation)
        {
            _authService = authService;
            _navigation = navigation;
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            _ = InitializeAsync();
        }


        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await _authService.CreateAuthenticatedClientAsync(); 
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Authentication error: " + ex.Message, "OK");
            }
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                // Meters (autenticado)
                var metersResponse = await _httpClient.GetAsync("Customer/meters/status");
                if (metersResponse.IsSuccessStatusCode)
                {
                    var meterList = await metersResponse.Content.ReadFromJsonAsync<List<MeterStatusDTO>>();
                    Meters = new ObservableCollection<MeterStatusDTO>(meterList ?? new());
                }
                else
                {
                    var error = await metersResponse.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Failed to get meters: {metersResponse.StatusCode}\n{error}", "OK");
                }

               
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var anonClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://10.0.2.2:44328/api/")
                };

                anonClient.DefaultRequestHeaders.Accept.Clear();
                anonClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var tariffsResponse = await anonClient.GetAsync("Customer/tariff-brackets");

                if (tariffsResponse.IsSuccessStatusCode)
                {
                    var tariffList = await tariffsResponse.Content.ReadFromJsonAsync<List<TariffDTO>>();
                    Tariffs = new ObservableCollection<TariffDTO>(tariffList ?? new());
                }
                else
                {
                    var error = await tariffsResponse.Content.ReadAsStringAsync();
                    await Shell.Current.DisplayAlert("Error", $"Failed to get tariffs: {tariffsResponse.StatusCode}\n{error}", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to load data: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
