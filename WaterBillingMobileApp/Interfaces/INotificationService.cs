namespace WaterBillingMobileApp.Interfaces
{
    /// <summary>
    /// Interface for notification service operations.
    /// Handles detection and notification of new invoices to keep users informed.
    /// Manages periodic checks and notification state persistence.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Checks if there are new invoices available since the last check.
        /// Compares current invoices with the last known state and displays notifications for new items.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing true if new invoices were found, false otherwise.</returns>
        Task<bool> CheckForNewInvoicesAsync();

        /// <summary>
        /// Starts a periodic background check for new invoices.
        /// The check runs automatically every 30 minutes to keep the user informed of new billing activity.
        /// </summary>
        void StartPeriodicCheck();

        /// <summary>
        /// Performs an initial check for new invoices when the application starts.
        /// Includes a short delay to allow the app to fully load before checking.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CheckOnAppStartAsync();

        /// <summary>
        /// Clears all stored notification data including last check time and invoice count.
        /// Typically called during logout to reset the notification state for the next user.
        /// </summary>
        void ClearNotificationData();

        /// <summary>
        /// Forces an immediate check for new invoices, bypassing the normal check interval.
        /// Useful for manual refresh or testing purposes.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ForceCheckAsync();
    }
}