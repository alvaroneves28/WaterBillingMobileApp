using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.Model
{
    public class Meter
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public DateTime InstallationDate { get; set; }
        public string Status { get; set; }  

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<Consumption> Consumptions { get; set; } = new List<Consumption>();
    }
}
