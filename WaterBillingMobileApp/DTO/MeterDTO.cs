namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a water meter.
    /// Used in meter selection scenarios, such as submitting readings in the MeterReadingPage.
    /// </summary>
    public class MeterDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the water meter.
        /// </summary>
        public int Id { get; set; }
    }
}