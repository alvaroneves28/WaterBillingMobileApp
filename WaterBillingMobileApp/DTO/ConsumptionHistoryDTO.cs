namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a water consumption history record.
    /// Used to display historical consumption data in the ConsumptionHistoryPage.
    /// </summary>
    public class ConsumptionHistoryDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the water meter.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// Gets or sets the date when the consumption was recorded.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the volume of water consumed in cubic meters (m³).
        /// </summary>
        public double Volume { get; set; }
    }
}