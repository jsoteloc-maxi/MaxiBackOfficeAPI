﻿using Maxi.BackOffice.CrossCutting.Common.Common;

namespace MaxiBackOfficeAPI.Middleware
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public class SessionContextMiddleware
    {
        private readonly RequestDelegate _next;
        public SessionContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAppCurrentSessionContext appCurrentSessionContext)
        {
            appCurrentSessionContext.IdAgent = 
                context.Request.Headers.TryGetValue("X-IdAgent", out var headerIdAgent) ? 
                Convert.ToInt32(headerIdAgent.ToString()) : 0;
            appCurrentSessionContext.IdUser =
                context.Request.Headers.TryGetValue("X-IdUser", out var headerIdUser) ?
                Convert.ToInt32(headerIdUser.ToString()) : 0;
            appCurrentSessionContext.Culture =
                context.Request.Headers.TryGetValue("X-Culture", out var headerCulture) ?
                headerCulture.ToString() : "en-US";
            appCurrentSessionContext.SessionGuid =
                context.Request.Headers.TryGetValue("X-SessionGuid", out var headerSessionGuid) ?
                headerSessionGuid.ToString() : string.Empty;
            appCurrentSessionContext.UserName =
                context.Request.Headers.TryGetValue("X-UserName", out var headerUserName) ?
                headerUserName.ToString() : string.Empty;
            appCurrentSessionContext.PcName =
                context.Request.Headers.TryGetValue("X-PcName", out var headerPcName) ?
                headerPcName.ToString() : string.Empty;
            appCurrentSessionContext.PcSerial =
                context.Request.Headers.TryGetValue("X-PcSerial", out var headerPcSerial) ?
                headerPcSerial.ToString() : string.Empty;
            appCurrentSessionContext.PcIdentifier =
                context.Request.Headers.TryGetValue("X-PcIdentifier", out var headerPcIdentifier) ?
                headerPcIdentifier.ToString() : string.Empty;


            // JISC TODO: ya no se utiliza AppCurrentSessionContext en el context ya que se uso DI
            //context.Items["AppCurrentSessionContext"] = appCurrentSessionContext;

            //if (context.Request.Headers.TryGetValue("X-IdAgent", out var headerValue))
            //{
            //    appCurrentSessionContext.IdAgent = Convert.ToInt32(headerValue.ToString());
            //}
            //else
            //{
            //    context.Response.StatusCode = 400;
            //    await context.Response.WriteAsync("Missing X-IdAgent");
            //    return;
            //}

            // Continuar con el pipeline
            await _next(context);
        }
    }
}
