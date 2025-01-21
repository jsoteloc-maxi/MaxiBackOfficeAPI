using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Model
{
    public class CopCurrency : Currency
    {
        public static CopCurrency Instance = new CopCurrency();

        public CopCurrency()
            : base("$", "COP")
        {
        }
    }
}
