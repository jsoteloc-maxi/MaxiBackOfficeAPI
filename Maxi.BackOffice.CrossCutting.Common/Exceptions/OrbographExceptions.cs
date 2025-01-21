using System;
using System.Collections.Generic;
using System.Text;

namespace Maxi.BackOffice.CrossCutting.Exceptions
{
    public class OrbographExceptions : Exception
    {
        public OrbographExceptions()
        { }

        public OrbographExceptions(string message)
            : base(message)
        { }

        public OrbographExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
