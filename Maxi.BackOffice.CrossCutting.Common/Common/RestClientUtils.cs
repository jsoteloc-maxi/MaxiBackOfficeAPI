using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common
{
    public static class RestClientUtils
    {

        private static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }


        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }


        public static HttpRequestMessage CreateRequestMessage(HttpMethod method, string uri, Object content = null)
        {
            var req = new HttpRequestMessage(method, uri);

            if (content != null)
                req.Content = CreateHttpContent(content);

            //if (!string.IsNullOrEmpty(_token))
            //    req.Headers.Add("Authorization", $"Bearer {_token}");

            ////Incluir datos de session
            //req.Headers.Add("X-IdUser", SystemContext.CurrentUser.IdUser.ToString());
            //req.Headers.Add("X-IdAgent", SystemContext.AgentDefault.IdAgent.ToString());
            //req.Headers.Add("X-Culture", SystemContext.CurrentUser.UserSession.Culture);
            //req.Headers.Add("X-SessionGuid", SystemContext.CurrentUser.UserSession.SessionGuid.ToString());
            //req.Headers.Add("X-UserName", SystemContext.CurrentUser.UserName);

            return req;
        }


        

    }
}