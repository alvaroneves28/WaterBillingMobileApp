namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating a user's password.
    /// Used in ProfilePage when the user wants to change their password.
    /// Requires current password verification for security purposes.
    /// </summary>
    public class UpdatePasswordRequest
    {
        /// <summary>
        /// Gets or sets the user's current password for verification.
        /// Required to authorize the password change operation.
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password the user wants to set.
        /// Must meet the application's password strength requirements (e.g., minimum 6 characters).
        /// </summary>
        public string NewPassword { get; set; }
    }
}