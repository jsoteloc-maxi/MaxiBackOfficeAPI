using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class CheckTinyAndSummary
    {
        public List<CheckTiny> CheckTinyRecords { get; set; }
        public CheckProccesedSummary Summary { get; set; }

    }

    public class CheckProccesedSummary
    {
        public int ProccesedChecks { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalFee { get; set; }
        public decimal TotalBaNSFFee { get; set; }
        public decimal TotalBaCredit { get; set; }
    }
}
