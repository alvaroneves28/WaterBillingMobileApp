using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.ViewModels;

/// <summary>
/// ViewModel for the Meter Reading submission page.
/// Handles loading user meters and submitting new water consumption readings.
/// Implements the MVVM pattern using CommunityToolkit.Mvvm.
/// </summary>
public partial class SubmitReadingViewModel : ObservableObject
{
    /// <summary>
    /// Navigation service for page navigation operations.
    /// </summary>
    private readonly INavigation _navigation;

    /// <summary>
    /// Authentication service for creating authenticated HTTP clients.
    /// </summary>
    private readonly IAuthService _authService;

    /// <summary>
    /// Authenticated HTTP client for making API requests.
    /// </summary>
    private HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitReadingViewModel"/> class.
    /// Initiates asynchronous initialization to load meters.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="authService">The authentication service.</param>
    public SubmitReadingViewModel(INavigation navigation, IAuthService authService)
    {
        _navigation = navigation;
        _authService = authService;
        InitializeAsync();
    }

    /// <summary>
    /// Initializes the authenticated HTTP client and loads available meters.
    /// Displays an error alert if authentication fails.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the collection of meters available for the authenticated user.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<MeterDto> meters = new();

    /// <summary>
    /// Gets or sets the meter selected by the user for reading submission.
    /// </summary>
    [ObservableProperty]
    private MeterDto selectedMeter;

    /// <summary>
    /// Gets or sets the meter reading value in cubic meters (m³).
    /// </summary>
    [ObservableProperty]
    private double value;

    /// <summary>
    /// Gets or sets the date when the meter reading was taken.
    /// Defaults to today's date.
    /// </summary>
    [ObservableProperty]
    private DateTime date = DateTime.Today;

    /// <summary>
    /// Loads the list of meters owned by the authenticated user from the API.
    /// Updates the Meters collection with the retrieved data.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

    /// <summary>
    /// Submits a new meter reading to the API.
    /// Validates that a meter is selected before submission.
    /// Navigates back to the previous page upon successful submission.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [RelayCommand]
    private async Task SubmitAsync()
    {
        // Validate meter selection
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