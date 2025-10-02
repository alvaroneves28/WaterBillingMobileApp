namespace WaterBillingMobileApp.Model
{
    /// <summary>
    /// Model representing a user login request.
    /// Contains the credentials required for authentication.
    /// Used in the LoginPage to authenticate users against the API.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// Used as the unique identifier for authentication.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user's password in plain text.
        /// This is sent securely over HTTPS and is hashed on the server for verification.
        /// </summary>
        public string Password { get; set; }
    }
}