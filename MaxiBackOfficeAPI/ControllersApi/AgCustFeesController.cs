using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using System.Web.Http;

namespace MaxiBackOfficeAPI.ControllersApi
{
    [System.Web.Http.Authorize]
    [RoutePrefix("api/AgCustFees")]
    public class AgCustFeesController : ApiControllerBase
    {
        private IAgCustFeesService _svc;

        public AgCustFeesController(IAgCustFeesService svc)
        {
            _svc = svc;
            InitService(_svc);
        }


        [Route("")]
        public AgCustFeesModel GET(int id)
        {
            return _svc.GetById(id);
        }

        [Route("")]
        public List<AgCustFeesModel> GetByAgent(int idAgent)
        {
            return _svc.GetAll(idAgent);
        }

        [Route("")]
        public AgCustFeesModel POST(AgCustFeesModel model)
        {
            var s = (model.APIPostAction ?? "").Trim().ToUpper();

            if (s == "PUT")
            {
                model.IdAgCustFee = _svc.Update(model); //se reusa IdAgCustFee pararegresar un result int
                return model;
            }

            if (s == "DELETE")
            {
                model.IdAgCustFee = _svc.Delete(model.IdAgCustFee); //se reusa IdAgCustFee pararegresar un result int
                return model;
            }

            return _svc.Insert(model);
        }

        [Route("")]
        public int PUT(AgCustFeesModel model)
        {
            return _svc.Update(model);
        }

        [Route("")]
        public int DELETE(int id)
        {
            return _svc.Delete(id);
        }
    }
}
