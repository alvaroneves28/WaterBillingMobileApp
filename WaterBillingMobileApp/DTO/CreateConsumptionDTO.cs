namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object for creating a new water consumption record.
    /// Used when submitting meter readings through the MeterReadingPage.
    /// </summary>
    public class CreateConsumptionDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the water meter.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// Gets or sets the date when the meter reading was taken.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the meter reading value in cubic meters (m³).
        /// </summary>
        public double Value { get; set; }
    }
}