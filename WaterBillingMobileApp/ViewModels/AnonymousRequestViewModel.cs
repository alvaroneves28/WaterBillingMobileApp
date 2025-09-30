using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using WaterBillingMobileApp.DTO;

namespace WaterBillingMobileApp.ViewModels
{
    public partial class AnonymousRequestViewModel : ObservableObject
    {
        private const string BaseUrl = "https://10.0.2.2:44328/api/";
        private HttpClient _httpClient;

        public AnonymousRequestViewModel()
        {
            SubmitRequestCommand = new AsyncRelayCommand(SubmitRequestAsync);
            LoadTariffsCommand = new AsyncRelayCommand(LoadTariffsAsync);
            BackToLoginCommand = new AsyncRelayCommand(BackToLoginAsync);

            InitializeHttpClient();

            // Carregar tarifas ao inicializar
            _ = LoadTariffsAsync();
        }

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private string installationAddress;

        [ObservableProperty]
        private string comments;

        [ObservableProperty]
        private ObservableCollection<TariffDTO> publicTariffs = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isLoadingTariffs;

        public IAsyncRelayCommand SubmitRequestCommand { get; }
        public IAsyncRelayCommand LoadTariffsCommand { get; }
        public IAsyncRelayCommand BackToLoginCommand { get; }

        private void InitializeHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task SubmitRequestAsync()
        {
            if (IsBusy) return;

            // Validações
            if (string.IsNullOrWhiteSpace(FullName))
            {
                await ShowAlert("Error", "Please enter your full name.", "OK");
                return;
            }

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

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await ShowAlert("Error", "Please enter your phone number.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(InstallationAddress))
            {
                await ShowAlert("Error", "Please enter the installation address.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                System.Diagnostics.Debug.WriteLine("=== SUBMITTING METER REQUEST ===");
                System.Diagnostics.Debug.WriteLine($"Name: {FullName}");
                System.Diagnostics.Debug.WriteLine($"Email: {Email}");

                // DTO que corresponde ao que a API espera (AnonymousMeterRequestDTO)
                var request = new
                {
                    Name = FullName,
                    Email = Email,
                    Address = InstallationAddress,
                    NIF = "000000000", // Temporário - pode adicionar campo no form se necessário
                    PhoneNumber = PhoneNumber
                };

                var json = JsonSerializer.Serialize(request);
                System.Diagnostics.Debug.WriteLine($"JSON: {json}");

                var content = new System.Net.Http.StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    "application/json");

                // Endpoint correto baseado no seu CustomerController
                var response = await _httpClient.PostAsync("Customer/meter-requests/anonymous", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response Status: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    await ShowAlert(
                        "Success",
                        "Your meter request has been submitted successfully! We will review your request and contact you soon via email or phone.",
                        "OK");

                    // Limpar o formulário
                    ClearForm();
                }
                else
                {
                    var errorMessage = "Failed to submit request. Please try again.";

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
            catch (System.Net.Http.HttpRequestException ex)
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

        private async Task LoadTariffsAsync()
        {
            if (IsLoadingTariffs) return;

            try
            {
                IsLoadingTariffs = true;

                System.Diagnostics.Debug.WriteLine("Loading tariffs...");

                // Usar o endpoint correto para tarifas públicas
                var tariffs = await _httpClient.GetFromJsonAsync<List<TariffDTO>>("Customer/tariff-brackets");

                if (tariffs != null && tariffs.Any())
                {
                    System.Diagnostics.Debug.WriteLine($"Loaded {tariffs.Count} tariffs");
                    PublicTariffs = new ObservableCollection<TariffDTO>(tariffs);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No tariffs found");
                    PublicTariffs = new ObservableCollection<TariffDTO>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tariffs: {ex.Message}");
                await ShowAlert("Error", $"Failed to load tariffs: {ex.Message}", "OK");
            }
            finally
            {
                IsLoadingTariffs = false;
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

        private void ClearForm()
        {
            FullName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            InstallationAddress = string.Empty;
            Comments = string.Empty;
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
                    await Application.Current.MainPage.DisplayAlert(title, message, button);
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