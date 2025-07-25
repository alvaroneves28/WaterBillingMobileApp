using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Model;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels;

public partial class SubmitReadingViewModel : ObservableObject
{
    private readonly INavigation _navigation;
    private readonly AuthService _authService;
    private HttpClient _httpClient;

    public SubmitReadingViewModel(INavigation navigation, AuthService authService)
    {
        _navigation = navigation;
        _authService = authService;

        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            _httpClient = await _authService.CreateAuthenticatedClientAsync();
            await LoadMetersAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Authentication error: " + ex.Message, "OK");
        }
    }

    [ObservableProperty]
    private ObservableCollection<MeterDto> meters = new();

    [ObservableProperty]
    private MeterDto selectedMeter;

    [ObservableProperty]
    private double value;

    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [RelayCommand]
    private async Task LoadMetersAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<MeterDto>>("Customer/mine");
            Meters = new ObservableCollection<MeterDto>(result ?? new());
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to load meters: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (SelectedMeter == null)
        {
            await Shell.Current.DisplayAlert("Error", "Please select a meter.", "OK");
            return;
        }

        var dto = new CreateConsumptionDTO
        {
            MeterId = SelectedMeter.Id,
            Value = Value,
            Date = Date
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("Customer/consumptions", dto);

            if (response.IsSuccessStatusCode)
            {
                await Shell.Current.DisplayAlert("Success", "Consumption registered successfully.", "OK");
                await _navigation.PopAsync();
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var error = JsonSerializer.Deserialize<Dictionary<string, string>>(errorJson);
                var msg = error.ContainsKey("message") ? error["message"] : "Unknown error";
                await Shell.Current.DisplayAlert("Error", msg, "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
