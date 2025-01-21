using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class IssuerActionCheck
    {
        public int? IdKYCAction { get; set; }

        public string MessageInSpanish { get; set; }

        public string MessageInEnglish { get; set; }
    }
}
