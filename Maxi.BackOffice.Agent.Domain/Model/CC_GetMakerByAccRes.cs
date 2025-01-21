using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class CC_GetMakerByAccRes
    {
        public int Maker_ID { get; set; }

        public DateTime MAK_DateCreated { get; set; }

        public int MAK_IdUserCreated { get; set; }

        public string MAK_Name { get; set; }

        public string MAK_Address { get; set; }

        public string MAK_City { get; set; }

        public string MAK_State { get; set; }

        public int IdState { get; set; }

        public string MAK_ZipCode { get; set; }

        public bool MAK_Active { get; set; }
        public string RoutingNum { get; set; }
        public string AccountNum { get; set; }
    }
}
