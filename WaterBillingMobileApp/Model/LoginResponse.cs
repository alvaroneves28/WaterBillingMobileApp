namespace WaterBillingMobileApp.Model
{
    /// <summary>
    /// Model representing the response from a successful login request.
    /// Contains the JWT authentication token used for subsequent API requests.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Gets or sets the JWT (JSON Web Token) authentication token.
        /// This token is stored securely and included in the Authorization header
        /// of all authenticated API requests as a Bearer token.
        /// </summary>
        public string Token { get; set; }
    }
}