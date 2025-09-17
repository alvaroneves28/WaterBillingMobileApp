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
            _ = LoadTariffsAsync(); // Carregar tarifas ao inicializar
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
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        private async Task SubmitRequestAsync()
        {
            if (IsBusy) return;

            // Validações
            if (string.IsNullOrWhiteSpace(FullName))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your full name.", "OK");
                return;
            }

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

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter your phone number.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(InstallationAddress))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter the installation address.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var request = new MeterRequestDTO
                {
                    FullName = FullName,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    InstallationAddress = InstallationAddress,
                    Comments = Comments ?? string.Empty,
                    RequestDate = DateTime.Now
                };

                var response = await _httpClient.PostAsJsonAsync("Anonymous/meter-request", request);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert(
                        "Success",
                        "Your meter request has been submitted successfully! We will review your request and contact you soon via email or phone.",
                        "OK");

                    // Limpar o formulário
                    ClearForm();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    string errorMessage = "Failed to submit request. Please try again.";

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
                        // Usar mensagem padrão
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

        private async Task LoadTariffsAsync()
        {
            if (IsLoadingTariffs) return;

            try
            {
                IsLoadingTariffs = true;

                var tariffs = await _httpClient.GetFromJsonAsync<List<TariffDTO>>("Anonymous/tariff-brackets");

                if (tariffs != null)
                {
                    PublicTariffs = new ObservableCollection<TariffDTO>(tariffs);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load tariffs: {ex.Message}", "OK");
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
                await Shell.Current.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
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
    }
}