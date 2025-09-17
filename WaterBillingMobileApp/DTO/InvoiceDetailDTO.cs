using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.DTO
{
    public class InvoiceDetailDTO
    {
        public int MeterId { get; set; }
        public double PreviousReading { get; set; }
        public double CurrentReading { get; set; }
        public double ConsumptionVolume { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal WaterSubtotal { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxAmount { get; set; }
        public ObservableCollection<TariffBreakdownDTO> TariffBreakdown { get; set; } = new();
    }
}
