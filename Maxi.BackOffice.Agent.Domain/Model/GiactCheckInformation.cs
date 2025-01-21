using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class GiactCheckInformation
    {
        // Check.AccountType = "Checking"
        public GiactCheckInformation()
        {
            AccountType = GiactAccountType.Checking;
        }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string CheckNumber { get; set; }
        public decimal CheckAmount { get; set; }
        public GiactAccountType AccountType { get; set; }
    }
}
