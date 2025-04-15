using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.CrossCutting.Common.Common;
using MaxiBackOfficeAPI.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;

namespace MaxiBackOfficeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RpcController : ControllerBase
    {
        private IRpcService _rpcService;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;

        public RpcController(IRpcService rpcService, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _rpcService = rpcService;
            _appCurrentSessionContext = appCurrentSessionContext;
        }

        [HttpGet("healthcheck")]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult HealthCheck()
        {
            return Ok(new { Success = true, Message = "Application is working!!" });
        }

        /// <summary>
        /// Obtiene la lista de tipos de cheque
        /// </summary>
        /// <returns></returns>
        [HttpGet("CheckTypesList")]
        public IActionResult GetCheckTypesList()
        {
            return Ok(_rpcService.GetCheckTypesList());
        }

        /// <summary>
        /// Analiza la imagen de un cheque para intenta obtener toda la informacion posible
        /// consume la API de Orbograph
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("ValidateCheckImage")]
        public MaxiItemInfo ValidateCheckImage(ObjetoRequestItem req)
        {
            return _rpcService.GetItemInfo(req);
        }

        /// <summary>
        /// Consulta informacion en el servicio GIACT
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("ValidateCheckGiact")]
        public GiactResult ValidateCheckGiact(GiactInquiry req)
        {
            return _rpcService.GiactValidation(req);
        }

        /// <summary>
        /// Hace revision de reglas y regresa notas al respecto
        /// </summary>
        /// <param name="rout"></param>
        /// <param name="acc"></param>
        /// <param name="checkNum"></param>
        /// <returns></returns>
        [HttpGet("AccountCautionNotes")]
        public AccountCautionNotes GetCautionNotes(string rout, string acc, string checkNum = "")
        {
            //Api para llamada manual de GetAccountCautionNotes
            //Todo: se va a necesitar tambien recibir el numero de cheque para validar si ya hay un ird procesado y rechazado

            return _rpcService.GetAccountCautionNotes(rout, acc, checkNum);
        }

        /// <summary>
        /// Hace revision de reglas y regresa notas al respecto
        /// </summary>
        /// <param name="idIssuer"></param>
        /// <returns></returns>
        [HttpGet("IssuerAction")]
        public List<IssuerActionCheck> GetIssuerAction(int idIssuer)
        {
            //Api para llamada manual de GetAccountCautionNotes
            //Todo: se va a necesitar tambien recibir el numero de cheque para validar si ya hay un ird procesado y rechazado

            return _rpcService.GetIssuerAction(idIssuer);
        }

        /// <summary>
        /// Busca resumen historico de cheques procesados
        /// </summary>
        /// <param name="rout"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpGet("AccountCheckSummary")]
        public AccountCheckSummary GetAccountCheckSummary(string rout, string acc)
        {
            return _rpcService.GetAccountCheckSummary(rout, acc);
        }

        /// <summary>
        /// Busca Maker usando routing y account
        /// </summary>
        /// <param name="rout"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        [HttpGet("GetMakerByAcc")]
        public CC_GetMakerByAccRes GetMakerByAcc(string rout, string acc)
        {
            return _rpcService.GetMakerByAcc(rout, acc);
        }//Este metodo se llama cada que se edita el cheque

        /// <summary>
        /// Lista de cheques mas recientes del customer
        /// </summary>
        [HttpGet("RecentChecksByCustomer")]
        public List<CheckTiny> GetRecentChecksByCustomer(int idCustomer)
        {
            return _rpcService.GetRecentChecksByCustomer(idCustomer);
        }

        /// <summary>
        /// Listado de cheques mas reciente del Issuer
        /// </summary>
        /// <param name="idIssuer"></param>
        /// <returns></returns>
        [HttpGet("RecentChecksByIssuer")]
        public List<CheckTiny> GetRecentChecksByIssuer(int idIssuer)
        {
            return _rpcService.GetRecentChecksByIssuer(idIssuer);
        }

        /// <summary>
        /// Listado de cheques mas reciente del Issuer paginado
        /// </summary>
        /// <param name="idIssuer"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="paged"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrden"></param>
        /// <returns></returns>
        [HttpGet("RecentChecksByIssuerPaged")]
        public IActionResult GetRecentChecksByIssuerV2(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrden = null)
        {
            var result = _rpcService.GetRecentChecksByIssuer(idIssuer, startDate, endDate, paged, offset, limit, sortColumn, sortOrden);
            if (!paged.Value)

                return Ok(result.Data);
            else
                return Ok(result);

        }

        /// <summary>
        /// Listado de cheques mas reciente del Customer paginado
        /// </summary>
        /// <param name="idCustomer"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="paged"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="sortColumn"></param>
        /// <param name="sortOrder"></param>
        /// <returns></returns>
        [HttpGet("RecentChecksByCustomerPaged")]
        public IActionResult GetRecentChecksByCustomerV2(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null)
        {
            var result = _rpcService.GetRecentChecksByCustomer(idCustomer, startDate, endDate, paged, offset, limit, sortColumn, sortOrder);
            if (!paged.Value)
                return Ok(result.Data);
            else
                return Ok(result);

        }

        [HttpGet("GetAgFeeComm")]
        public CCAgFeeCommRes GetAgFeeComm(int idAgent, decimal amount, int idState)
        {
            return _rpcService.GetAgFeeCommInfo(idAgent, amount, idState);
        }

        /// <summary>
        /// Deprecated ,  se deja como referencia
        /// </summary>
        /// <returns></returns>
        //private IActionResult TestGiact()
        //{
        //    var chk = new GiactCheckInformation
        //    {
        //        RoutingNumber = "053101121",
        //        AccountNumber = "0005106575357",
        //        CheckNumber = "022842",
        //        CheckAmount = Convert.ToDecimal(73.5),
        //        AccountType = GiactAccountType.Checking
        //    };

        //    GiactInquiry req = new GiactInquiry()
        //    {
        //        GVerifyEnabled = true,
        //        FundsConfirmationEnabled = true,
        //        Check = chk,
        //    };

        //    _appCurrentSessionContext.IdAgent = _appCurrentSessionContext.IdAgent == 0 ? 1242 : _appCurrentSessionContext.IdAgent;

        //    return Ok(_rpcService.GiactValidation(req));
        //}

        //[AllowAnonymous]
        [HttpGet("ChecksProcessed")]
        public List<CheckTiny> GetChecksProcessed(DateTime startDate, DateTime endDate, string custName = "", string checkNum = "")
        {
            return _rpcService.GetChecksProcessedReport(startDate, endDate, custName, checkNum);
        }

        /// <summary>
        /// Obtiene el listado de cheques procesados con paginación y de ser necesario consulta el summary 
        /// Se consume por H2.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="custName"></param>
        /// <param name="checkNum"></param>
        /// <returns></returns>
        [HttpGet("ChecksProcessedSummary")]
        public IActionResult ChecksProcessedSummary(DateTime? startDate, DateTime? endDate, bool? paged, bool? summary, int? limit, int? offset, string custName = "", string checkNum = "")
        {
            return Ok(_rpcService.GetChecksProcessedReport(startDate, endDate, paged, summary, limit, offset, custName, checkNum));
        }

        //[AllowAnonymous]
        [HttpGet("ChecksRejected")]
        public List<CheckTiny> GetChecksRejected(DateTime startDate, DateTime endDate, string custName = "", string checkNum = "", string printed = "")
        {
            return _rpcService.GetChecksRejectedReport(startDate, endDate, custName, checkNum, printed);
        }

        /// <summary>
        /// JISC TOOD probar el consumo desde cronos
        /// Ej: request: 
        ///     "Rpc/ProcessIRD?idCheck={check.IdCheck.ToString()}&docType=REPPARAMS"
        ///     "Rpc/ProcessIRD?idCheck={idCheck.ToString()}&docType={docType}"
        /// </summary>
        /// <param name="idCheck"></param>
        /// <param name="docType"></param>
        /// <returns></returns>
        [AllowAnonymous] // Este endpoint ignora las políticas globales de autorización
        [HttpGet("ProcessIRD")]
        public IRDResponse ProcessIRD(int idCheck, string docType = "")
        {
            return _rpcService.GetCheckIRD(idCheck, docType);
        }

        //[AllowAnonymous]
        [HttpGet("PcParam")]
        public string GetPcParam(string ident, string col)
        {
            return _rpcService.GetPcParam(ident, col);
        }

        //[AllowAnonymous]
        [HttpPost("PcParam")]
        public int SetPcParam(dynamic data)
        {
            string i = data.ident;
            string c = data.col;
            string v = data.value;

            return _rpcService.SetPcParam(i, c, v);
        }

        [AllowAnonymous]
        [HttpGet("CheckEditedElements")]
        public List<CheckElementEdited> GetCheckEditedElements(int idCheck)
        {
            return _rpcService.GetCheckEditedElements(idCheck);
        }

        /*05-Oct-2021*/
        /*UCF*/
        /*TSI_MAXI_013*/
        /*Se declara la ruta GetCheckByMircData en de tipo Get para ejecutar el metodo*/
        [HttpGet("GetCheckByMircData")]
        public CheckTiny GetCheckByMircData(string ChNum, string RoutNum, string AccNum)
        {
            return _rpcService.GetCheckByMircData(ChNum, RoutNum, AccNum);
        }

        [HttpPost("SetIdcheckImagePending")]
        public int SetIdcheckImagePending()
        {
            return _rpcService.SetIdcheckImagePending();
        }

        [HttpPost("DeleteCheckByIdCheckPending")]
        public int DeleteCheckByIdCheckPending(int id)
        {
            return _rpcService.DeleteCheckByIdCheckPending(id);
        }

        [HttpPost("CheckImageProcessed")]
        public int CheckImageProcessed(int id, List<UploadCheck> uploadChecks)
        {
            return _rpcService.CheckImageProcessed(id, uploadChecks);
        }
    }
}
