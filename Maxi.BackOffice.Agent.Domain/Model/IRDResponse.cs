using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public class IRDResponse
    {
        public string DocType { get; set; }
        public byte[] FrontBytes { get; set; }
        public byte[] RearBytes { get; set; }
        public string IrdMicr { get; set; }

        public Dictionary<string,string> RepParams { get; set; }

        public int ErrorCode { get; set; }
        public string ErrorMsg { get; set; }
    }
}
