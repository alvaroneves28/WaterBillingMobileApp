using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.Services
{
    /// <summary>
    /// Service implementation for managing invoice notifications.
    /// Handles detection of new invoices, periodic checking, and notification display.
    /// Persists notification state using platform preferences to track the last check time.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Authentication service used to verify user login status and create authenticated HTTP clients.
        /// </summary>
        private readonly AuthService _authService;

        /// <summary>
        /// Timestamp of the last invoice check. Used to detect new invoices since the last verification.
        /// </summary>
        private DateTime _lastCheckTime;

        /// <summary>
        /// Preferences key for storing the last invoice check timestamp.
        /// </summary>
        private const string LAST_CHECK_KEY = "last_invoice_check";

        /// <summary>
        /// Preferences key for storing the last known invoice count.
        /// </summary>
        private const string LAST_INVOICE_COUNT_KEY = "last_invoice_count";

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// Loads the last check time from persistent storage.
        /// </summary>
        /// <param name="authService">The authentication service for user verification and HTTP client creation.</param>
        public NotificationService(AuthService authService)
        {
            _authService = authService;
            LoadLastCheckTime();
        }

        /// <summary>
        /// Checks if there are new invoices available since the last verification.
        /// Compares current invoices with the last known state by date and count.
        /// Displays a notification alert if new invoices are detected.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing true if new invoices were found, false otherwise.</returns>
        public async Task<bool> CheckForNewInvoicesAsync()
        {
            try
            {
                // Verify if user is authenticated
                if (!await _authService.IsLoggedIn())
                    return false;

                var httpClient = await _authService.CreateAuthenticatedClientAsync();
                var invoices = await httpClient.GetFromJsonAsync<List<InvoiceDTO>>("Customer/invoices");

                if (invoices == null || invoices.Count == 0)
                    return false;

                // Check for invoices issued after the last check
                var newInvoices = invoices.Where(i => i.IssueDate > _lastCheckTime).ToList();

                // Alternative method: check if invoice count has increased
                var lastInvoiceCount = await GetLastInvoiceCountAsync();
                var hasNewInvoicesByCount = invoices.Count > lastInvoiceCount;

                if (newInvoices.Any() || hasNewInvoicesByCount)
                {
                    // Update the last check timestamp
                    _lastCheckTime = DateTime.Now;
                    await SaveLastCheckTimeAsync();
                    await SaveInvoiceCountAsync(invoices.Count);

                    // Show notification for new invoices
                    await ShowNewInvoiceNotificationAsync(newInvoices.Count > 0 ? newInvoices.Count : 1);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log error (in production, use a proper logging framework)
                System.Diagnostics.Debug.WriteLine($"Error checking for new invoices: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Starts automatic periodic checking for new invoices every 30 minutes.
        /// The first check runs immediately upon calling this method.
        /// </summary>
        public void StartPeriodicCheck()
        {
            var timer = new Timer(async _ => await CheckForNewInvoicesAsync(),
                                 null,
                                 TimeSpan.Zero, // Start immediately
                                 TimeSpan.FromMinutes(30)); // Repeat every 30 minutes
        }

        /// <summary>
        /// Checks for new invoices when the application starts.
        /// Includes a delay to allow the app to fully load before performing the check.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task CheckOnAppStartAsync()
        {
            // Wait for app to fully load
            await Task.Delay(3000);
            await CheckForNewInvoicesAsync();
        }

        /// <summary>
        /// Displays a notification alert to inform the user about new invoices.
        /// Uses the MAUI alert system with options to view invoices immediately or dismiss.
        /// </summary>
        /// <param name="invoiceCount">The number of new invoices detected.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShowNewInvoiceNotificationAsync(int invoiceCount)
        {
            string message = invoiceCount == 1
                ? "You have a new invoice available!"
                : $"You have {invoiceCount} new invoices available!";

            // Use MAUI alert system
            await Shell.Current.DisplayAlert(
                "New Invoice",
                message + " Tap to view your invoices.",
                "View Now",
                "Later");

            // Navigation to invoice page can be implemented with events for better UX
        }

        /// <summary>
        /// Loads the last check time from persistent storage.
        /// If no previous check exists, defaults to 30 days ago to capture all recent invoices.
        /// </summary>
        private void LoadLastCheckTime()
        {
            var lastCheckString = Preferences.Get(LAST_CHECK_KEY, string.Empty);

            if (DateTime.TryParse(lastCheckString, out var lastCheck))
            {
                _lastCheckTime = lastCheck;
            }
            else
            {
                // No previous check recorded, use old date to fetch all invoices
                _lastCheckTime = DateTime.Now.AddDays(-30);
            }
        }

        /// <summary>
        /// Saves the last check timestamp to persistent storage.
        /// Uses ISO 8601 format for reliable date parsing across platforms.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SaveLastCheckTimeAsync()
        {
            Preferences.Set(LAST_CHECK_KEY, _lastCheckTime.ToString("O"));
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves the last known invoice count from persistent storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the last invoice count or 0 if none exists.</returns>
        private async Task<int> GetLastInvoiceCountAsync()
        {
            return Preferences.Get(LAST_INVOICE_COUNT_KEY, 0);
        }

        /// <summary>
        /// Saves the current invoice count to persistent storage.
        /// </summary>
        /// <param name="count">The current number of invoices.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SaveInvoiceCountAsync(int count)
        {
            Preferences.Set(LAST_INVOICE_COUNT_KEY, count);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Clears all stored notification data including check time and invoice count.
        /// Resets the last check time to 30 days ago.
        /// Typically called during user logout to reset notification state.
        /// </summary>
        public void ClearNotificationData()
        {
            Preferences.Remove(LAST_CHECK_KEY);
            Preferences.Remove(LAST_INVOICE_COUNT_KEY);
            _lastCheckTime = DateTime.Now.AddDays(-30);
        }

        /// <summary>
        /// Forces an immediate check for new invoices, bypassing the normal interval.
        /// Useful for manual refresh operations or testing purposes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ForceCheckAsync()
        {
            _lastCheckTime = DateTime.Now.AddDays(-1); // Force check by setting old timestamp
            await CheckForNewInvoicesAsync();
        }
    }
}