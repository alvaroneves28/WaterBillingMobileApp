using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;
using WaterBillingMobileApp.DTO;

namespace WaterBillingMobileApp.ViewModels
{
    /// <summary>
    /// ViewModel for the Anonymous Request page.
    /// Handles meter installation requests from non-authenticated users.
    /// Provides functionality to submit meter requests, display public tariffs, and navigate back to login.
    /// Implements the MVVM pattern using CommunityToolkit.Mvvm.
    /// </summary>
    public partial class AnonymousRequestViewModel : ObservableObject
    {
        /// <summary>
        /// Base URL for the API endpoints.
        /// </summary>
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        /// <summary>
        /// HTTP client for making unauthenticated API requests.
        /// </summary>
        private HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousRequestViewModel"/> class.
        /// Sets up commands, initializes the HTTP client, and loads public tariff information.
        /// </summary>
        public AnonymousRequestViewModel()
        {
            SubmitRequestCommand = new AsyncRelayCommand(SubmitRequestAsync);
            LoadTariffsCommand = new AsyncRelayCommand(LoadTariffsAsync);
            BackToLoginCommand = new AsyncRelayCommand(BackToLoginAsync);

            InitializeHttpClient();

            // Load tariffs on initialization
            _ = LoadTariffsAsync();
        }

        /// <summary>
        /// Gets or sets the full name of the person requesting a meter installation.
        /// </summary>
        [ObservableProperty]
        private string fullName;

        /// <summary>
        /// Gets or sets the email address for contact and account creation.
        /// </summary>
        [ObservableProperty]
        private string email;

        /// <summary>
        /// Gets or sets the phone number for contact purposes.
        /// </summary>
        [ObservableProperty]
        private string phoneNumber;

        /// <summary>
        /// Gets or sets the address where the meter should be installed.
        /// </summary>
        [ObservableProperty]
        private string installationAddress;

        /// <summary>
        /// Gets or sets optional comments or special requests for the installation.
        /// </summary>
        [ObservableProperty]
        private string comments;

        /// <summary>
        /// Gets or sets the collection of publicly available water tariffs.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<TariffDTO> publicTariffs = new();

        /// <summary>
        /// Gets or sets a value indicating whether a submit operation is in progress.
        /// </summary>
        [ObservableProperty]
        private bool isBusy;

        /// <summary>
        /// Gets or sets a value indicating whether tariffs are being loaded.
        /// </summary>
        [ObservableProperty]
        private bool isLoadingTariffs;

        /// <summary>
        /// Gets the command to submit a meter installation request.
        /// </summary>
        public IAsyncRelayCommand SubmitRequestCommand { get; }

        /// <summary>
        /// Gets the command to load public tariff information.
        /// </summary>
        public IAsyncRelayCommand LoadTariffsCommand { get; }

        /// <summary>
        /// Gets the command to navigate back to the login page.
        /// </summary>
        public IAsyncRelayCommand BackToLoginCommand { get; }

        /// <summary>
        /// Initializes the HTTP client with SSL certificate validation bypass for development.
        /// Configures base address, timeout, and JSON content type header.
        /// </summary>
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

        /// <summary>
        /// Submits a meter installation request to the API.
        /// Validates all required fields before submission and displays appropriate success or error messages.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task SubmitRequestAsync()
        {
            if (IsBusy) return;

            // Validate full name
            if (string.IsNullOrWhiteSpace(FullName))
            {
                await ShowAlert("Error", "Please enter your full name.", "OK");
                return;
            }

            // Validate email
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

            // Validate phone number
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                await ShowAlert("Error", "Please enter your phone number.", "OK");
                return;
            }

            // Validate installation address
            if (string.IsNullOrWhiteSpace(InstallationAddress))
            {
                await ShowAlert("Error", "Please enter the installation address.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // Create request DTO matching API expectations
                var request = new
                {
                    Name = FullName,
                    Email = Email,
                    Address = InstallationAddress,
                    NIF = "000000000", // Temporary - can add field to form if needed
                    PhoneNumber = PhoneNumber
                };

                var json = JsonSerializer.Serialize(request);

                var content = new System.Net.Http.StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    "application/json");

                // Submit to correct endpoint
                var response = await _httpClient.PostAsync("Customer/meter-requests/anonymous", content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await ShowAlert(
                        "Success",
                        "Your meter request has been submitted successfully! We will review your request and contact you soon via email or phone.",
                        "OK");

                    // Clear form after successful submission
                    ClearForm();
                }
                else
                {
                    var errorMessage = "Failed to submit request. Please try again.";

                    // Attempt to extract error message from response
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

        /// <summary>
        /// Loads publicly available tariff information from the API.
        /// Updates the PublicTariffs collection with the retrieved data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task LoadTariffsAsync()
        {
            if (IsLoadingTariffs) return;

            try
            {
                IsLoadingTariffs = true;

                System.Diagnostics.Debug.WriteLine("Loading tariffs...");

                // Use correct endpoint for public tariffs
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
        /// Clears all form fields after successful submission.
        /// </summary>
        private void ClearForm()
        {
            FullName = string.Empty;
            Email = string.Empty;
            PhoneNumber = string.Empty;
            InstallationAddress = string.Empty;
            Comments = string.Empty;
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