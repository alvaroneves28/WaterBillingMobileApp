using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.Model
{
    public class PriceList
    {
        public int Id { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Description { get; set; }
    }
}
