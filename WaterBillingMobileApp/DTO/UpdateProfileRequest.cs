using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

    }
}
