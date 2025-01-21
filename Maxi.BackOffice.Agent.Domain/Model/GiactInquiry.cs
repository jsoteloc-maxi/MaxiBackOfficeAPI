using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class GiactInquiry
    {    
        public GiactInquiry()
        {
            UniqueId = Guid.NewGuid().ToString();
            GVerifyEnabled = true;
            FundsConfirmationEnabled = true;
            Check = new GiactCheckInformation();

        }
        public string UniqueId { get; set; }
        public bool GVerifyEnabled { get; set; }
        public bool FundsConfirmationEnabled { get; set; }
        //public bool GAuthenticateEnabled { get; set; }
        //public bool VoidedCheckImageEnabled { get; set; }
        //public bool GIdentifyEnabled { get; set; }
        //public bool CustomerIdEnabled { get; set; }
        //public bool OfacScanEnabled { get; set; }
        //public bool GIdentifyEsiEnabled { get; set; }
        //public bool IpAddressInformationEnabled { get; set; }
        //public bool DomainWhoisEnabled { get; set; }
        //public bool MobileVerifyEnabled { get; set; }
        //public bool MobileIdentifyEnabled { get; set; }
        public GiactCheckInformation Check { get; set; }
    }   
}
