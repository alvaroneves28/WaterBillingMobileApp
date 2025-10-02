namespace WaterBillingMobileApp.Model
{
    /// <summary>
    /// Entity model representing a water meter installed at a customer's location.
    /// Contains meter identification, installation details, and relationships to Customer and Consumption entities.
    /// </summary>
    public class Meter
    {
        /// <summary>
        /// Gets or sets the unique identifier for the meter.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the serial number of the water meter device.
        /// Used for physical identification and tracking of the meter hardware.
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the date when the meter was installed at the customer's location.
        /// </summary>
        public DateTime InstallationDate { get; set; }

        /// <summary>
        /// Gets or sets the current operational status of the meter (e.g., "Active", "Inactive", "Maintenance").
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the customer who owns this meter.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated Customer entity.
        /// Represents the customer who owns and is billed for this meter.
        /// </summary>
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the collection of consumption records for this meter.
        /// Contains the historical meter reading data used for billing calculations.
        /// </summary>
        public ICollection<Consumption> Consumptions { get; set; } = new List<Consumption>();
    }
}