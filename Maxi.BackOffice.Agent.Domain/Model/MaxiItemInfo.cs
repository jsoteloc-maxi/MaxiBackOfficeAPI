using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{

    public class MaxiItemInfo
    {
        //Constructor
        public MaxiItemInfo()
        {
            Ocr = new ImageOcr();
            Maker = new MaxiItemInfoMaker();
            Customer = new MaxiItemInfoCustomer();

            //WarningsAlerts = new List<string>();

            AccSummary = new AccountCheckSummary();
            AccCautionNotes = new AccountCautionNotes();
        }

        //todo: armar elementos de respuesta
        public string ItemName { get; set; }
        public string Micr { get; set; }
        public string RoutingNum { get; set; }
        public string AccountNum { get; set; }
        public string CheckNum { get; set; }
        public string MicrOnUs { get; set; }

        public DateTime? CheckDate { get; set; }
        public int CheckDateScore { get; set; } = 0;

        public Decimal CheckAmount { get; set; }
        public int CheckAmountScore { get; set; } = 0;

        public string ItemDocType { get; set; }

        public string CheckCanBeProcessed { get; set; }  // true si el cheque puede ser procesado, false si hay algun bloqueo
        //public List<string> WarningsAlerts { get; set; }

        public ImageOcr Ocr { get; set; }
        public MaxiItemInfoMaker Maker { get; set; }
        public MaxiItemInfoCustomer Customer { get; set; }

        //Todo:  AccountReserchResult

        public AccountCautionNotes AccCautionNotes { get; set; }
        public AccountCheckSummary AccSummary { get; set; }

        //Rquerimiento Maxi-014
        //Resultado de lectura para firma de cheque
        public bool IsSigned { get; set; }

        public Guid IdImage { get; set; }

        public bool IsCompanyCheck { get; set; }
    }


    public class MaxiItemInfoMaker
    {
        public int IdMaker { get; set; } = 0;
        public string MakerName { get; set; } = "";
        public string MakerAddress { get; set; } = "";
        public string MakerCity { get; set; } = "";
        public string MakerState { get; set; } = "";
        public string MakerZip { get; set; } = "";
        public string MakerTel { get; set; } = "";
    }

    public class MaxiItemInfoCustomer
    {
        public int IdCustomer { get; set; } = 0;
        public string CustName { get; set; } = "";
        public string CustAddress { get; set; } = "";
        public string CustCity { get; set; } = "";
        public string CustState { get; set; } = "";
        public string CustZip { get; set; } = "";
        public string CustTel { get; set; } = "";
    }


    public class ImageOcr
    {
        public string DocType { get; set; } = "";
        public string DocName { get; set; } = "";
        public int ErrorCode { get; set; }
        public int Score { get; set; }

        public int MicrAmountScore { get; set; } = 0;
        public string MicrAmount { get; set; } = "";

        public int RoutingScore { get; set; } = 0;
        public string Routing { get; set; } = "";

        public int OnUsAccountScore { get; set; } = 0;
        public string OnUsAccount { get; set; } = "";

        public int OnUsCheckScore { get; set; } = 0;
        public string OnUsCheck { get; set; } = "";

        public int MicrScore { get; set; } = 0;
        public string Micr { get; set; } = "";

        public int PayerScore { get; set; } = 0;
        public string PayerName { get; set; } = "";
    }

}
