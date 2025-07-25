using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class UpdateEmailRequest
    {
        public string CurrentPassword { get; set; }
        public string NewEmail { get; set; }
    }
}
