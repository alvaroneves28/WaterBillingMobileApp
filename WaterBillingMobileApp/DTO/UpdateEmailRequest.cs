namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating a user's email address.
    /// Used in ProfilePage when the user wants to change their email.
    /// Requires current password verification for security purposes.
    /// </summary>
    public class UpdateEmailRequest
    {
        /// <summary>
        /// Gets or sets the user's current password for verification.
        /// Required to authorize the email change operation.
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// Gets or sets the new email address the user wants to use.
        /// Must be a valid email format and not already in use by another account.
        /// </summary>
        public string NewEmail { get; set; }
    }
}