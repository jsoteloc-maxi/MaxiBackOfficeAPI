using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Model
{
    public class EurCurrency : Currency
    {
        public static EurCurrency Instance = new EurCurrency();

        public EurCurrency()
            : base("€", "EUR")
        {
        }
    }
}
