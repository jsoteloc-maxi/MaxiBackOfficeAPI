using Maxi.BackOffice.Agent.Domain.Model;
using OrbographWebService;
using Maxi.BackOffice.Agent.Infrastructure.Mappings;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.CrossCutting.Common.Extensions;
using Maxi.BackOffice.Agent.Infrastructure.Common;
using Maxi.BackOffice.CrossCutting.Enums;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.CrossCutting.Common.Common;

namespace Maxi.BackOffice.Agent.Application.Services
{
    public class RpcService : IRpcService
    {
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;
        private readonly ICheckRepository _checkRepository;
        private readonly IOrbographRepository _orbographRepository;
        private readonly IGlobalAttributesRepository _globalAttributesRepository;
        private readonly IGiactServiceLogRepository _giactServiceLogRepository;
        //private readonly IIrdRepository _irdRepository;
        private readonly IPcParamsRepository _pcParamsRepository;
        private readonly ICheckImagePendingRepository _checkImagePendingRepository;

        public RpcService(
            IAppCurrentSessionContext appCurrentSessionContext,
            ICheckRepository checkRepository,
            IOrbographRepository orbographRepository,
            IGlobalAttributesRepository globalAttributesRepository,
            IGiactServiceLogRepository giactServiceLogRepository,
            //IIrdRepository irdRepository,
            IPcParamsRepository pcParamsRepository,
            ICheckImagePendingRepository checkImagePendingRepository)
        {
            _appCurrentSessionContext = appCurrentSessionContext;
            _checkRepository = checkRepository;
            _orbographRepository = orbographRepository;
            _globalAttributesRepository = globalAttributesRepository;
            _giactServiceLogRepository = giactServiceLogRepository;
            //_irdRepository = irdRepository;
            _pcParamsRepository = pcParamsRepository;
            _checkImagePendingRepository = checkImagePendingRepository;
        }

        MaxiItemInfo IRpcService.GetItemInfo(ObjetoRequestItem aReq)
        {
            string lerror = "";
            var vlItemInfo = new MaxiItemInfo();
            vlItemInfo.CheckCanBeProcessed = "OK";

            #region Llamada Orbograph
            try
            {
                var orboRes = _orbographRepository.ValidateCheck(aReq.ImageBytes, aReq.ImageName);
                if (orboRes.Reco.ErrorCode == 1000)
                {
                    OrboAssignResult(orboRes, ref vlItemInfo, aReq.Metodo);
                }
                else
                    lerror = "ErrorCode " + orboRes.Reco.ErrorCode.ToString();
            }
            catch (Exception ex)
            {
                lerror = ExtractExceptionMessage(ex);
                lerror = lerror.Substring(0, Math.Min(200, lerror.Length));
            }
            //llamar funcionalidad para guardar imagenes y generar guid de identificacion 

            if (aReq.Metodo == "BATCH")
            {
                vlItemInfo.IdImage = Guid.NewGuid();

                ImageManager imageManager = new ImageManager(_globalAttributesRepository.GetValue("BatchImgPath"));
                imageManager.SaveImage(aReq.IdcheckImagePending, aReq.ImageBytes, aReq.ImageBytesRear, vlItemInfo.IdImage);
            }

            if (lerror != "")
                vlItemInfo.AccCautionNotes.Add("INFO", "OCR: " + lerror, "Orbo");

            //Si hay datos de routing => busca directo en base el issuer
            if (!vlItemInfo.RoutingNum.IsBlank() && !vlItemInfo.AccountNum.IsBlank())
            {
                var issuer = CC_GetMakerByAccResMapper.Map(_checkRepository.GetMakerByAcc(vlItemInfo.RoutingNum, vlItemInfo.AccountNum));
                if (issuer != null && issuer.Maker_ID > 0)
                {
                    vlItemInfo.Maker.IdMaker = issuer.Maker_ID;
                    vlItemInfo.Maker.MakerName = issuer.MAK_Name;
                }
            }
            #endregion

            try
            {
                //Busca totales de cheques procesados por esta cuenta
                vlItemInfo.AccSummary = GetAccountCheckSummary(vlItemInfo.RoutingNum, vlItemInfo.AccountNum);
            }
            catch (Exception ex)
            {
                vlItemInfo.AccCautionNotes.Add("WARN", "GetCheckSummary: " + ExtractExceptionMessage(ex), "");
            }

            //Todo: ver mejorar rutina para obtener notas,  ahora sera
            //GetAccountValidationData que incluye las notas y ultimos datos de giact

            try
            {
                //Se busca hacer una sola llamada en el metodo de batch ya que del lado del front se consume
                if (aReq.Metodo != "BATCH")
                {
                    //Buscar notas de la cuenta
                    var lnotes = GetAccountCautionNotes(vlItemInfo.RoutingNum, vlItemInfo.AccountNum, vlItemInfo.CheckNum);
                    vlItemInfo.AccCautionNotes.Notes.AddRange(lnotes.Notes);
                }
            }
            catch (Exception ex)
            {
                vlItemInfo.AccCautionNotes.Add("WARN", "GetCautionNotes: " + ExtractExceptionMessage(ex), "");
            }

            //De momento los CautionNotes son los que indican si el cheque puede ser procesado, ver mas
            //adelante si otras regla se van a incluir o si se crea api para esa validacion
            vlItemInfo.CheckCanBeProcessed = vlItemInfo.AccCautionNotes.Status; //esta prop ya no se usa, se va a quitar

            //Ordena las notas antes de regresarlas
            vlItemInfo.AccCautionNotes.SortNotes();

            //using (var context = CreateUnitOfWork())
            //{
            //    var r = _checkRepository.GetAllWordFilter();
            //    // bool palabra = r.Exists(it => vlItemInfo.Customer.CustName.Contains(it.Word));
            //    foreach (var info in r)
            //    {
            //        if (vlItemInfo.Customer.CustName.Contains(info.Word))
            //        {
            //            vlItemInfo.IsCompanyCheck = true;
            //            break;
            //        }
            //    }
            //    //if (palabra == true)
            //    //{
            //    //    vlItemInfo.IsCompanyCheck = true;
            //    //}
            //}

            try
            {
                if (vlItemInfo.Customer.CustName != "" && vlItemInfo.Customer.CustName != " " && vlItemInfo.Customer.CustName != null)
                {
                    var r = _checkRepository.GetAllWordFilter();
                    string[] words = vlItemInfo.Customer.CustName.Split(' ');

                    foreach (var info in r)
                    {
                        vlItemInfo.IsCompanyCheck = words.Any(it => it.Equals(info.Word, StringComparison.OrdinalIgnoreCase));
                        if (vlItemInfo.IsCompanyCheck == true)
                            break;
                    }
                }
            }
            catch
            {
                vlItemInfo.IsCompanyCheck = false;
            }



            return vlItemInfo;
        }


        CCAgFeeCommRes IRpcService.GetAgFeeCommInfo(int idAgent, decimal amount, int idState)
        {
            var r = _checkRepository.GetCCAgFeeComm(idAgent, amount, idState);
            CCAgFeeCommRes fee = CCAgFeeCommResMapper.Map(r);
            return fee;
        }


        CC_GetMakerByAccRes IRpcService.GetMakerByAcc(string rout, string acc)
        {
            var r = _checkRepository.GetMakerByAcc(rout, acc);
            return CustomMapperBase<CC_GetMakerByAccRes, CC_GetMakerByAccEntity>.Map(r);
        }


        GiactResult IRpcService.GiactValidation(GiactInquiry request)
        {
            var res = _giactServiceLogRepository.ValidateCheck(request);
            return res;
        }


        //-------------------------------------------
        public List<CheckType> GetCheckTypesList()
        {
            var r = _checkRepository.GetAllCheckTypes();
            return CustomMapperBase<CheckType, CC_CheckTypeEntity>.Map(r);
        }

        private void OrboAssignResult(ValidationResponse orboRes, ref MaxiItemInfo prmItem, String method)
        {
            try
            {
                //Requerimiento Maxi-014 validacion de Firma
                int CC_SignatureScore = Convert.ToInt32(_globalAttributesRepository.GetValue("CC_SignatureScore"));
                var signatureOrboResult = orboRes.TestResult.TestResults.Find(x => x.Type == ValidationType.Signature);
                if (signatureOrboResult.Result == ValidationResultType.Passed && signatureOrboResult.Score >= CC_SignatureScore &&
                    signatureOrboResult.ReasonCode == ErrorCodes.ErrorOk)
                    prmItem.IsSigned = true;
                else
                    prmItem.IsSigned = false;

                prmItem.Ocr.ErrorCode = orboRes.Reco.ErrorCode;
                prmItem.Ocr.DocName = orboRes.Reco.DocName;
                prmItem.Ocr.DocType = orboRes.Reco.DocType;

                //Datos extraidos directo del Micr
                if (orboRes.Reco.Micr != null)
                {
                    prmItem.Ocr.Score = orboRes.Reco.Micr.Score;

                    if (orboRes.Reco.Micr.Amount != null)
                    {
                        prmItem.Ocr.MicrAmount = orboRes.Reco.Micr.Amount.Value;
                        prmItem.Ocr.MicrAmountScore = orboRes.Reco.Micr.Amount.Score;
                    }

                    if (orboRes.Reco.Micr.Routing != null)
                    {
                        prmItem.Ocr.Routing = orboRes.Reco.Micr.Routing.Value;
                        prmItem.Ocr.RoutingScore = orboRes.Reco.Micr.Routing.Score;
                    }

                    if (orboRes.Reco.Micr.OnusAccount != null)
                    {
                        prmItem.Ocr.OnUsAccount = orboRes.Reco.Micr.OnusAccount.Value;
                        prmItem.Ocr.OnUsAccountScore = orboRes.Reco.Micr.OnusAccount.Score;
                    }

                    if (orboRes.Reco.Micr.OnusCheck != null)
                    {
                        prmItem.Ocr.OnUsCheck = orboRes.Reco.Micr.OnusCheck.Value;
                        prmItem.Ocr.OnUsCheckScore = orboRes.Reco.Micr.OnusCheck.Score;
                    }
                    else
                    {
                        if (orboRes.Reco.Micr.Aux != null)
                        {
                            prmItem.Ocr.OnUsCheck = orboRes.Reco.Micr.Aux.Value;
                            prmItem.Ocr.OnUsCheckScore = orboRes.Reco.Micr.Aux.Score;
                        }
                    }
                }

                //Datos de Payer-Maker
                if (orboRes.Reco.Payer != null)
                {
                    if (orboRes.Reco.Payer.Name1 != null)
                    {
                        prmItem.Ocr.PayerScore = orboRes.Reco.Payer.Name1.Score;
                        prmItem.Ocr.PayerName = orboRes.Reco.Payer.Name1.Value;

                        if (orboRes.Reco.Payer.Name1.Score >= 80)
                            prmItem.Maker.MakerName = prmItem.Ocr.PayerName;
                    }
                }

                if (prmItem.Ocr.RoutingScore > 80)
                    prmItem.RoutingNum = prmItem.Ocr.Routing;

                if (prmItem.Ocr.OnUsAccountScore > 80)
                    prmItem.AccountNum = prmItem.Ocr.OnUsAccount;

                if (prmItem.Ocr.OnUsCheckScore > 80)
                    prmItem.CheckNum = prmItem.Ocr.OnUsCheck;


                int car_score = 0;
                int.TryParse(_globalAttributesRepository.GetValue("AmountScore_ForDisplay"), out int car_score_max);
                if (car_score_max <= 0)
                    car_score_max = 30;

                prmItem.Customer.CustName = "";

                decimal LAR = 0;
                foreach (var ResponseField in orboRes.Reco.ResponseFields)
                {
                    //MICR
                    if (ResponseField.Type == "micr")
                    {
                        prmItem.Ocr.MicrScore = ResponseField.Score;
                        prmItem.Ocr.Micr = ResponseField.Result ?? "";
                        if (ResponseField.Score >= 80)
                            prmItem.Micr = prmItem.Ocr.Micr;
                    }

                    //CheckAmount
                    if (ResponseField.Type == "car")
                    {
                        car_score = ResponseField.Score;

                        /*
                        // Dependiendo del tipo de cheque es el score_max
                        switch (orboRes.Reco.DocType)
                        {
                            case "2":  //"Personal"
                                car_score_max = 30;
                                break;
                            case "3":  //"Business"
                                car_score_max = 30;
                                break;
                            case "MoneyOrder":
                                car_score_max = 50;
                                break;
                            case "TravelerCheck":
                                car_score_max = 50;
                                break;
                            default:
                                car_score_max = 60;
                                break;
                        }
                        */

                        //Ahora se usa un parametro para configurar el score_max para desplegar el monto
                        if (car_score > car_score_max)
                            prmItem.CheckAmount = Convert.ToDecimal(ResponseField.Result);
                    }

                    //CheckDate
                    if (ResponseField.Type.ToLower() == "date")
                    {
                        prmItem.CheckDateScore = ResponseField.Score;

                        //Validar cual es el origen Individual aplica lo existente
                        if (method.EqualText("BATCH"))
                        {
                            int score_date = 0;
                            int.TryParse(_globalAttributesRepository.GetValue("ScoreDate"), out score_date);
                            if (ResponseField.Score > score_date)
                                if (DateTime.TryParse(ResponseField.Result, out var checkDate))
                                    prmItem.CheckDate = checkDate;
                        }
                        else
                        {

                            if (ResponseField.Score > 30)//Cambiar este valor por un GlobalAttribute siempre que sea de batch
                                if (DateTime.TryParse(ResponseField.Result, out var checkDate))
                                    prmItem.CheckDate = checkDate;
                        }
                        //En caso de no superar el score enviar el valor vacio
                    }

                    //CustomerName
                    if (ResponseField.Type == "payee" && ResponseField.Score > 30)
                    {
                        prmItem.Customer.CustName = ResponseField.Result ?? "";
                    }

                    if (ResponseField.Type == "lar")
                    {
                        if (!string.IsNullOrEmpty(ResponseField.Result))
                            decimal.TryParse(ResponseField.Result, out LAR);
                    }
                }

                //tarea 13 validar que LAR y checkAmount sean iguales
                //if (LAR != prmItem.CheckAmount)
                //    prmItem.CheckAmount = 0;

                prmItem.CheckAmountScore = car_score;

                //Valida si amount en micr tiene mejor score
                if (prmItem.Ocr.MicrAmountScore > 0)
                    if (prmItem.Ocr.MicrAmountScore >= car_score)
                    {
                        var s = prmItem.Ocr.MicrAmount.Insert(prmItem.Ocr.MicrAmount.Length - 2, ".");
                        prmItem.CheckAmount = Convert.ToDecimal(s);
                        prmItem.CheckAmountScore = prmItem.Ocr.MicrAmountScore;
                    }

                //var Amount = vlValRes.TestResult.TestResults.SingleOrDefault(tr => tr.Type == ValidationType.Amount);
                //var Signature = prmResponse.TestResult.TestResults.SingleOrDefault(tr => tr.Type == ValidationType.Signature);
                //if (Signature.Result == ValidationResultType.Failed)
                //{
                //    prmItem.WarningsAlerts.Add("Signature Validation Failed");
                //}

            }
            catch (Exception ex)
            {
                //string vlFName = string.Format(@"D:\CC\Images\{0:yyyyMMdd_HHmmss}_ERR.txt", DateTime.Now);
                //System.IO.File.WriteAllText(vlFName, ex.Message);
                throw ex;
            }
        }


        public AccountCheckSummary GetAccountCheckSummary(string rout, string acc)
        {
            var r = _checkRepository.GetAccountCheckSummary(rout, acc);
            return CustomMapperBase<AccountCheckSummary, SpCC_GetAccountCheckSummaryEntity>.Map(r);
        }

        public List<IssuerActionCheck> GetIssuerAction(int idIssuer)
        {
            var r = _checkRepository.GetIssuerAction(idIssuer);
            return CustomMapperBase<IssuerActionCheck, CC_IssuerActionCheck>.Map(r);
        }


        public AccountCautionNotes GetAccountCautionNotes(string rout, string acc, string checkNum)// Aqui se debe de validar el Routing number con la tabla de routing number bloqueados
        {
            var r = new AccountCautionNotes();
            var n = _checkRepository.GetCautionNotes(rout, acc, checkNum);
            r.Notes.AddRange(CustomMapperBase<AccountCautionNote, SpCC_GetCautionNotesEntity>.Map(n));
            r.SortNotes();
            return r;
        }


        public AccountCautionNotes GetAccountVerificationInfo(string rout, string acc)
        {
            var r = new AccountCautionNotes();
            //Manda buscar notas
            var n = _checkRepository.GetCautionNotes(rout, acc, "");
            r.Notes.AddRange(CustomMapperBase<AccountCautionNote, SpCC_GetCautionNotesEntity>.Map(n));
            r.SortNotes();
            return r;
        }



        private string ExtractExceptionMessage(Exception ex)
        {
            string s = "";
            if (ex.InnerException != null) s = ex.InnerException.Message + ": ";
            s += ex.Message;
            return s;
        }


        public List<CheckTiny> GetRecentChecksByCustomer(int idCustomer)
        {
            var r = _checkRepository.GetRecentChecksByCustomer(idCustomer);
            return CustomMapper.Map<List<CheckTiny>>(r);
        }
        public PaginationResponse<List<CheckTiny>> GetRecentChecksByCustomer(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null)
        {
            PaginationResponse<List<CheckTiny>> response = new PaginationResponse<List<CheckTiny>>();
            return _checkRepository.GetRecentChecksByCustomer(idIssuer, startDate, endDate, paged, offset, limit, sortColumn, sortOrder);
        }

        public List<CheckTiny> GetRecentChecksByIssuer(int idIssuer)
        {
            var r = _checkRepository.GetRecentChecksByIssuer(idIssuer);
            return CustomMapper.Map<List<CheckTiny>>(r);
        }

        public PaginationResponse<List<CheckTiny>> GetRecentChecksByIssuer(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrden = null)
        {
            PaginationResponse<List<CheckTiny>> response = new PaginationResponse<List<CheckTiny>>();
            return _checkRepository.GetRecentChecksByIssuer(idCustomer, startDate, endDate, paged, offset, limit, sortColumn, sortOrden);
        }

        public List<CheckTiny> GetChecksProcessedReport(DateTime date1, DateTime date2, string custName, string checkNum)
        {
            var r = _checkRepository.GetChecksProcessedReport(date1, date2, custName, checkNum);
            return CustomMapper.Map<List<CheckTiny>>(r);
        }


        public List<CheckTiny> GetChecksRejectedReport(DateTime date1, DateTime date2, string custName, string checkNum, string printed)
        {
            var r = _checkRepository.GetChecksRejectedReport(date1, date2, custName, checkNum, printed);
            return CustomMapper.Map<List<CheckTiny>>(r);
        }

        public IRDResponse GetCheckIRD(int idCheck, string docType)
        {
            IRDResponse result = null;
            // JISC TODO: REVISAR EL ERROR
            //var logoFile = AppDomain.CurrentDomain.BaseDirectory + @"\Assets\Logo256.bmp";

            //if (docType.ToUpper() == "PDF")
            //{
            //    var m = _irdRepository.ObtenerImpresionIRD(idCheck, logoFile);
            //    result = CustomMapper.Map<IRDResponse>(m);
            //    result.DocType = "PDF";
            //}

            //if (docType.ToUpper() == "REPPARAMS")
            //{
            //    var m = _irdRepository.ObtenerImpresionIRDParams(idCheck, logoFile);
            //    result = CustomMapper.Map<IRDResponse>(m);
            //    result.DocType = "REPPARAMS";
            //}

            //if (docType.ToUpper() == "VIEWREPROCESS")
            //{
            //    var m = _irdRepository.ObtenerImagenesIRD(idCheck, false, false);
            //    result = CustomMapper.Map<IRDResponse>(m);
            //    result.DocType = "TIF";
            //}

            //if (result == null)
            //{
            //    var m = _irdRepository.ObtenerImagenesIRD(idCheck);
            //    result = CustomMapper.Map<IRDResponse>(m);
            //    result.DocType = "TIF";
            //}

            // JISC TODO revisar context.SaveChanges();
            //context.SaveChanges();
            return result;
        }


        public string GetPcParam(string ident, string col)
        {
            return _pcParamsRepository.GetParam(ident, col);
        }


        public int SetPcParam(string ident, string col, string value)
        {
            var r = _pcParamsRepository.SetParam(ident, col, value);
            // JISC TODO revisar SaveChanges()
            //uw.SaveChanges();
            return r;
        }

        public List<CheckElementEdited> GetCheckEditedElements(int idCheck)
        {
            var r = _checkRepository.GetCheckEditedElements(idCheck);
            return CustomMapper.Map<List<CheckElementEdited>>(r);
        }

        /*07-Sep-2021*/
        /*UCF*/
        /*TSI_MAXI_013*/
        /*Llama al repositorio y retorna true/false dependiendo del resultado de la ejecucion de GetCheckByMircData*/
        public CheckTiny GetCheckByMircData(string ChNum, string RoutNum, string AccNum)
        {
            List<CheckTiny> r = CustomMapper.Map<List<CheckTiny>>(_checkRepository.GetCheckByMircData(ChNum, RoutNum, AccNum)); ;
            return r.FirstOrDefault();
        }

        public int SetIdcheckImagePending()
        {
            var result = _checkImagePendingRepository.Insert(new CheckImagePendingEntity()).IdcheckImagePending;
            return result;
        }

        public int CheckImageProcessed(int id, List<UploadCheck> uploadChecks)
        {
            UploadChecks(uploadChecks);
            var result = _checkImagePendingRepository.ChecImageProcessed(id);
            return result;
        }

        public void UploadChecks(List<UploadCheck> uploadChecks)
        {
            List<UploadFileDto> fileDto = new List<UploadFileDto>();
            ImageManager imageManager = new ImageManager(_globalAttributesRepository.GetValue("BatchImgPath"));
            string directory = _globalAttributesRepository.GetValue("ChecksPath");
            List<string> guidName = new List<string>();

            foreach (var check in uploadChecks)
            {
                guidName = new List<string>();
                guidName.AddRange(
                imageManager.MoveImage(check.IdCheckPendingImage, check.IdImage, directory + "\\" + check.IdIssuer.ToString() + "\\Checks" + "\\" + check.IdCheck.ToString() + "\\"
                    , directory + "\\" + check.IdIssuer.ToString() + "\\Checks" + "\\" + check.IdCheck.ToString()));

                foreach (var item in guidName)
                {
                    //idtypedocument 69 es para cheques  en el nombre del archivo se tiene que concatenar un 1 o 2 para diferenciar entre frontal y trasera
                    fileDto.Add(new UploadFileDto()
                    {
                        IdReference = check.IdCheck,
                        Extension = ".tif",
                        FileGuid = item.Split('_')[0],
                        FileName = item.Split('_')[0],
                        IdStatus = 1,
                        IdUser = _appCurrentSessionContext.IdUser,
                        IdDocumentType = (int)eDocumentType.Check,
                        ExpirationDate = null,
                        idImgType = Convert.ToInt32(item.Split('_')[1]),
                        IdCountry = null,
                        IdState = null
                    });
                }
            }

            if (fileDto.Count > 0)
            {
                _checkImagePendingRepository.UploadImage(fileDto);
            }
        }

        public int DeleteCheckByIdCheckPending(int Id)
        {
            try
            {
                var current = _checkImagePendingRepository.GetById(Id);
                if (current.ProcessingDate <= DateTime.MinValue)
                {
                    ImageManager imageManager = new ImageManager(_globalAttributesRepository.GetValue("BatchImgPath"));
                    imageManager.DeleteById(Id);
                    // JISC TODO: revisar SaveChanges()
                    //contex.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 1;
        }
    }
}
