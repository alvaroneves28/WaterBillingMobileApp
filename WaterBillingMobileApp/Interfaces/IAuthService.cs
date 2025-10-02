using WaterBillingMobileApp.Model;

namespace WaterBillingMobileApp.Interfaces
{
    /// <summary>
    /// Interface for authentication service operations.
    /// Handles user authentication, token management, and authenticated HTTP client creation.
    /// Implements secure storage of JWT tokens and provides methods for login/logout operations.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with their credentials and retrieves a JWT token.
        /// The token is automatically stored in secure storage upon successful login.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>A task that represents the asynchronous operation, containing the login response with JWT token.</returns>
        /// <exception cref="Exception">Thrown when authentication fails or credentials are invalid.</exception>
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Retrieves the stored JWT authentication token from secure storage.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing the JWT token string or empty string if not found.</returns>
        Task<string> GetTokenAsync();

        /// <summary>
        /// Checks if a user is currently logged in by verifying the presence of a valid token.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing true if user is logged in, false otherwise.</returns>
        Task<bool> IsLoggedIn();

        /// <summary>
        /// Logs out the current user by removing the stored JWT token and clearing notification data.
        /// This action invalidates the current session.
        /// </summary>
        void Logout();

        /// <summary>
        /// Creates an HTTP client configured with the stored JWT token for authenticated API requests.
        /// The client includes the Authorization header with Bearer token authentication.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a configured HttpClient instance.</returns>
        /// <exception cref="Exception">Thrown when no token is found or user is not authenticated.</exception>
        Task<HttpClient> CreateAuthenticatedClientAsync();
    }
}