using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class CCAgFeeCommRes
    {
        public decimal CustFee { get; set; }
        public decimal AgComm { get; set; }
        public decimal CustMaxStatePercFee { get; set; }
        public decimal CustMaxStateFixedFee { get; set; }
    }
}
