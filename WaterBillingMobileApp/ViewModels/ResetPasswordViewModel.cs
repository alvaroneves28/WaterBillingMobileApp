using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WaterBillingMobileApp.Services;

namespace WaterBillingMobileApp.ViewModels
{
    [QueryProperty(nameof(Token), "token")]
    [QueryProperty(nameof(Email), "email")]
    public partial class ResetPasswordViewModel : ObservableObject
    {
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        [ObservableProperty]
        private string token; // Vem do deep link (Base64 URL-safe)

        [ObservableProperty]
        private string email; // Vem do deep link

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand ResetPasswordCommand { get; }
        public IAsyncRelayCommand BackToLoginCommand { get; }

        public ResetPasswordViewModel(ResetPasswordService service)
        {
            Token = service.Token;
            Email = service.Email;

            ResetPasswordCommand = new AsyncRelayCommand(ResetPasswordAsync);
            BackToLoginCommand = new AsyncRelayCommand(BackToLoginAsync);
        }

        partial void OnTokenChanged(string value)
        {
            System.Diagnostics.Debug.WriteLine($"Token received (Base64 URL-safe): {value}");
        }

        partial void OnEmailChanged(string value)
        {
            System.Diagnostics.Debug.WriteLine($"Email received: {value}");
        }

        private async Task ResetPasswordAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Token))
            {
                await ShowAlert("Error", "Invalid reset link. Please request a new one.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await ShowAlert("Error", "Please enter a new password.", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await ShowAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // 🔹 Decodificar token Base64 URL-safe antes de enviar para o backend
                var tokenBytes = Convert.FromBase64String(Token.Replace('-', '+').Replace('_', '/'));
                var originalToken = Encoding.UTF8.GetString(tokenBytes);

                var request = new
                {
                    Email = Email,
                    Token = originalToken,  // usar token decodificado
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

        private async Task BackToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task ShowAlert(string title, string message, string button)
        {
            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(title, message, button);
            }
        }
    }
}
