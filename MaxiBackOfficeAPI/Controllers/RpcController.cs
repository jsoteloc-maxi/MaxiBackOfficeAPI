using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Domain.Model;
using MaxiBackOfficeAPI.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiBackOfficeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RpcController : ControllerBase
    {
        private IRpcService _rpcService;

        public RpcController(IRpcService rpcService)
        {
            _rpcService = rpcService;
        }

        /// <summary>
        /// Obtiene la lista de tipos de cheque
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("CheckTypesList")]
        public IActionResult GetCheckTypesList()
        {
            return Ok(_rpcService.GetCheckTypesList());
        }

        /// <summary>
        /// Analiza la imagen de un cheque para intenta obtener toda la informacion posible
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateCheckImage")]
        public MaxiItemInfo ValidateCheckImage(ObjetoRequestItem req)//Este metodo es el que llama a orbo
        {
            return _rpcService.GetItemInfo(req);
        }

        //[HttpPost]
        //[Route("ValidateCheckImage")]
        //public MaxiItemInfo ValidateCheckImage(string imageName, byte[] imageBytes)
        //{
        //    var req = new ObjetoRequestItem
        //    {
        //        ImageName = imageName,
        //        ImageBytes = imageBytes,
        //    };
        //    return _svc.GetItemInfo(req);
        //}


        /// <summary>
        /// Consulta informacion en el servicio GIACT
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ValidateCheckGiact")]
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
        [HttpGet]
        [Route("AccountCautionNotes")]
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
        [HttpGet]
        [Route("IssuerAction")]
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
        [HttpGet]
        [Route("AccountCheckSummary")]
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
        [HttpGet]
        [Route("GetMakerByAcc")]
        public CC_GetMakerByAccRes GetMakerByAcc(string rout, string acc)
        {
            return _rpcService.GetMakerByAcc(rout, acc);
        }//Este metodo se llama cada que se edita el cheque


        /// <summary>
        /// Lista de cheques mas recientes del issuer
        /// </summary>
        [HttpGet]
        [Route("RecentChecksBy")]
        public List<CheckTiny> GetRecentChecksByCustomer(int idCustomer)
        {
            return _rpcService.GetRecentChecksByCustomer(idCustomer);
        }

        [HttpGet]
        [Route("RecentChecksBy")]
        public List<CheckTiny> GetRecentChecksByIssuer(int idIssuer)
        {
            return _rpcService.GetRecentChecksByIssuer(idIssuer);
        }

        [HttpGet]
        [Route("RecentChecksBy")]
        public IActionResult GetRecentChecksByIssuerV2(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrden = null)
        {
            var result = _rpcService.GetRecentChecksByIssuer(idIssuer, startDate, endDate, paged, offset, limit, sortColumn, sortOrden);
            if (!paged.Value)

                return Ok(result.Data);
            else
                return Ok(result);

        }

        [HttpGet]
        [Route("RecentChecksBy")]
        public IActionResult GetRecentChecksByCustomerV2(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null)
        {
            var result = _rpcService.GetRecentChecksByCustomer(idCustomer, startDate, endDate, paged, offset, limit, sortColumn, sortOrder);
            if (!paged.Value)
                return Ok(result.Data);
            else
                return Ok(result);

        }

        [HttpGet]
        [Route("GetAgFeeComm")]
        public CCAgFeeCommRes GetAgFeeComm(int idAgent, decimal amount, int idState)
        {
            return _rpcService.GetAgFeeCommInfo(idAgent, amount, idState);
        }

        private IActionResult TestGiact()
        {
            var chk = new GiactCheckInformation
            {
                RoutingNumber = "053101121",
                AccountNumber = "0005106575357",
                CheckNumber = "022842",
                CheckAmount = Convert.ToDecimal(73.5),
                AccountType = GiactAccountType.Checking
            };

            GiactInquiry req = new GiactInquiry()
            {
                GVerifyEnabled = true,
                FundsConfirmationEnabled = true,
                Check = chk,
            };

            // ahora 
            var appCurrentSessionContext = HttpContext.Items["appCurrentSessionContext"] as AppCurrentSessionContext;
            if (appCurrentSessionContext != null && appCurrentSessionContext.IdAgent == 0)
            {
                appCurrentSessionContext.IdAgent = 1242;
                HttpContext.Items["appCurrentSessionContext"] = appCurrentSessionContext;
            }

            return Ok(_rpcService.GiactValidation(req));
        }


        [HttpGet]
        //[OverrideAuthorization]
        [Route("ChecksProcessed")]
        public List<CheckTiny> GetChecksProcessed(DateTime startDate, DateTime endDate, string custName = "", string checkNum = "")
        {
            return _rpcService.GetChecksProcessedReport(startDate, endDate, custName, checkNum);
        }

        [HttpGet]
        //[OverrideAuthorization]
        [Route("ChecksRejected")]
        public List<CheckTiny> GetChecksRejected(DateTime startDate, DateTime endDate, string custName = "", string checkNum = "", string printed = "")
        {
            return _rpcService.GetChecksRejectedReport(startDate, endDate, custName, checkNum, printed);
        }


        [AllowAnonymous] // Este endpoint ignora las políticas globales de autorización
        [HttpGet, Route("ProcessIRD")]
        public IRDResponse ProcessIRD(int idCheck, string docType = "")
        {
            return _rpcService.GetCheckIRD(idCheck, docType);
        }



        //[OverrideAuthorization]
        [HttpGet, Route("PcParam")]
        public string GetPcParam(string ident, string col)
        {
            return _rpcService.GetPcParam(ident, col);
        }

        //[OverrideAuthorization]
        [HttpPost, Route("PcParam")]
        public int SetPcParam(dynamic data)
        {
            string i = data.ident;
            string c = data.col;
            string v = data.value;

            return _rpcService.SetPcParam(i, c, v);
        }


        [AllowAnonymous] // antes [OverrideAuthorization]
        [HttpGet, Route("CheckEditedElements")]
        public List<CheckElementEdited> GetCheckEditedElements(int idCheck)
        {
            return _rpcService.GetCheckEditedElements(idCheck);
        }

        /*05-Oct-2021*/
        /*UCF*/
        /*TSI_MAXI_013*/
        /*Se declara la ruta GetCheckByMircData en de tipo Get para ejecutar el metodo*/
        [HttpGet, Route("GetCheckByMircData")]
        public CheckTiny GetCheckByMircData(string ChNum, string RoutNum, string AccNum)
        {
            return _rpcService.GetCheckByMircData(ChNum, RoutNum, AccNum);
        }

        [HttpPost, Route("SetIdcheckImagePending")]
        public int SetIdcheckImagePending()
        {
            return _rpcService.SetIdcheckImagePending();
        }

        [HttpPost, Route("DeleteCheckByIdCheckPending")]
        public int DeleteCheckByIdCheckPending(int id)
        {
            return _rpcService.DeleteCheckByIdCheckPending(id);
        }

        [HttpPost, Route("CheckImageProcessed")]
        public int CheckImageProcessed(int id, List<UploadCheck> uploadChecks)
        {
            return _rpcService.CheckImageProcessed(id, uploadChecks);
        }
    }
}
