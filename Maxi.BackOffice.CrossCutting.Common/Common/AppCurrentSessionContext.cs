using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Common
{
    public interface IAppCurrentSessionContext
    {
        string SessionGuid { get; set; }
        int IdAgent { get; set; }
        int IdUser { get; set; }
        string UserName { get; set; }
        int IdLang { get => ResolveLangId(); }
        string Culture { get; set; }
        string PcName { get; set; }
        string PcIdentifier { get; set; }
        string PcSerial { get; set; }

        int ResolveLangId();
    }

    public class AppCurrentSessionContext: IAppCurrentSessionContext
    {
        public string SessionGuid { get; set; }

        public int IdAgent { get; set; }

        public int IdUser { get; set; }
        public string UserName { get; set; }

        public int IdLang { get => ResolveLangId(); }
        public string Culture { get; set; }

        public string PcName { get; set; } = "";
        public string PcIdentifier { get; set; } = "";
        public string PcSerial { get; set; } = "";


        public AppCurrentSessionContext()
        {
            IdAgent = 0;
            IdUser = 0;
            Culture = "en-US";
            //IdLang = 1;
            SessionGuid = "";
            UserName = "";
        }

        public int ResolveLangId()
        {
            int m = 1;
            var s = Culture.ToLower().Trim();
            if (s.StartsWith("es-"))
                m = 2;
            return m;
        }


    }
}
