using System.Text.Json.Serialization;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.DTO
{
    public class MeterStatusDTO
    {
        /// <summary>
        /// O JSON usa "id" (minúsculo)
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// O JSON usa "installationAddress" (camelCase)
        /// </summary>
        [JsonPropertyName("installationAddress")]
        public string InstallationAddress { get; set; }

        /// <summary>
        /// O JSON usa "requestDate" (camelCase)
        /// </summary>
        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// O JSON usa "status" como número (0=Pending, 1=Approved, 2=Rejected)
        /// O JsonConverter irá automaticamente converter para o enum
        /// </summary>
        [JsonPropertyName("status")]
        public MeterStatus Status { get; set; }
    }
}
