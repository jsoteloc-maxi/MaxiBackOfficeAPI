using Maxi.BackOffice.Agent.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MaxiBackOfficeAPI.Filters
{
    //public class CatchAllExceptionFilterAttribute : ExceptionFilterAttribute
    //{
    //    public override void OnException(HttpActionExecutedContext context)
    //    {
    //        GLogger.Info("--------");
    //        GLogger.Info(context.ActionContext.Request.RequestUri.LocalPath);
    //        GLogger.Error(context.Exception);

    //        //var o = new { Message = context.Exception.Message };

    //        ////new System.Net.Http.ObjectContent(Object, o,)

    //        //context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
    //        //{
    //        //    //Content = new StringContent(o);
    //        //};
    //    }
    //}
}
