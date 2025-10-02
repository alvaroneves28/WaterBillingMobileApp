namespace WaterBillingMobileApp.Enums
{
    /// <summary>
    /// Represents the possible statuses of a water meter installation request.
    /// Used to track the approval state of meter requests from submission to installation.
    /// The numeric values (0, 1, 2) match the API's representation for proper serialization.
    /// </summary>
    public enum MeterStatus
    {
        /// <summary>
        /// The meter installation request is pending review by the water utility company.
        /// This is the initial status when a request is submitted.
        /// Numeric value: 0
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The meter installation request has been approved and is scheduled for installation.
        /// Numeric value: 1
        /// </summary>
        Approved = 1,

        /// <summary>
        /// The meter installation request has been rejected, possibly due to invalid information or technical constraints.
        /// Numeric value: 2
        /// </summary>
        Rejected = 2
    }
}