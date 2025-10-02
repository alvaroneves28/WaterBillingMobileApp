namespace WaterBillingMobileApp.Enums
{
    /// <summary>
    /// Represents the possible statuses of a water bill invoice.
    /// Used to track the approval and payment state of invoices.
    /// </summary>
    public enum InvoiceStatus
    {
        /// <summary>
        /// The invoice is pending review or payment.
        /// This is the initial status when an invoice is created.
        /// </summary>
        Pending,

        /// <summary>
        /// The invoice has been approved and is ready for payment or has been paid.
        /// </summary>
        Approved,

        /// <summary>
        /// The invoice has been rejected, possibly due to disputed charges or errors.
        /// </summary>
        Rejected
    }
}