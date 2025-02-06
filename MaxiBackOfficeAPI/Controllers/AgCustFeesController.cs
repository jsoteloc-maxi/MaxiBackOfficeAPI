using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiBackOfficeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AgCustFeesController : ControllerBase
    {
        private IAgCustFeesService _agCustFeesService;

        public AgCustFeesController(IAgCustFeesService agCustFeesService)
        {
            _agCustFeesService = agCustFeesService;
        }

        [HttpGet]
        public AgCustFeesModel GET([FromQuery] int id)
        {
            return _agCustFeesService.GetById(id);
        }

        [HttpGet]
        public List<AgCustFeesModel> GetByAgent([FromQuery] int idAgent)
        {
            return _agCustFeesService.GetAll(idAgent);
        }

        /// <summary>
        /// TODO verificar la llamada usando FromBody
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public AgCustFeesModel POST([FromBody] AgCustFeesModel model)
        {
            var s = (model.APIPostAction ?? "").Trim().ToUpper();

            if (s == "PUT")
            {
                model.IdAgCustFee = _agCustFeesService.Update(model); //se reusa IdAgCustFee pararegresar un result int
                return model;
            }

            if (s == "DELETE")
            {
                model.IdAgCustFee = _agCustFeesService.Delete(model.IdAgCustFee); //se reusa IdAgCustFee pararegresar un result int
                return model;
            }

            return _agCustFeesService.Insert(model);
        }

        /// <summary>
        /// TODO este metodo no se manda llamar desde H1
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        public int PUT([FromBody] AgCustFeesModel model)
        {
            return _agCustFeesService.Update(model);
        }

        /// <summary>
        /// TODO este metodo no se manda llamar desde H1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public int DELETE([FromBody] int id)
        {
            return _agCustFeesService.Delete(id);
        }
    }
}
