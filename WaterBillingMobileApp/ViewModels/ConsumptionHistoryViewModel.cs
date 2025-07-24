using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class ConsumptionHistoryViewModel : ObservableObject
    {
        private readonly INavigation _navigation;
        private HttpClient _httpClient;

        public ConsumptionHistoryViewModel(INavigation navigation)
        {
            _navigation = navigation;
            LoadHistoryCommand = new AsyncRelayCommand(LoadHistoryAsync);
            _ = InitializeAsync();
        }

        [ObservableProperty]
        private ObservableCollection<ConsumptionHistoryDTO> consumptionHistory = new();

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand LoadHistoryCommand { get; }

        private async Task InitializeAsync()
        {
            try
            {
                _httpClient = await AuthService.CreateAuthenticatedClientAsync();
                await LoadHistoryAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Authentication error: " + ex.Message, "OK");
            }
        }

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
