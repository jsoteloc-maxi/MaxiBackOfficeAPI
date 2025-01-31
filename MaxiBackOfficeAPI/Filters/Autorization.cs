using System.Net;

namespace MaxiBackOfficeAPI.Filters
{
    //public class Autorization : AuthorizationFilterAttribute
    //{
    //    public override void OnAuthorization(HttpActionContext actionContext)
    //    {
    //        try
    //        {
    //            if (actionContext.Request.Headers.Authorization == null)
    //            {
    //                throw new ArgumentException("Token Requerido");
    //            }
    //            if (Guid.Parse(actionContext.Request.Headers.Authorization.Parameter) == Guid.Empty)
    //            {
    //                throw new ArgumentException("Token Requerido , Usuario no Autenticado");
    //            }

    //            //Validate(Guid.Parse(actionContext.Request.Headers.Authorization.Parameter));

    //        }
    //        catch (Exception ex)
    //        {
    //            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex.Message);
    //        }
    //    }
    //}
}
