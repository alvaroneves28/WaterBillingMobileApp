using WaterBillingMobileApp.DTO;

namespace WaterBillingMobileApp.Interfaces
{
    /// <summary>
    /// Interface for user profile management operations.
    /// Handles retrieval and updates of user profile information including personal data,
    /// email, password, and profile image through authenticated API calls.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Retrieves the current user's profile information from the server.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the user's profile data.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task<ProfileDTO> GetProfileAsync();

        /// <summary>
        /// Updates the user's profile information (name, phone number, and address).
        /// Does not include email or password updates, which have separate methods.
        /// </summary>
        /// <param name="request">The profile update request containing the new user information.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task UpdateProfileAsync(UpdateProfileRequest request);

        /// <summary>
        /// Updates the user's email address.
        /// Requires the current password for security verification.
        /// </summary>
        /// <param name="request">The email update request containing the new email and current password.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails or password is incorrect.</exception>
        Task UpdateEmailAsync(UpdateEmailRequest request);

        /// <summary>
        /// Updates the user's password.
        /// Requires the current password for security verification.
        /// </summary>
        /// <param name="request">The password update request containing current and new passwords.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if password was updated successfully, false otherwise.</returns>
        Task<bool> UpdatePasswordAsync(UpdatePasswordRequest request);

        /// <summary>
        /// Updates the user's profile image.
        /// Accepts image URLs or base64 encoded image strings.
        /// </summary>
        /// <param name="request">The profile image update request containing the image URL or base64 string.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
        Task UpdateProfileImageAsync(UpdateProfileImageRequest request);
    }
}