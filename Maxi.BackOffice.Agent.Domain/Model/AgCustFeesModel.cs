using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class AgCustFeesModel
    {
        public int IdAgCustFee { get; set; }

        public DateTime ACF_DateCreated { get; set; }

        public int ACF_IdUserCreated { get; set; }

        public int IdAgent { get; set; }

        public int IdCheckType { get; set; }

        public double ACF_CheckAmountFrom { get; set; }

        public double ACF_CheckAmountTo { get; set; }

        public double ACF_FeeFixed { get; set; }

        public double ACF_FeePerc { get; set; }


        public string APIPostAction { get; set; }
    }
}
