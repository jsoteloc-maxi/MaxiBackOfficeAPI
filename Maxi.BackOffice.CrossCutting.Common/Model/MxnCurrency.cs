using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Model
{
    public class MxnCurrency : Currency
    {
        public static MxnCurrency Instance = new MxnCurrency();

        public MxnCurrency()
            : base("$", "MXN")
        {
        }
    }
}
