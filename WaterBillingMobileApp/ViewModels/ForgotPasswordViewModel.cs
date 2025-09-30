using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class ForgotPasswordViewModel : ObservableObject
    {
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        public ForgotPasswordViewModel()
        {
            SendRecoveryCommand = new AsyncRelayCommand(SendRecoveryAsync);
            BackToLoginCommand = new AsyncRelayCommand(BackToLoginAsync);
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private bool isBusy;

        public IAsyncRelayCommand SendRecoveryCommand { get; }
        public IAsyncRelayCommand BackToLoginCommand { get; }

        private async Task SendRecoveryAsync()
        {
            if (IsBusy) return;

            // Validações
            if (string.IsNullOrWhiteSpace(Email))
            {
                await ShowAlert("Error", "Please enter your email address.", "OK");
                return;
            }

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

                // Criar request para a API
                var request = new { Email = Email };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Configurar HttpClient
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };

                using var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(BaseUrl),
                    Timeout = TimeSpan.FromSeconds(30)
                };

                // Enviar pedido
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

                    // Limpar o campo email
                    Email = string.Empty;

                    // Tentar voltar para a página de login após sucesso
                    await BackToLoginAsync();
                }
                else
                {
                    // Tentar extrair mensagem de erro
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
                        // Usar mensagem padrão ou resposta bruta
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

        private async Task BackToLoginAsync()
        {
            try
            {
                // Se foi aberto como modal, fechar modal
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
                // Tentar alternativa
                try
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.Navigation.PopModalAsync();
                    }
                }
                catch
                {
                    // Se nada funcionar, pelo menos não crashar
                    System.Diagnostics.Debug.WriteLine("Could not navigate back");
                }
            }
        }

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