using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WaterBillingMobileApp.Interfaces;
using WaterBillingMobileApp.Model;

namespace WaterBillingMobileApp.Services
{
    /// <summary>
    /// Service implementation for authentication operations.
    /// Handles user login, token management, logout, and creation of authenticated HTTP clients.
    /// Implements secure storage of JWT tokens using the platform's SecureStorage API.
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// Base URL for the API endpoints.
        /// Points to the local development server (Android emulator localhost).
        /// </summary>
        private const string BaseUrl = "https://10.0.2.2:44328/api/";

        /// <summary>
        /// Key used to store and retrieve the JWT token from secure storage.
        /// </summary>
        private const string TokenKey = "jwt_token";

        /// <summary>
        /// HTTP client instance used for API communication.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// Configures the HTTP client with SSL certificate validation bypass for development.
        /// </summary>
        public AuthService()
        {
            var handler = new HttpClientHandler();
            // Bypass SSL certificate validation (development only - should be removed in production)
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        /// <summary>
        /// Authenticates a user with their credentials and stores the JWT token securely.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>A task that represents the asynchronous operation, containing the login response with JWT token.</returns>
        /// <exception cref="Exception">Thrown when authentication fails, credentials are invalid, or token is not received.</exception>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Auth/login", content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
                {
                    throw new Exception("Invalid token received from API");
                }

                // Store token securely
                await SecureStorage.SetAsync(TokenKey, loginResponse.Token);

                return loginResponse;
            }
            else
            {
                throw new Exception($"Login error: {response.StatusCode} - {responseContent}");
            }
        }

        /// <summary>
        /// Retrieves the stored JWT token from secure storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the JWT token or an empty string if not found.</returns>
        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey) ?? string.Empty;
        }

        /// <summary>
        /// Checks if a user is currently logged in by verifying the presence of a stored token.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing true if a valid token exists, false otherwise.</returns>
        public async Task<bool> IsLoggedIn()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            return !string.IsNullOrWhiteSpace(token);
        }

        /// <summary>
        /// Logs out the current user by removing the stored JWT token and clearing notification data.
        /// This action invalidates the current session.
        /// </summary>
        public void Logout()
        {
            SecureStorage.Remove(TokenKey);

            // Clear notification data
            try
            {
                var notificationService = new NotificationService(this);
                notificationService.ClearNotificationData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing notifications on logout: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates an HTTP client configured with JWT token authentication for making authorized API requests.
        /// The client includes the Authorization header with Bearer token authentication.
        /// SSL certificate validation is bypassed for development purposes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a configured HttpClient instance.</returns>
        /// <exception cref="Exception">Thrown when no token is found or user is not authenticated.</exception>
        public async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var token = await SecureStorage.GetAsync(TokenKey);
            if (string.IsNullOrEmpty(token))
                throw new Exception("Token not found. User not authenticated.");

            var handler = new HttpClientHandler
            {
                // Bypass SSL certificate validation (development only)
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };

            // Add JWT token to Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}