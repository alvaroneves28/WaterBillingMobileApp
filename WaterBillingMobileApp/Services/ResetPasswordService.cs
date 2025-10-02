namespace WaterBillingMobileApp.Services
{
    /// <summary>
    /// Service for managing password reset data received from deep links.
    /// Acts as a temporary storage for reset token and email when the app is opened via a password reset link.
    /// This data is used by the ResetPasswordViewModel to complete the password reset process.
    /// </summary>
    /// <remarks>
    /// This service is registered as a singleton to maintain state across the deep link handling
    /// and the ResetPasswordPage navigation. The token is expected to be Base64 URL-safe encoded.
    /// </remarks>
    public class ResetPasswordService
    {
        /// <summary>
        /// Gets or sets the password reset token received from the deep link.
        /// This token is typically Base64 URL-safe encoded and must be decoded before sending to the API.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the password reset request.
        /// Used to identify which user account should have its password reset.
        /// </summary>
        public string Email { get; set; }
    }
}