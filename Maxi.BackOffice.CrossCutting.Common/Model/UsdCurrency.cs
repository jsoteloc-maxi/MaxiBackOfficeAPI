using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Model
{
    public class UsdCurrency : Currency
    {
        public static UsdCurrency Instance = new UsdCurrency();

        public UsdCurrency()
            : base("$", "USD")
        {
        }
    }
}
