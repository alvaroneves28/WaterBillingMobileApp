namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object for updating a user's profile information.
    /// Used in ProfilePage when the user wants to update their personal details.
    /// Does not include email or password changes, which have separate endpoints.
    /// </summary>
    public class UpdateProfileRequest
    {
        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the user's phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the user's residential or billing address.
        /// </summary>
        public string Address { get; set; }
    }
}