using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class CheckTiny
    {
        public int IdCheck { get; set; }

        public DateTime? DateOfIssue { get; set; }
        public DateTime? DateOfMovement { get; set; }

        public decimal Amount { get; set; }

        public int IdIssuer { get; set; }
        public string IssuerName { get; set; }

        public int IdCustomer { get; set; }
        public string CustomerName { get; set; }

        public string CheckNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string Account { get; set; }

        public int IdStatus { get; set; }
        public string StatusName { get; set; }
        public string LastNote { get; set; }

        public int IdAgent { get; set; }
        public string AgentName { get; set; }


        //para consulta de procesados
        public decimal Fee { get; set; }
        public string BaReference { get; set; }
        public decimal BaValFee { get; set; }
        public decimal BaNSFFee { get; set; }
        public decimal BaCredit { get; set; }
        public decimal Balance { get; set; }
        public string BaTypeOfMovement { get; set; }
        public decimal LastBalance { get; set; }


        //para consulta de rechazados
        public int IdRejectReason { get; set; }
        public string RejectReasonName { get; set; }
        public DateTime? RejectDate { get; set; }
        public int RejectByIdUser { get; set; }
        public bool IrdPrinted { get; set; }
        public bool? CanReProcessCheck { get; set; }
        public bool? CanRePrintCheck { get; set; }

    }

}
