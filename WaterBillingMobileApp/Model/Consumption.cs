namespace WaterBillingMobileApp.Model
{
    /// <summary>
    /// Entity model representing a water consumption record for a specific meter.
    /// Stores meter reading data and establishes a relationship with the Meter entity.
    /// </summary>
    public class Consumption
    {
        /// <summary>
        /// Gets or sets the unique identifier for the consumption record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date when the consumption reading was recorded.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the meter reading value in cubic meters (m³).
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the associated meter.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated Meter entity.
        /// Represents the water meter that this consumption record belongs to.
        /// </summary>
        public Meter Meter { get; set; }
    }
}