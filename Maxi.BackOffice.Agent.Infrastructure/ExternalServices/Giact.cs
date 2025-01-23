using System.Net;
using System.Net.Http.Headers;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.CrossCutting.Common.Extensions;

namespace Maxi.BackOffice.Agent.Infrastructure.ExternalServices
{

    public class Giact
    {
        private readonly HttpClient _clientREST;
        private readonly GiactService.InquiriesWS58SoapClient _clientSOAP;
        private readonly GiactApiType _api;
        private readonly string _apiUsername;
        private readonly string _apiPassword;


        // Se arma el end point
        public Giact(string apiUsername, string apiPassword, GiactApiType api = GiactApiType.SOAP)
        {
            _api = api;
            _apiUsername = apiUsername;
            _apiPassword = apiPassword;
            string urlbase = string.Empty; //Leer del Web Config

            if (_api == GiactApiType.REST)
            {
                _clientREST = new HttpClient
                {
                    BaseAddress = new Uri(urlbase)
                };
                _clientREST.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            if (_api == GiactApiType.SOAP)
            {
                
                //https://stackoverflow.com/questions/45465731/how-to-fix-the-server-certificate-is-not-configured-properly-with-http-sys-on/48100471
                //requiere esto para forzar TLS1.2
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                _clientSOAP = new GiactService.InquiriesWS58SoapClient();
            }
        }


        public GiactResult GetAccInfo(GiactInquiry request)
        {
            if (_api == GiactApiType.SOAP)
            {
                return AuxGetAccInfoSOAP(request);
            }
            if (_api == GiactApiType.REST)
            {
                return AuxGetAccInfoREST(request);
            }
            throw new ArgumentException("API Type, Invalid");
        }

        // Parametros Clases: GiactInquiry_5_8 
        private GiactResult AuxGetAccInfoREST(GiactInquiry request)
        {
            GiactResult response = new GiactResult();

            // Llamamos al servicio de Giact


            HttpResponseMessage responseJson = _clientREST.PostAsync("PostInquiry",
                new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request).ToString())).Result;
            if (responseJson.StatusCode == HttpStatusCode.OK)
            {
                //Serializamos el Json
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<GiactResult>(responseJson.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new ArgumentException(string.Format("StatusCode:{0} ({1})", (int)responseJson.StatusCode, responseJson.ReasonPhrase));
            }
            // Resultado Clases: GiactResult_5_8, VerificationResponse_5_8, AccountResponseCode, FundsConfirmationResult 
            return response;
        }


        private GiactResult AuxGetAccInfoSOAP(GiactInquiry request)
        {
            GiactResult response = new GiactResult();

            // Llamamos al servicio de Giact
            GiactService.AuthenticationHeader authenticationHeader = new GiactService.AuthenticationHeader
            {
                ApiUsername = _apiUsername,
                ApiPassword = _apiPassword
            };

            GiactService.GiactInquiry_5_8 soapRequest = new GiactService.GiactInquiry_5_8
            {
                UniqueId = request.UniqueId,
                Check = new GiactService.CheckInformation_5_8
                {
                    RoutingNumber = request.Check.RoutingNumber,
                    AccountNumber = request.Check.AccountNumber,
                    CheckAmount = request.Check.CheckAmount,
                    CheckNumber = request.Check.CheckNumber
                },
                GVerifyEnabled = request.GVerifyEnabled,
                FundsConfirmationEnabled = request.FundsConfirmationEnabled,
            };

            switch (request.Check.AccountType)
            {
                case GiactAccountType.Checking:
                    soapRequest.Check.AccountType = GiactService.BankAccountType.Checking;
                    break;
                case GiactAccountType.Savings:
                    soapRequest.Check.AccountType = GiactService.BankAccountType.Savings;
                    break;
                case GiactAccountType.Other:
                    soapRequest.Check.AccountType = GiactService.BankAccountType.Other;
                    break;
                default:
                    break;
            }

            //var logPath = IOUtil.AppBaseDirForce("GiactLog");
            //System.IO.File.WriteAllText($"{logPath}{request.UniqueId}_Request.json", JsonConvert.SerializeObject(soapRequest));

            //AQUI ESTA LA LLAMADA
            var giactResult = _clientSOAP.PostInquiry(authenticationHeader, soapRequest);

            //System.IO.File.WriteAllText($"{logPath}{request.UniqueId}_Response.json", JsonConvert.SerializeObject(giactResult));


            response.BankName = giactResult.BankName ?? "";
            response.ItemReferenceId = giactResult.ItemReferenceId;
            response.CreatedDate = giactResult.CreatedDate;
            response.ErrorMessage = giactResult.ErrorMessage;

            response.AccountAddedDate = giactResult.AccountAddedDate;
            response.AccountLastUpdatedDate = giactResult.AccountLastUpdatedDate;
            response.AccountClosedDate = giactResult.AccountClosedDate;
            response.VoidedCheckImage = giactResult.VoidedCheckImage;

            //Preferimos usar el nombre del enum, facilita el analisis en logs y front, facilita reevaluar el estatus para despliegue
            response.VerificationResponse = giactResult.VerificationResponse?.ToString();
            response.AccountResponseCode = giactResult.AccountResponseCode?.ToString();
            response.FundsConfirmationResult = giactResult.FundsConfirmationResult?.ToString();

            AddNote(response, "Veri", response.VerificationResponse);
            AddNote(response, "Acco", response.AccountResponseCode);
            AddNote(response, "Fund", response.FundsConfirmationResult);

            #region comantado, se va a quitar
            /*
            string noteStatus = "";
            switch (giactResult.VerificationResponse)
            {
                case GiactService.VerificationResponse_5_8.Error:
                    response.VerificationResponse = GiactVerificationResponse.Error;
                    noteStatus = "ERROR";
                    break;
                case GiactService.VerificationResponse_5_8.PrivateBadChecksList:
                    response.VerificationResponse = GiactVerificationResponse.PrivateBadChecksList;
                    noteStatus = "WARN";
                    break;
                case GiactService.VerificationResponse_5_8.Declined:
                    response.VerificationResponse = GiactVerificationResponse.Declined;
                    noteStatus = "WARN";
                    break;
                case GiactService.VerificationResponse_5_8.RejectItem:
                    response.VerificationResponse = GiactVerificationResponse.RejectItem;
                    noteStatus = "ERROR";
                    break;
                case GiactService.VerificationResponse_5_8.AcceptWithRisk:
                    response.VerificationResponse = GiactVerificationResponse.AcceptWithRisk;
                    noteStatus = "WARN";
                    break;
                case GiactService.VerificationResponse_5_8.RiskAlert:
                    response.VerificationResponse = GiactVerificationResponse.RiskAlert;
                    noteStatus = "WARN";
                    break;
                case GiactService.VerificationResponse_5_8.Pass:
                    response.VerificationResponse = GiactVerificationResponse.Pass;
                    noteStatus = "INFO";
                    break;
                case GiactService.VerificationResponse_5_8.PassNdd:
                    response.VerificationResponse = GiactVerificationResponse.PassNdd;
                    noteStatus = "INFO";
                    break;
                case GiactService.VerificationResponse_5_8.NegativeData:
                    response.VerificationResponse = GiactVerificationResponse.NegativeData;
                    noteStatus = "WARN";
                    break;
                case GiactService.VerificationResponse_5_8.NoData:
                    response.VerificationResponse = GiactVerificationResponse.NoData;
                    noteStatus = "INFO";
                    break;
                default:
                    response.VerificationResponse = null;
                    break;
            }

            if (noteStatus != "")
                AddNote(response, "GiactVeri", giactResult.VerificationResponse.ToString(), noteStatus);


            noteStatus = "ERROR";
            switch (giactResult.AccountResponseCode)
            {
                case GiactService.AccountResponseCode.Null:
                    response.AccountResponseCode = null;
                    noteStatus = "";
                    break;
                case GiactService.AccountResponseCode.GS01:
                    response.AccountResponseCode = GiactAccountResponseCode.GS01;
                    break;
                case GiactService.AccountResponseCode.GS02:
                    response.AccountResponseCode = GiactAccountResponseCode.GS02;
                    break;
                case GiactService.AccountResponseCode.GS03:
                    response.AccountResponseCode = GiactAccountResponseCode.GS03;
                    break;
                case GiactService.AccountResponseCode.GS04:
                    response.AccountResponseCode = GiactAccountResponseCode.GS04;
                    break;
                case GiactService.AccountResponseCode.GP01:
                    response.AccountResponseCode = GiactAccountResponseCode.GP01;
                    break;
                case GiactService.AccountResponseCode.RT00:
                    response.AccountResponseCode = GiactAccountResponseCode.RT00;
                    noteStatus = "INFO";
                    break;
                case GiactService.AccountResponseCode.RT01:
                    response.AccountResponseCode = GiactAccountResponseCode.RT01;
                    break;
                case GiactService.AccountResponseCode.RT02:
                    response.AccountResponseCode = GiactAccountResponseCode.RT02;
                    break;
                case GiactService.AccountResponseCode.RT03:
                    response.AccountResponseCode = GiactAccountResponseCode.RT03;
                    noteStatus = "WARN";
                    break;
                case GiactService.AccountResponseCode.RT04:
                    response.AccountResponseCode = GiactAccountResponseCode.RT04;
                    break;
                case GiactService.AccountResponseCode.RT05:
                    response.AccountResponseCode = GiactAccountResponseCode.RT05;
                    noteStatus = "";
                    break;
                case GiactService.AccountResponseCode._1111:
                    response.AccountResponseCode = GiactAccountResponseCode._1111;
                    noteStatus = "INFO";
                    break;
                case GiactService.AccountResponseCode._2222:
                    response.AccountResponseCode = GiactAccountResponseCode._2222;
                    noteStatus = "INFO";
                    break;
                case GiactService.AccountResponseCode._3333:
                    response.AccountResponseCode = GiactAccountResponseCode._3333;
                    noteStatus = "INFO";
                    break;
                case GiactService.AccountResponseCode._5555:
                    response.AccountResponseCode = GiactAccountResponseCode._5555;
                    noteStatus = "WARN";
                    break;
                case GiactService.AccountResponseCode._7777:
                    response.AccountResponseCode = GiactAccountResponseCode._7777;
                    noteStatus = "";
                    break;
                case GiactService.AccountResponseCode._8888:
                    response.AccountResponseCode = GiactAccountResponseCode._8888;
                    noteStatus = "";
                    break;
                case GiactService.AccountResponseCode._9999:
                    response.AccountResponseCode = GiactAccountResponseCode._9999;
                    noteStatus = "";
                    break;
                case GiactService.AccountResponseCode.GN01:
                    response.AccountResponseCode = GiactAccountResponseCode.GN01;
                    noteStatus = "WARN";
                    break;
                case GiactService.AccountResponseCode.GN05:
                    response.AccountResponseCode = GiactAccountResponseCode.GN05;
                    break;
                case GiactService.AccountResponseCode.ND00:
                    response.AccountResponseCode = GiactAccountResponseCode.ND00;
                    noteStatus = "INFO";
                    break;
                case GiactService.AccountResponseCode.ND01:
                    response.AccountResponseCode = GiactAccountResponseCode.ND01;
                    break;
                default:
                    response.AccountResponseCode = null;
                    noteStatus = "INFO";
                    break;
            }

            if (noteStatus != "")
                AddNote(response, "GiactAcco", giactResult.AccountResponseCode.ToString(), noteStatus);

            noteStatus = "";
            switch (giactResult.FundsConfirmationResult)
            {
                case GiactService.FundsConfirmationResult.Null:
                    response.FundsConfirmationResult = null;
                    break;
                case GiactService.FundsConfirmationResult.NonParticipatingBank:
                    response.FundsConfirmationResult = GiactFundsConfirmationResult.NonParticipatingBank;
                    noteStatus = "WARN";
                    break;
                case GiactService.FundsConfirmationResult.InvalidAccountNumber:
                    response.FundsConfirmationResult = GiactFundsConfirmationResult.InvalidAccountNumber;
                    noteStatus = "WARN";
                    break;
                case GiactService.FundsConfirmationResult.AccountClosed:
                    response.FundsConfirmationResult = GiactFundsConfirmationResult.AccountClosed;
                    noteStatus = "ERROR";
                    break;
                case GiactService.FundsConfirmationResult.InsufficientFunds:
                    response.FundsConfirmationResult = GiactFundsConfirmationResult.InsufficientFunds;
                    noteStatus = "WARN";
                    break;
                case GiactService.FundsConfirmationResult.SufficientFunds:
                    response.FundsConfirmationResult = GiactFundsConfirmationResult.SufficientFunds;
                    break;
                default:
                    response.FundsConfirmationResult = null;
                    break;
            }

            if (noteStatus != "")
                AddNote(response, "GiactFund", giactResult.FundsConfirmationResult.ToString(), noteStatus);
            */
            #endregion


            return response;
        }

        private void AddNote(GiactResult res, string aType, string aName)
        {
            var st = ResolveStatusForNote(aType, aName);
            if (st.IsBlank()) return; //sin estatus no agrega nota

            var langTag = $"Giact{aType}_{aName}";

            var n = new AccountCautionNote
            {
                Source = "GIACT",
                ResType = aType,
                ResName = aName,
                Text = langTag,
                TextTag = langTag,
                Status = st,
                ReviewDate = DateTime.Now,
            };
            res.Notes.Add(n);
        }


        public static string ResolveStatusForNote(string cual, string enumName)
        {
            string noteStatus = "";

            //--------------------------
            if (cual.EqualText("Veri"))
            {
                noteStatus = "INFO";

                if (Enum.TryParse<GiactService.VerificationResponse_5_8>(enumName, true, out var mValue))
                {
                    switch (mValue)
                    {
                        case GiactService.VerificationResponse_5_8.Error:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.VerificationResponse_5_8.PrivateBadChecksList:
                            noteStatus = "WARN";
                            break;
                        case GiactService.VerificationResponse_5_8.Declined:
                            noteStatus = "WARN";
                            break;
                        case GiactService.VerificationResponse_5_8.RejectItem:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.VerificationResponse_5_8.AcceptWithRisk:
                            noteStatus = "WARN";
                            break;
                        case GiactService.VerificationResponse_5_8.RiskAlert:
                            noteStatus = "WARN";
                            break;
                        case GiactService.VerificationResponse_5_8.Pass:
                            noteStatus = "INFO";
                            break;
                        case GiactService.VerificationResponse_5_8.PassNdd:
                            noteStatus = "INFO";
                            break;
                        case GiactService.VerificationResponse_5_8.NegativeData:
                            noteStatus = "WARN";
                            break;
                        case GiactService.VerificationResponse_5_8.NoData:
                            noteStatus = "INFO";
                            break;
                        default:
                            noteStatus = "INFO";
                            break;
                    }//switch
                }
            }//Veri


            //--------------------------
            if (cual.EqualText("Acco"))
            {
                noteStatus = "ERROR";
                if (Enum.TryParse<GiactService.AccountResponseCode>(enumName, true, out var mValue))
                {
                    switch (mValue)
                    {
                        case GiactService.AccountResponseCode.Null:
                            noteStatus = "";
                            break;
                        case GiactService.AccountResponseCode.GS01:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.GS02:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.GS03:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.GS04:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.GP01:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.RT00:
                            noteStatus = "INFO";
                            break;
                        case GiactService.AccountResponseCode.RT01:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.RT02:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.RT03:
                            noteStatus = "WARN";
                            break;
                        case GiactService.AccountResponseCode.RT04:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.RT05:
                            noteStatus = "";
                            break;
                        case GiactService.AccountResponseCode._1111:
                            noteStatus = "INFO";
                            break;
                        case GiactService.AccountResponseCode._2222:
                            noteStatus = "INFO";
                            break;
                        case GiactService.AccountResponseCode._3333:
                            noteStatus = "INFO";
                            break;
                        case GiactService.AccountResponseCode._5555:
                            noteStatus = "WARN";
                            break;
                        case GiactService.AccountResponseCode._7777:
                            noteStatus = "";
                            break;
                        case GiactService.AccountResponseCode._8888:
                            noteStatus = "";
                            break;
                        case GiactService.AccountResponseCode._9999:
                            noteStatus = "";
                            break;
                        case GiactService.AccountResponseCode.GN01:
                            noteStatus = "WARN";
                            break;
                        case GiactService.AccountResponseCode.GN05:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.AccountResponseCode.ND00:
                            noteStatus = "INFO";
                            break;
                        case GiactService.AccountResponseCode.ND01:
                            noteStatus = "ERROR";
                            break;
                        default:
                            noteStatus = "INFO";
                            break;
                    }//switch
                }
            }//Acco


            //--------------------------
            if (cual.EqualText("Fund"))
            {
                noteStatus = "";
                if (Enum.TryParse<GiactService.FundsConfirmationResult>(enumName, true, out var mValue))
                {
                    switch (mValue)
                    {
                        case GiactService.FundsConfirmationResult.Null:
                            break;
                        case GiactService.FundsConfirmationResult.NonParticipatingBank:
                            noteStatus = "WARN";
                            break;
                        case GiactService.FundsConfirmationResult.InvalidAccountNumber:
                            noteStatus = "WARN";
                            break;
                        case GiactService.FundsConfirmationResult.AccountClosed:
                            noteStatus = "ERROR";
                            break;
                        case GiactService.FundsConfirmationResult.InsufficientFunds:
                            noteStatus = "WARN";
                            break;
                        case GiactService.FundsConfirmationResult.SufficientFunds:
                            break;
                        default:
                            break;
                    }//switch
                }
            }//Fund

            return noteStatus;
        }


    }
}
