using System.Net.Http.Json;
using WaterBillingMobileApp.DTO;
using WaterBillingMobileApp.Interfaces;

namespace WaterBillingMobileApp.Services
{
    /// <summary>
    /// Service implementation for user profile management operations.
    /// Handles all profile-related API calls including retrieval and updates of user information,
    /// email, password, and profile image through authenticated HTTP requests.
    /// </summary>
    public class ProfileService : IProfileService
    {
        /// <summary>
        /// Authenticated HTTP client used for making API requests.
        /// Configured with JWT Bearer token authentication by the AuthService.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileService"/> class.
        /// </summary>
        /// <param name="httpClient">The authenticated HTTP client for API communication.</param>
        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves the current user's profile information from the API.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the user's profile data.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails or returns a non-success status code.</exception>
        public async Task<ProfileDTO> GetProfileAsync()
        {
            var response = await _httpClient.GetAsync("Profile");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProfileDTO>();
        }

        /// <summary>
        /// Updates the user's profile information (name, phone number, and address).
        /// Does not include email or password updates, which have separate dedicated methods.
        /// </summary>
        /// <param name="request">The profile update request containing the new user information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails or returns a non-success status code.</exception>
        public async Task UpdateProfileAsync(UpdateProfileRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync("Profile", request);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Updates the user's email address.
        /// Requires the current password for security verification to prevent unauthorized email changes.
        /// </summary>
        /// <param name="request">The email update request containing the new email and current password for verification.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails, password is incorrect, or email is already in use.</exception>
        public async Task UpdateEmailAsync(UpdateEmailRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync("Profile/email", request);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Updates the user's password.
        /// Requires the current password for security verification to prevent unauthorized password changes.
        /// </summary>
        /// <param name="request">The password update request containing current and new passwords.</param>
        /// <returns>
        /// A task that represents the asynchronous operation, containing true if the password was updated successfully, 
        /// false if the current password is incorrect or validation fails.
        /// </returns>
        public async Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync("Profile/password", request);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates the user's profile image.
        /// Accepts image URLs, local file paths, or base64 encoded image strings.
        /// </summary>
        /// <param name="request">The profile image update request containing the image URL or base64 encoded image data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails, image format is invalid, or file size exceeds limits.</exception>
        public async Task UpdateProfileImageAsync(UpdateProfileImageRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync("Profile/image", request);
            response.EnsureSuccessStatusCode();
        }
    }
}