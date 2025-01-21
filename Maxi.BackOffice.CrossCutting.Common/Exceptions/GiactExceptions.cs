using System;
using System.Collections.Generic;
using System.Text;

namespace Maxi.BackOffice.CrossCutting.Exceptions
{
    public class GiactExceptions : Exception
    {
        public GiactExceptions()
        { }

        public GiactExceptions(string message)
            : base(message)
        { }

        public GiactExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
