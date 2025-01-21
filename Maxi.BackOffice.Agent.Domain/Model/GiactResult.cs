using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class GiactResult
    {
        public long ItemReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ErrorMessage { get; set; }
        
        //public GiactVerificationResponse? VerificationResponse { get; set; }
        public string VerificationResponse { get; set; }

        //public GiactAccountResponseCode? AccountResponseCode { get; set; }
        public string AccountResponseCode { get; set; }

        public string BankName { get; set; }
        public DateTime? AccountAddedDate { get; set; }
        public DateTime? AccountLastUpdatedDate { get; set; }
        public DateTime? AccountClosedDate { get; set; }
        public byte[] VoidedCheckImage { get; set; }

        //public GiactFundsConfirmationResult? FundsConfirmationResult {get;set;}
        public string FundsConfirmationResult { get; set; }


        public List<AccountCautionNote> Notes { get; set; }

        public GiactResult()
        {
            Notes = new List<AccountCautionNote>();
        }

    }
}
