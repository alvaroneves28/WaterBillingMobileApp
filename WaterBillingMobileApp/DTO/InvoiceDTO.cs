using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterBillingMobileApp.Enums;

namespace WaterBillingMobileApp.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public DateTime IssueDate { get; set; }
        public double TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
    }
}
