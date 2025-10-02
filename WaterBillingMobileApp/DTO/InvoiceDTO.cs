using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a summary of a water bill invoice.
    /// Used to display invoice lists in the InvoicesPage.
    /// </summary>
    public class InvoiceDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the invoice.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date when the invoice was issued.
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Gets or sets the total amount to be paid for this invoice in the local currency.
        /// </summary>
        public double TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the current status of the invoice (Pending, Approved, or Rejected).
        /// </summary>
        public InvoiceStatus Status { get; set; }
    }
}