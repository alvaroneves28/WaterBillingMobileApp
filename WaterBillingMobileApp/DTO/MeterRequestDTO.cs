using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class MeterRequestDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string InstallationAddress { get; set; }
        public string Comments { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
