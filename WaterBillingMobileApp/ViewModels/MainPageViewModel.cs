using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.Views;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Main Dashboard page.
    /// Provides navigation to all major app features and displays notification indicators for new invoices.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm.
    /// </summary>
    public partial class MainPageViewModel : ObservableObject
    {
        /// <summary>
        /// Navigation service for page navigation operations.
        /// </summary>
        private readonly INavigation _navigation;

        /// <summary>
        /// Authentication service for creating authenticated HTTP clients and managing user sessions.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Gets the command to log out the current user.
        /// </summary>
        public IAsyncRelayCommand LogoutCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// Sets up navigation commands and initiates a check for new invoices.
        /// </summary>
        /// <param name="navigation">The navigation service.</param>
        /// <param name="authService">The authentication service.</param>
        public MainPageViewModel(INavigation navigation, IAuthService authService)
        {
            _navigation = navigation;
            _authService = authService;
            _ = CheckForNewInvoicesAsync();
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);
        }

        /// <summary>
        /// Logs out the current user by clearing stored credentials and navigating to the login page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LogoutAsync()
        {
            try
            {
                Preferences.Remove("AuthToken");
                Preferences.Remove("UserEmail");
                Preferences.Remove("UserName");

                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Logout failed: {ex.Message}");
                await App.Current.MainPage.DisplayAlert("Error", "Unable to logout. Please try again.", "OK");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there are new unread invoices.
        /// Used to display notification badges in the UI.
        /// </summary>
        [ObservableProperty]
        private bool hasNewInvoices;

        /// <summary>
        /// Gets or sets the count of new unread invoices.
        /// Displayed in notification badges.
        /// </summary>
        [ObservableProperty]
        private int newInvoicesCount;

        /// <summary>
        /// Checks for new unread invoices from the API.
        /// Updates notification indicators based on the result.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task CheckForNewInvoicesAsync()
        {
            try
            {
                var httpClient = await _authService.CreateAuthenticatedClientAsync();
                var unreadInvoices = await httpClient.GetFromJsonAsync<List<InvoiceDTO>>("Customer/invoices/unread");

                NewInvoicesCount = unreadInvoices?.Count ?? 0;
                HasNewInvoices = NewInvoicesCount > 0;
            }
            catch (Exception ex)
            {
                // Log error if necessary
                System.Diagnostics.Debug.WriteLine($"Error checking for new invoices: {ex.Message}");
            }
        }

        /// <summary>
        /// Navigates to the meter reading submission page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task SubmitReading()
        {
            var submitReadingViewModel = new SubmitReadingViewModel(_navigation, _authService);
            await _navigation.PushAsync(new MeterReadingPage(submitReadingViewModel));
        }

        /// <summary>
        /// Navigates to the consumption history page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewConsumptionHistory()
        {
            await _navigation.PushAsync(new ConsumptionHistoryPage(_authService));
        }

        /// <summary>
        /// Navigates to the invoices page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewInvoices()
        {
            await _navigation.PushAsync(new InvoicesPage(_authService));
        }

        /// <summary>
        /// Navigates to the rates and meter status page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewRatesAndStatus()
        {
            await _navigation.PushAsync(new RatesAndStatusPage(_authService));
        }

        /// <summary>
        /// Navigates to the user profile page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewProfile()
        {
            await _navigation.PushAsync(new ProfilePage(_authService));
        }

        /// <summary>
        /// Navigates to the about page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewAbout()
        {
            await _navigation.PushAsync(new AboutPage());
        }
    }
}