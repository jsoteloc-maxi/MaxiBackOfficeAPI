using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class CheckElementEdited
    {
        public int IdCheckEdits { get; set; }

        public int IdCheck { get; set; }

        public string EditName { get; set; }

        public string OriValue { get; set; }

        public int OriScore { get; set; }

        public string Value { get; set; }

        public int EditLevel { get; set; }
    }
}
