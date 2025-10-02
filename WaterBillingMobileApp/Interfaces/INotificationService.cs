using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterBillingMobileApp.Interfaces
{
    public interface INotificationService
    {
        Task<bool> CheckForNewInvoicesAsync();
        void StartPeriodicCheck();
        Task CheckOnAppStartAsync();
        void ClearNotificationData();
        Task ForceCheckAsync();
    }
}
