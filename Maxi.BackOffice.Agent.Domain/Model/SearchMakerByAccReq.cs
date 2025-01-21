using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class SearchMakerByAccReq
    {
        public int  RoutingNum { get; set; }
        public Int64 AccountNum { get; set; }
    }
}
