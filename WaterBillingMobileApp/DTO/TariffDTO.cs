using System.Text.Json.Serialization;

namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing a water tariff bracket.
    /// Defines the pricing structure for water consumption based on volume ranges.
    /// Used in RatesAndStatusPage and AnonymousRequestPage to display current pricing information.
    /// JSON property names are mapped to match the API's camelCase convention.
    /// </summary>
    public class TariffDTO
    {
        /// <summary>
        /// Gets or sets the minimum water volume for this tariff bracket in cubic meters (m³).
        /// JSON property name: "minVolume" (camelCase).
        /// </summary>
        [JsonPropertyName("minVolume")]
        public double MinVolume { get; set; }

        /// <summary>
        /// Gets or sets the maximum water volume for this tariff bracket in cubic meters (m³).
        /// Nullable to support open-ended brackets (e.g., "50+ m³").
        /// JSON property name: "maxVolume" (camelCase).
        /// </summary>
        [JsonPropertyName("maxVolume")]
        public double? MaxVolume { get; set; }

        /// <summary>
        /// Gets or sets the price charged per cubic meter within this tariff bracket.
        /// JSON property name: "pricePerCubicMeter" (camelCase).
        /// </summary>
        [JsonPropertyName("pricePerCubicMeter")]
        public decimal PricePerCubicMeter { get; set; }
    }
}