using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class AccountCheckSummary
    {
        public int TotalProcessed { get; set; }
        public int TotalRejected { get; set; }
        public DateTime? LastRejectionDate { get; set; }
        public string LastRejectionReason { get; set; }
    }

}
