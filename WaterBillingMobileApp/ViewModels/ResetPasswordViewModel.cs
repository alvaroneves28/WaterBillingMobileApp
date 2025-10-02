using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;
using System.Text.Json;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Reset Password page.
    /// Handles password reset operations using a token received from a deep link.
    /// Validates password requirements and communicates with the API to complete the reset.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm with query property support.
    /// </summary>
    [QueryProperty(nameof(Token), "token")]
    [QueryProperty(nameof(Email), "email")]
    public partial class ResetPasswordViewModel : ObservableObject
    {
        /// <summary>
        /// Base URL for the API endpoints.
        /// </summary>
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        /// <summary>
        /// Gets or sets the password reset token received from the deep link.
        /// This token is Base64 URL-safe encoded and must be decoded before sending to the API.
        /// </summary>
        [ObservableProperty]
        private string token;

        /// <summary>
        /// Gets or sets the email address associated with the password reset request.
        /// </summary>
        [ObservableProperty]
        private string email;

        /// <summary>
        /// Gets or sets the new password entered by the user.
        /// </summary>
        [ObservableProperty]
        private string newPassword;

        /// <summary>
        /// Gets or sets the password confirmation entered by the user.
        /// Must match NewPassword for the reset to proceed.
        /// </summary>
        [ObservableProperty]
        private string confirmPassword;

        /// <summary>
        /// Gets or sets a value indicating whether a reset operation is in progress.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets the command to reset the password.
        /// </summary>
        public IAsyncRelayCommand ResetPasswordCommand { get; }

        /// <summary>
        /// Gets the command to navigate back to the login page.
        /// </summary>
        public IAsyncRelayCommand BackToLoginCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetPasswordViewModel"/> class.
        /// Retrieves token and email from the ResetPasswordService.
        /// </summary>
        /// <param name="service">The service containing reset password data from the deep link.</param>
        public ResetPasswordViewModel(ResetPasswordService service)
        {
            Token = service.Token;
            Email = service.Email;

            ResetPasswordCommand = new AsyncRelayCommand(ResetPasswordAsync);
            BackToLoginCommand = new AsyncRelayCommand(BackToLoginAsync);
        }

        /// <summary>
        /// Called when the Token property changes.
        /// Logs the received Base64 URL-safe encoded token for debugging.
        /// </summary>
        /// <param name="value">The new token value.</param>
        partial void OnTokenChanged(string value)
        {
            System.Diagnostics.Debug.WriteLine($"Token received (Base64 URL-safe): {value}");
        }

        /// <summary>
        /// Called when the Email property changes.
        /// Logs the received email for debugging.
        /// </summary>
        /// <param name="value">The new email value.</param>
        partial void OnEmailChanged(string value)
        {
            System.Diagnostics.Debug.WriteLine($"Email received: {value}");
        }

        /// <summary>
        /// Resets the user's password after validation.
        /// Decodes the Base64 URL-safe token, validates password requirements, and sends the reset request to the API.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ResetPasswordAsync()
        {
            if (IsBusy) return;

            // Validate token exists
            if (string.IsNullOrWhiteSpace(Token))
            {
                await ShowAlert("Error", "Invalid reset link. Please request a new one.", "OK");
                return;
            }

            // Validate new password is not empty
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await ShowAlert("Error", "Please enter a new password.", "OK");
                return;
            }

            // Validate passwords match
            if (NewPassword != ConfirmPassword)
            {
                await ShowAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Decode Base64 URL-safe token before sending to backend
                var tokenBytes = Convert.FromBase64String(Token.Replace('-', '+').Replace('_', '/'));
                var originalToken = Encoding.UTF8.GetString(tokenBytes);

                var request = new
                {
                    Email = Email,
                    Token = originalToken,  // Use decoded token
                    NewPassword = NewPassword
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(BaseUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var response = await httpClient.PostAsync("Auth/reset-password", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    await ShowAlert("Success", "Password reset successfully! You can now login with your new password.", "OK");

                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;

                    await BackToLoginAsync();
                }
                else
                {
                    var errorMessage = "Failed to reset password. The link may have expired.";

                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                        if (errorResponse?.ContainsKey("message") == true)
                        {
                            errorMessage = errorResponse["message"];
                        }
                    }
                    catch { }

                    await ShowAlert("Error", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                await ShowAlert("Error", "An error occurred. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Navigates back to the login page.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task BackToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
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
            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(title, message, button);
            }
        }
    }
}