using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Text.Json;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Forgot Password page.
    /// Handles password recovery requests by sending reset instructions to the user's email.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm.
    /// </summary>
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        /// <summary>
        /// Base URL for the API endpoints.
        /// </summary>
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        /// <summary>
        /// Initializes a new instance of the <see cref="ForgotPasswordViewModel"/> class.
        /// Sets up commands for sending recovery email and navigating back to login.
        /// </summary>
        public ForgotPasswordViewModel()
        {
            SendRecoveryCommand = new AsyncRelayCommand(SendRecoveryAsync);
            BackToLoginCommand = new AsyncRelayCommand(BackToLoginAsync);
        }

        /// <summary>
        /// Gets or sets the email address for password recovery.
        /// </summary>
        [ObservableProperty]
        private string email;

        /// <summary>
        /// Gets or sets a value indicating whether a recovery operation is in progress.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets the command to send password recovery instructions to the email address.
        /// </summary>
        public IAsyncRelayCommand SendRecoveryCommand { get; }

        /// <summary>
        /// Gets the command to navigate back to the login page.
        /// </summary>
        public IAsyncRelayCommand BackToLoginCommand { get; }

        /// <summary>
        /// Sends password recovery instructions to the specified email address.
        /// Validates the email format before making the API request.
        /// Displays success or error messages based on the API response.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SendRecoveryAsync()
        {
            if (IsBusy) return;

            // Validate email is not empty
            if (string.IsNullOrWhiteSpace(Email))
            {
                await ShowAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            // Validate email format
            if (!IsValidEmail(Email))
            {
                await ShowAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                System.Diagnostics.Debug.WriteLine("=== SENDING PASSWORD RECOVERY ===");
                System.Diagnostics.Debug.WriteLine($"Email: {Email}");

                // Create API request
                var request = new { Email = Email };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Configure HttpClient with SSL bypass for development
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(BaseUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                };

                // Send request to API
                var response = await httpClient.PostAsync("Auth/forgot-password", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    await ShowAlert(
                        "Success",
                        "Password recovery instructions have been sent to your email address. Please check your inbox and follow the instructions.",
                        "OK");

                    // Clear email field
                    Email = string.Empty;

                    // Navigate back to login page after success
                    await BackToLoginAsync();
                }
                else
                {
                    // Attempt to extract error message from response
                    string errorMessage = "Failed to send recovery email. Please try again.";

                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                        if (errorResponse != null && errorResponse.ContainsKey("message"))
                        {
                            errorMessage = errorResponse["message"];
                        }
                        else if (!string.IsNullOrWhiteSpace(responseContent))
                        {
                            errorMessage = responseContent;
                        }
                    }
                    catch
                    {
                        // Use default message or raw response
                        if (!string.IsNullOrWhiteSpace(responseContent))
                        {
                            errorMessage = responseContent;
                        }
                    }

                    await ShowAlert("Error", errorMessage, "OK");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"HTTP Error: {ex.Message}");
                await ShowAlert("Error", "Network error: Unable to connect to the server. Please check your connection.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await ShowAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Navigates back to the login page by closing the current modal.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task BackToLoginAsync()
        {
            try
            {
                // Close modal if opened as modal
                var currentPage = Application.Current?.MainPage;

                if (currentPage is NavigationPage navPage)
                {
                    await navPage.Navigation.PopModalAsync();
                }
                else if (currentPage != null)
                {
                    await currentPage.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
                // Try alternative navigation
                try
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.Navigation.PopModalAsync();
                    }
                }
                catch
                {
                    // Fail gracefully without crashing
                    System.Diagnostics.Debug.WriteLine("Could not navigate back");
                }
            }
        }

        /// <summary>
        /// Validates an email address format.
        /// </summary>
        /// <param name="email">The email address to validate.</param>
        /// <returns>True if the email format is valid, false otherwise.</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Displays an alert dialog with the specified title, message, and button text.
        /// Ensures the alert is shown on the main UI thread.
        /// </summary>
        /// <param name="title">The title of the alert.</param>
        /// <param name="message">The message content.</param>
        /// <param name="button">The button text.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShowAlert(string title, string message, string button)
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    if (MainThread.IsMainThread)
                    {
                        await Application.Current.MainPage.DisplayAlert(title, message, button);
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            await Application.Current.MainPage.DisplayAlert(title, message, button);
                        });
                    }
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