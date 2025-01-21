using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class CheckType
    {
        public int IdCheckType { get; set; }
        public DateTime CT_DateCreated { get; set; }
        public string CT_Name { get; set; }
        public bool CT_Active { get; set; }
    }
}
