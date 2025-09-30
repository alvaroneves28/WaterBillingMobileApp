using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class TariffDTO
    {
        [JsonPropertyName("minVolume")]
        public double MinVolume { get; set; }

        [JsonPropertyName("maxVolume")]
        public double? MaxVolume { get; set; }  

        [JsonPropertyName("pricePerCubicMeter")]
        public decimal PricePerCubicMeter { get; set; }
    }
}
