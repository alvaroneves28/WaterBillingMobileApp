using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class TariffBreakdownDTO
    {
        public string Description { get; set; }
        public double Volume { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
