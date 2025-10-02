namespace WaterBillingMobileApp.Model
{
    /// <summary>
    /// Entity model representing a customer in the water billing system.
    /// Contains authentication credentials, personal information, and relationships to meters and invoices.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the customer's email address used for authentication and communication.
        /// Must be unique across all customers.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the hashed password for customer authentication.
        /// Stored as a hash for security purposes, never stored in plain text.
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the customer's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the customer's phone number for contact purposes.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the customer's residential or billing address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the customer's profile picture.
        /// Nullable to support customers without a profile picture.
        /// </summary>
        public string? ProfilePictureUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer account is active.
        /// Inactive accounts cannot log in or access services.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the collection of water meters associated with this customer.
        /// A customer can have multiple meters at different locations.
        /// </summary>
        public ICollection<Meter> Meters { get; set; } = new List<Meter>();

        /// <summary>
        /// Gets or sets the collection of invoices issued to this customer.
        /// Represents the billing history for all meters owned by the customer.
        /// </summary>
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}