using Maxi.BackOffice.CrossCutting.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Model
{
    public class Currency : ValueObject<Currency>
    {
        public static Currency Default = new UsdCurrency();

        protected Currency(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }

        public string Symbol { get; private set; }

        public string Name { get; private set; }
    }
}
