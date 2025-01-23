using Newtonsoft.Json;
using Maxi.BackOffice.CrossCutting.Common;
using Maxi.BackOffice.Agent.Infrastructure.Common;

namespace Maxi.BackOffice.Agent.Infrastructure.ExternalServices
{
    public class MaxiCommonServices
    {
        private static HttpClient _hClient = null;
        private static HttpClient hClient
        {
            get
            {
                if (_hClient == null)
                    _hClient = CreateHttpClient();
                return _hClient;
            }
        }


        private static HttpClient CreateHttpClient()
        {
            string uriString = @"http://192.168.3.8:8080/CommonServices/";
            var hc = new HttpClient { BaseAddress = new Uri(uriString) };
            return hc;
        }



        private static void RequestAddHeaders(HttpRequestMessage req)
        {
            //Por si se quiere agregar headers

            //if (!string.IsNullOrEmpty(_token))
            //    req.Headers.Add("Authorization", $"Bearer {_token}");

            ////Incluir datos de session
            //req.Headers.Add("X-IdUser", SystemContext.CurrentUser.IdUser.ToString());
            //req.Headers.Add("X-IdAgent", SystemContext.AgentDefault.IdAgent.ToString());
            //req.Headers.Add("X-Culture", SystemContext.CurrentUser.UserSession.Culture);
            //req.Headers.Add("X-SessionGuid", SystemContext.CurrentUser.UserSession.SessionGuid.ToString());
            //req.Headers.Add("X-UserName", SystemContext.CurrentUser.UserName);
        }

        private static async Task<HttpResponseMessage> HttpPostAsync(string uri, Object content)
        {
            var req = RestClientUtils.CreateRequestMessage(HttpMethod.Post, uri, content);

            RequestAddHeaders(req);

            var res = await hClient.SendAsync(req);
            return res;

            //bool evalTokenExp = true;
            //await AquireAccessToken();
            //while (true)
            //{
            //    var req = CreateRequestMessage(HttpMethod.Post, uri, content);
            //    var res = await hClient.SendAsync(req);
            //    if (evalTokenExp)
            //    {
            //        evalTokenExp = false;
            //        if (await EvaluateTokenExpired(res))
            //            continue;
            //    }
            //    return res;
            //}
        }



        public static ExportReportValue Report(string reportName, object parameters, ExportReportType repType=ExportReportType.PDF)
        {
            GLogger.Debug("MaxiCommonServices Report");

            var content = new
            {
                ReportName = reportName,
                Parameters = parameters,
                ExportType = repType,
            };
            
            var req = RestClientUtils.CreateRequestMessage(HttpMethod.Post, "Report", content);

            GLogger.Debug("  Call Service Report ");
            var res = hClient.SendAsync(req).Result;
            res.EnsureSuccessStatusCode();

            //var rep = res.Content.ReadAsAsync<ExportReportResult>().Result;

            var s = res.Content.ReadAsStringAsync().Result;
            GLogger.Debug("  ReportResult: " + s);
            var rep = JsonConvert.DeserializeObject<ExportReportResult>(s);

            if (rep == null)
                throw new Exception("Export report result null");

            if (rep.Value == null)
                throw new Exception("Export report result null");

            return rep.Value;
        }


    }

    public class ExportReportResult
    {
        public ExportReportValue Value { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ExportReportValue
    {
        public Byte[] Bytes { get; set; }
        public string MimeType { get; set; }
        public string Encoding { get; set; }
        public string FileNameExtension { get; set; }

        public string[] Streams { get; set; }
        public string[] Warnings { get; set; }
    }

    public enum ExportReportType
    {
        PDF,
        CSV,
        WORDOPENXML,
        EXCELOPENXML,
    }


}
