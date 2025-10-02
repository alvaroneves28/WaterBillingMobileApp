namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a single tariff bracket breakdown for invoice calculation.
    /// Used in InvoiceDetailPage to show how the total water cost is calculated across different consumption tiers.
    /// </summary>
    public class TariffBreakdownDTO
    {
        /// <summary>
        /// Gets or sets the description of the tariff bracket (e.g., "First 10 m³", "Next 15 m³").
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the volume of water consumed within this tariff bracket in cubic meters (m³).
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Gets or sets the rate charged per cubic meter for this tariff bracket.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Gets or sets the total amount charged for this tariff bracket.
        /// Calculated as Volume × Rate.
        /// </summary>
        public decimal Amount { get; set; }
    }
}