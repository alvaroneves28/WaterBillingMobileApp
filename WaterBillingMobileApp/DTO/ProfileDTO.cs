namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a user's profile information.
    /// Used in the ProfilePage to display and update user details.
    /// </summary>
    public class ProfileDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user account.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the residential or billing address of the user.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the file path or URL of the user's profile image.
        /// Can be a local path, remote URL, or base64 encoded image string.
        /// </summary>
        public string ProfileImagePath { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier associated with this profile.
        /// Nullable to support cases where the user is not yet a registered customer.
        /// </summary>
        public int? CustomerId { get; set; }
    }
}