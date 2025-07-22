using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.Model
{
    public class Consumption
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }

        public int MeterId { get; set; }
        public Meter Meter { get; set; }
    }
}
