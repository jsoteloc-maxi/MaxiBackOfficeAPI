using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.Agent.Infrastructure.OrbographWebService;

namespace Maxi.BackOffice.Agent.Infrastructure.ExternalServices
{
    public class Orbograph
    {
        private OrboServiceClient OrboServiceProxy;
        //private readonly CancellationTokenSource _cancellationTokenSource;
        private ConcurrentQueue<ValidationResponse> resqueue = new ConcurrentQueue<ValidationResponse>();
        //private readonly TaskFactory taskFactory;

        private String orboUser;
        private String orboPass;
        private bool _connected;

        public Orbograph()
        {
            _connected = false;
            OrboServiceProxy = new OrboServiceClient();
        }
        

        private HealthResponse HealthInfo()
        {
            HealthResponse HealthinfoResponse = new HealthResponse();
            do
            {
                HealthinfoResponse = OrboServiceProxy.GetHealthInfo();
                if (!(HealthinfoResponse.EngineConnected))
                {
                    Thread.Sleep(5000);
                    Console.Out.WriteLine("Engine not Connected, waiting 5 seconds");
                }
            } while (!(HealthinfoResponse.EngineConnected));

            return HealthinfoResponse;
        }


        public ValidationResponse ValidateCheck(ValidationRequest request)
        {
            Connect(orboUser, orboPass);
            ValidationResponse response = null;

            response = OrboServiceProxy.ValidateCheck(request, 0);

            if (response.Reco.ErrorCode == 1000) // ErrorOk = 1000;
            {
                //NotTested Test result is unknown.The reasons can be any of the following:
                //  • Not enough input data was supplied • Engine failed to recognize a required field 
                //Failed Test failed. Score is below the threshold.
                //Passed Test passed.Score is above the threshold.
                //Skipped Test skipped.The reasons can be any of the following:
                //  • The amount in the image is less than the minimal amount defined for the entire profile • The amount in the image is less than the minimal amount defined for the specific test in the profile

                // Resultado de las validaciones
                //switch (response.TestResult.Result)
                //{
                //    case ValidationResultType.Passed:
                //        txtValidation.Text = "Passed";
                //        break;
                //    case ValidationResultType.NotTested:
                //        txtValidation.Text = "NotTested";
                //        break;
                //    case ValidationResultType.Skipped:
                //        txtValidation.Text = "Skipped";
                //        break;
                //    case ValidationResultType.Failed:
                //        txtValidation.Text = "Failed";
                //        break;

                //}

                //AssignResults(response, ref prmItem);
            }

            //var Amount = response.TestResult.TestResults.SingleOrDefault(tr => tr.Type == OrboService.ValidationType.Amount);
            //var Signature = response.TestResult.TestResults.SingleOrDefault(tr => tr.Type == OrboService.ValidationType.Signature);
            //Signature.Result  - for loop

            // Grabamos el log de la respuesta 
            var lDir = @"D:\CC\Images\";
            if (System.IO.Directory.Exists(lDir))
            {
                string strResponse = ConvertTo.ToJSON(response);
                string vlFName = string.Format(lDir + "{0:yyyyMMdd_HHmmss}.txt", DateTime.Now);
                System.IO.File.WriteAllText(vlFName, strResponse);
            }

            return response;
        }

        public ValidationRequest CreateValidationRequest(byte[] prmImage, string filename) //, Hashtable HashNames)
        {
            ValidationRequest checkValidationRequest = null;
            checkValidationRequest = new ValidationRequest
            {
                Reco = new RecoRequest
                {
                    UniqueDocName = filename,
                    ImageData = new ImageParameters
                    {
                        ImageBuffer = prmImage,  // GetFileData(fullname),  // 
                        Page = ImagePage.Front
                    },

                    // Listado de campos que se requieren obtener del cheque
                    RequiredFields = new List<RecoField>
                    {
                        RecoField.Date,
                        RecoField.Car,
                        RecoField.Lar, // Se usaba el LAR para comparar con el CAR sin embargo hay un campo que indica CAR/LAR discrepancy
                        RecoField.Micr,
                        RecoField.Serial,
                        RecoField.Payee,
                        RecoField.Payer,
                    }
                },

                // Metada es enviada para ayudar a la Validacion de datos
                MetaData = new DocumentMetaData
                {
                    MicrLineData = "",  //MicrLine,
                    Amount = "",  //Amount_For_AV,        //Amount_For
                    IsOnUs = false,
                    Channel = ChannelType.Teller,
                    AccountHolder = new NameAddress
                    {
                        Name = "",
                        City = "",
                        State = "",
                        Zip = ""
                    },
                    PlaceOfDeposit = new NameAddress
                    {
                        Name = "",
                        City = "",
                        State = "",
                    },
                    AccountCreateDate = DateTime.Parse("06/2/2011"),  //null,  // 
                    CurrentDate = DateTime.Now,  //DateTime.ParseExact("08/06/2019", "M/d/yyyy", null),
                    IssuedName = null,
                },
                //  ProfileName: NO se usa por el momento, se tiene por compatibilidad para uso futuro
                ProfileName = "FirstTest",  //ProfileName   //ProfileName // could be another profile name

            };

            return checkValidationRequest;
        }

        public void Connect(string user, string pass)
        {
            if (!_connected)
            {
                _connected = true;
                orboUser = user;
                orboPass = pass;
                //ServicePointManager.ServerCertificateValidationCallback = IgnoreCertificateErrorHandler;
                ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11;
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                OrboServiceProxy.ClientCredentials.UserName.UserName = orboUser;
                OrboServiceProxy.ClientCredentials.UserName.Password = orboPass;
            }
        }
        
        private  bool IgnoreCertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

    }
}
