using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.Model
{
    public class Customer
    {
        public int Id { get; set; }

        
        public string Email { get; set; }
        public string PasswordHash { get; set; }  

        
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        
        public string? ProfilePictureUrl { get; set; }

       
        public bool IsActive { get; set; }  

       
        public ICollection<Meter> Meters { get; set; } = new List<Meter>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
