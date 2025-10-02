using System.Text.Json.Serialization;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.DTO
{
    /// <summary>
    /// Data Transfer Object representing the status of a water meter installation request.
    /// Used in the RatesAndStatusPage to display meter request information.
    /// JSON property names are mapped to match the API's camelCase convention.
    /// </summary>
    public class MeterStatusDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the meter request.
        /// JSON property name: "id" (lowercase).
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the address where the meter is or will be installed.
        /// JSON property name: "installationAddress" (camelCase).
        /// </summary>
        [JsonPropertyName("installationAddress")]
        public string InstallationAddress { get; set; }

        /// <summary>
        /// Gets or sets the date when the meter installation was requested.
        /// JSON property name: "requestDate" (camelCase).
        /// </summary>
        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Gets or sets the current status of the meter request.
        /// JSON property name: "status" (lowercase).
        /// API sends numeric values: 0 = Pending, 1 = Approved, 2 = Rejected.
        /// The JSON converter automatically maps these to the MeterStatus enum.
        /// </summary>
        [JsonPropertyName("status")]
        public MeterStatus Status { get; set; }
    }
}