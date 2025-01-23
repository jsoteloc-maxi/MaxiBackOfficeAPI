using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.CrossCutting.Common.Common;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace MaxiBackOfficeAPI
{
    public class ApiControllerBase : ApiController
    {
        public AppCurrentSessionContext SessionCtx { get; private set; }

        private event EventHandler<AppCurrentSessionContext> InitSessionContext;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            SessionCtx = Utils.ExtractSessionData(controllerContext.Request);
            InitSessionContext?.Invoke(this, SessionCtx);
        }

        public void InitService(ICustomServiceBase svc)
        {
            InitSessionContext += (o, a) => svc.Initialize(SessionCtx);
        }
    }
}
