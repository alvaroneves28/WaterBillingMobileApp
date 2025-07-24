using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class ConsumptionHistoryDTO
    {
        public int MeterId { get; set; }
        public DateTime Date { get; set; }
        public double Volume { get; set; }
    }
}
