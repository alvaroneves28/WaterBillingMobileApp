using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class TariffDTO
    {
        public double MinVolume { get; set; }
        public double MaxVolume { get; set; }
        public decimal PricePerCubicMeter { get; set; }
    }
}
