using Maxi.BackOffice.CrossCutting.Common.Common;
using System.Net.Http.Headers;

namespace MaxiBackOfficeAPI
{
    public  class Utils
    {

        private readonly IConfiguration _configuration;

        public Utils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static int HeaderAsInt(HttpRequestHeaders hdr, string name, int def = 0)
        {
            int i = def;
            if (hdr.TryGetValues(name, out IEnumerable<string> values))
            {
                var s = values.First();
                int.TryParse(s, out i);
            }
            return i;
        }

        private static string HeaderAsStr(HttpRequestHeaders hdr, string name, string def = "")
        {
            string s = def;
            if (hdr.TryGetValues(name, out IEnumerable<string> values))
                s = values.First();
            return s;
        }

        //Extraer la informacion de custom headers
        public static AppCurrentSessionContext ExtractSessionData(HttpRequestMessage req)
        {
            var _ctx = new AppCurrentSessionContext
            {
                IdAgent = HeaderAsInt(req.Headers, "X-IdAgent"),
                IdUser = HeaderAsInt(req.Headers, "X-IdUser"),
                //IdLang = HeaderAsInt(req.Headers, "X-IdLang", 1),
                Culture = HeaderAsStr(req.Headers, "X-Culture", "en-US"),
                SessionGuid = HeaderAsStr(req.Headers, "X-SessionGuid"),
                UserName = HeaderAsStr(req.Headers, "X-UserName"),

                PcName = HeaderAsStr(req.Headers, "X-PcName"),
                PcSerial = HeaderAsStr(req.Headers, "X-PcSerial"),
                PcIdentifier = HeaderAsStr(req.Headers, "X-PcIdentifier"),
            };
            return _ctx;
        }


        public static dynamic GetJwtConfig()
        {
           // if (!Int32.TryParse(_configuration["Jwt_Expires"], out int expires))
           int  expires = 24;

            var r = new
            {
                SecretKey = "f00eb976-917b-4bc1-a2cd-8b96cba2d7d8",
                Issuer = "www.maxi-ms.com",
                Audience = "maxi webapi",
                Expires = expires,
            };

            if (string.IsNullOrEmpty(r.SecretKey))
                throw new Exception("SecretKey no encontrado");

            return r;

            //return new
            //{
            //    SecretKey = ConfigurationManager.AppSettings["Jwt_Secret"],
            //    Issuer = ConfigurationManager.AppSettings["Jwt_Issuer"],
            //    Audience = ConfigurationManager.AppSettings["Jwt_Audience"],
            //    Expires = expires,
            //};
        }
    }
}
