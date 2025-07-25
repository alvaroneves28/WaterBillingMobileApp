using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.DTO
{
    public class MeterStatusDTO
    {
        public int Id { get; set; }
        public string InstallationAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public MeterStatus Status { get; set; }
    }
}
