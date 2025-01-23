using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Domain.Model;
using System.Web.Http;

namespace MaxiBackOfficeAPI.ControllersApi
{
    [RoutePrefix("api/Rpc")]
    public class RpcController: ApiControllerBase
    {
        private IRpcService _svc;

        //Constructor
        public RpcController(IRpcService svc)
        {
            _svc = svc;
            InitService(_svc);
        }



        //Ejemplo de como recibir
        // POST: api/Rpc
        //public HttpResponseMessage Post([FromBody]ObjetoRequestItem oReq)
        //public HttpResponseMessage Post([FromBody]JObject jsonReq)
        //[Route("")]
        //public HttpResponseMessage Post([FromBody]JObject jsonReq)
        //{
        //    string lmethod = (string)jsonReq["Method"] ?? "";
        //    //string lparams = (string)jsonReq["Params"] ?? "";

        //    switch (lmethod.ToLower())
        //    {
        //        //case "validatecheck":
        //        //    var oReq = JsonConvert.DeserializeObject<ObjetoRequestItem>(jsonReq.ToString());
        //        //    var vlItemInfoRes = _svc.GetItemInfo(oReq);

        //            //solo para probar si esta llegando la imagen correctamente
        //            //System.IO.File.WriteAllBytes(@"D:\CC\Images\yourfile.tif", oReq.ImageBytes);
        //            //return Request.CreateResponse(HttpStatusCode.OK, vlItemInfoRes);


        //        //case "getagfeecomm":
        //        //    var vlCCAgFeeCommReq = JsonConvert.DeserializeObject<CCAgFeeCommReq>(jsonReq.ToString());
        //        //    var vlAgFeeCommRes = _svc.GetAgFeeCommInfo(vlCCAgFeeCommReq);
        //        //    return Request.CreateResponse(HttpStatusCode.OK, vlAgFeeCommRes);

        //        default:
        //            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, " is the error");
        //    }

        //}




        /// <summary>
        /// Obtiene la lista de tipos de cheque
        /// </summary>
        /// <returns></returns>
        [Route("CheckTypesList")]
        public List<CheckType> GetCheckTypesList()
        {
            return _svc.GetCheckTypesList();
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
            return _svc.GetItemInfo(req);
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
            return _svc.GiactValidation(req);
        }


        /// <summary>
        /// Hace revision de reglas y regresa notas al respecto
        /// </summary>
        /// <param name="rout"></param>
        /// <param name="acc"></param>
        /// <param name="checkNum"></param>
        /// <returns></returns>
        [Route("AccountCautionNotes")]
        public AccountCautionNotes GetCautionNotes(string rout, string acc, string checkNum = "")
        {
            //Api para llamada manual de GetAccountCautionNotes
            //Todo: se va a necesitar tambien recibir el numero de cheque para validar si ya hay un ird procesado y rechazado

            return _svc.GetAccountCautionNotes(rout, acc, checkNum);
        }

        /// <summary>
        /// Hace revision de reglas y regresa notas al respecto
        /// </summary>
        /// <param name="idIssuer"></param>
        /// <returns></returns>
        [Route("IssuerAction")]
        public List<IssuerActionCheck> GetIssuerAction(int idIssuer)
        {
            //Api para llamada manual de GetAccountCautionNotes
            //Todo: se va a necesitar tambien recibir el numero de cheque para validar si ya hay un ird procesado y rechazado

            return _svc.GetIssuerAction(idIssuer);
        }



        /// <summary>
        /// Busca resumen historico de cheques procesados
        /// </summary>
        /// <param name="rout"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        [Route("AccountCheckSummary")]
        public AccountCheckSummary GetAccountCheckSummary(string rout, string acc)
        {
            return _svc.GetAccountCheckSummary(rout, acc);
        }

        /// <summary>
        /// Busca Maker usando routing y account
        /// </summary>
        /// <param name="rout"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        [Route("GetMakerByAcc")]
        public CC_GetMakerByAccRes GetMakerByAcc(string rout, string acc)
        {
            return _svc.GetMakerByAcc(rout, acc);
        }//Este metodo se llama cada que se edita el cheque


        /// <summary>
        /// Lista de cheques mas recientes del issuer
        /// </summary>
        [Route("RecentChecksBy")]
        public List<CheckTiny> GetRecentChecksByCustomer(int idCustomer)
        {
            return _svc.GetRecentChecksByCustomer(idCustomer);
        }
        [Route("RecentChecksBy")]
        public List<CheckTiny> GetRecentChecksByIssuer(int idIssuer)
        {
            return _svc.GetRecentChecksByIssuer(idIssuer);
        }
        [Route("RecentChecksBy")]
        public IHttpActionResult GetRecentChecksByIssuerV2(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrden = null)
        {
            var result = _svc.GetRecentChecksByIssuer(idIssuer, startDate, endDate, paged, offset, limit, sortColumn, sortOrden);
            if (!paged.Value)

                return Ok(result.Data);
            else
                return Ok(result);

        }
        [Route("RecentChecksBy")]
        public IHttpActionResult GetRecentChecksByCustomerV2(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null)
        {
            var result = _svc.GetRecentChecksByCustomer(idCustomer, startDate, endDate, paged, offset, limit, sortColumn, sortOrder);
            if (!paged.Value)
                return Ok(result.Data);
            else
                return Ok(result);

        }


        [Route("GetAgFeeComm")]
        public CCAgFeeCommRes GetAgFeeComm(int idAgent, decimal amount, int idState)
        {
            return _svc.GetAgFeeCommInfo(idAgent, amount, idState);
        }


        //[HttpGet, Route("TestGiact")]
        private GiactResult TestGiact()
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

            if (SessionCtx.IdAgent == 0)
            {
                SessionCtx.IdAgent = 1242;
            }


            return _svc.GiactValidation(req);
        }


        //[OverrideAuthorization]
        [Route("ChecksProcessed")]
        public List<CheckTiny> GetChecksProcessed(DateTime startDate, DateTime endDate, string custName = "", string checkNum = "")
        {
            return _svc.GetChecksProcessedReport(startDate, endDate, custName, checkNum);
        }

        //[OverrideAuthorization]
        [Route("ChecksRejected")]
        public List<CheckTiny> GetChecksRejected(DateTime startDate, DateTime endDate, string custName = "", string checkNum = "", string printed = "")
        {
            return _svc.GetChecksRejectedReport(startDate, endDate, custName, checkNum, printed);
        }


        [OverrideAuthorization]
        [HttpGet, Route("ProcessIRD")]
        public IRDResponse ProcessIRD(int idCheck, string docType = "")
        {
            return _svc.GetCheckIRD(idCheck, docType);
        }



        //[OverrideAuthorization]
        [HttpGet, Route("PcParam")]
        public string GetPcParam(string ident, string col)
        {
            return _svc.GetPcParam(ident, col);
        }

        //[OverrideAuthorization]
        [HttpPost, Route("PcParam")]
        public int SetPcParam(dynamic data)
        {
            string i = data.ident;
            string c = data.col;
            string v = data.value;

            return _svc.SetPcParam(i, c, v);
        }


        [OverrideAuthorization]
        [HttpGet, Route("CheckEditedElements")]
        public List<CheckElementEdited> GetCheckEditedElements(int idCheck)
        {
            return _svc.GetCheckEditedElements(idCheck);
        }

        /*05-Oct-2021*/
        /*UCF*/
        /*TSI_MAXI_013*/
        /*Se declara la ruta GetCheckByMircData en de tipo Get para ejecutar el metodo*/
        [HttpGet, Route("GetCheckByMircData")]
        public CheckTiny GetCheckByMircData(string ChNum, string RoutNum, string AccNum)
        {
            return _svc.GetCheckByMircData(ChNum, RoutNum, AccNum);
        }

        [HttpPost, Route("SetIdcheckImagePending")]
        public int SetIdcheckImagePending()
        {
            return _svc.SetIdcheckImagePending();
        }

        [HttpPost, Route("DeleteCheckByIdCheckPending")]
        public int DeleteCheckByIdCheckPending(int id)
        {
            return _svc.DeleteCheckByIdCheckPending(id);
        }

        [HttpPost, Route("CheckImageProcessed")]
        public int CheckImageProcessed(int id, List<UploadCheck> uploadChecks)
        {
            return _svc.CheckImageProcessed(id, uploadChecks);
        }
    }
}
