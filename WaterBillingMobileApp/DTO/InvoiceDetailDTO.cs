using System.Collections.ObjectModel;

namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing detailed information about a water bill invoice.
    /// Contains consumption data, billing period, cost breakdown, and tariff details.
    /// Used in the InvoiceDetailPage to display comprehensive invoice information.
    /// </summary>
    public class InvoiceDetailDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the water meter associated with this invoice.
        /// </summary>
        public int MeterId { get; set; }

        /// <summary>
        /// Gets or sets the previous meter reading value in cubic meters (m³).
        /// </summary>
        public double PreviousReading { get; set; }

        /// <summary>
        /// Gets or sets the current meter reading value in cubic meters (m³).
        /// </summary>
        public double CurrentReading { get; set; }

        /// <summary>
        /// Gets or sets the total water consumption volume for the billing period in cubic meters (m³).
        /// Calculated as CurrentReading - PreviousReading.
        /// </summary>
        public double ConsumptionVolume { get; set; }

        /// <summary>
        /// Gets or sets the start date of the billing period.
        /// </summary>
        public DateTime PeriodStart { get; set; }

        /// <summary>
        /// Gets or sets the end date of the billing period.
        /// </summary>
        public DateTime PeriodEnd { get; set; }

        /// <summary>
        /// Gets or sets the subtotal amount for water consumption before fees and taxes.
        /// </summary>
        public decimal WaterSubtotal { get; set; }

        /// <summary>
        /// Gets or sets the service fee charged for water supply maintenance and operations.
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// Gets or sets the tax amount applied to the invoice (e.g., VAT).
        /// </summary>
        public decimal TaxAmount { get; set; }

        /// <summary>
        /// Gets or sets the collection of tariff brackets applied to calculate the water cost.
        /// Each bracket contains the volume range, rate, and calculated amount.
        /// </summary>
        public ObservableCollection<TariffBreakdownDTO> TariffBreakdown { get; set; } = new();
    }
}