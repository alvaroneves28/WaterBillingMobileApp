namespace WaterBillingMobileApp.Model
{
    /// <summary>
    /// Entity model representing a water bill invoice.
    /// Contains billing information and establishes relationships with Customer and Meter entities.
    /// </summary>
    public class Invoice
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date when the invoice was issued.
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Gets or sets the total amount due for this invoice in the local currency.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the current status of the invoice (e.g., "Pending", "Approved", "Rejected").
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the customer who received this invoice.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated Customer entity.
        /// Represents the customer who is billed for this invoice.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the meter associated with this invoice.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated Meter entity.
        /// Represents the water meter for which this invoice was generated.
        /// </summary>
        public Meter Meter { get; set; }
    }
}