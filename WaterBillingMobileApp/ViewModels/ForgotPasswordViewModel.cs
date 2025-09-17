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
                await Shell.Current.DisplayAlert("Error", "Please enter your email address.", "OK");
                return;
            }

            if (!IsValidEmail(Email))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a valid email address.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Criar request para a API
                var request = new { Email = Email };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Configurar HttpClient
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(BaseUrl)
                };

                // Enviar pedido
                var response = await httpClient.PostAsync("Auth/forgot-password", content);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert(
                        "Success",
                        "Password recovery instructions have been sent to your email address. Please check your inbox and follow the instructions.",
                        "OK");

                    // Limpar o campo email
                    Email = string.Empty;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    // Tentar extrair mensagem de erro
                    string errorMessage = "Failed to send recovery email. Please try again.";

                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent);
                        if (errorResponse != null && errorResponse.ContainsKey("message"))
                        {
                            errorMessage = errorResponse["message"];
                        }
                    }
                    catch
                    {
                        // Usar mensagem padrão se não conseguir extrair
                    }

                    await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Network error: {ex.Message}", "OK");
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
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
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
    }
}