using Maxi.BackOffice.Agent.Infrastructure.Common;

namespace MaxiBackOfficeAPI.MessageHandler
{
    internal class LogRequestResponseHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.RequestUri.LocalPath.ToLower().Contains("/api/login"))
                {
                    return await base.SendAsync(request, cancellationToken);
                }

                if (request.Content != null)
                {
                    // log request body
                    GLogger.Debug(request.RequestUri.LocalPath + " REQUEST:");
                    GLogger.Debug(await ReadSomeChars(request.Content, 200));
                }

                // let other handlers process the request
                var result = await base.SendAsync(request, cancellationToken);

                if (result.Content != null)
                {
                    // once response body is ready, log it
                    GLogger.Debug(request.RequestUri.LocalPath + " RESPONSE:");
                    GLogger.Debug(await ReadSomeChars(result.Content, 200));
                }

                return result;
            }
            catch (Exception ex)
            {
                //hay errores que no se logean con el filtro => se pone esto aqui
                GLogger.Info("--------");
                GLogger.Debug(request.RequestUri.LocalPath);
                GLogger.Error(ex);
                throw ex;
            }
        }

        private async Task<string> ReadSomeChars(HttpContent content, int ln)
        {
            var strEnd = " ...";
            string responseContent;
            Stream responseStream = await content.ReadAsStreamAsync().ConfigureAwait(false);

            int totalBytesRead = 0;
            byte[] buffer = new byte[ln];

            while (totalBytesRead < buffer.Length)
            {
                int bytesRead = await responseStream
                    .ReadAsync(buffer, totalBytesRead, buffer.Length - totalBytesRead);

                if (bytesRead == 0)
                {
                    // end-of-stream...can't read any more
                    strEnd = "";
                    break;
                }

                totalBytesRead += bytesRead;
            }

            responseStream.Position = 0; //Muy importante colocar al inicio

            MemoryStream tempStream = new MemoryStream(buffer, 0, totalBytesRead);

            using (StreamReader streamReader = new StreamReader(tempStream))
            {
                responseContent = streamReader.ReadToEnd() + strEnd;
                // responseContent = Data from streamReader until ResponseContentMaxLength
            }

            return responseContent;
        }
    }
}
