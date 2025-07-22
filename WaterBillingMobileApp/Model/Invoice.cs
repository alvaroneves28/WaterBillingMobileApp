using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.Model
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } 

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int MeterId { get; set; }
        public Meter Meter { get; set; }
    }
}
