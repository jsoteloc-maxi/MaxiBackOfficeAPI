using System;
using System.Collections.Generic;
using System.Text;

namespace Maxi.BackOffice.CrossCutting.Exceptions
{
    public class GoogleOCRExceptions : Exception
    {
        public GoogleOCRExceptions()
        { }

        public GoogleOCRExceptions(string message)
            : base(message)
        { }

        public GoogleOCRExceptions(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
