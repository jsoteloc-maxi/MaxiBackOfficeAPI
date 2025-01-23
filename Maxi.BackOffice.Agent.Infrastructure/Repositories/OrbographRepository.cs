using System.ServiceModel;
using Microsoft.Data.SqlClient;
using NLog;
using Maxi.BackOffice.CrossCutting.Common.Configurations;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.ExternalServices;
using Maxi.BackOffice.Agent.Infrastructure.OrbographWebService;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Extensions;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class OrbographRepository : IOrbographRepository
    {
        private readonly UnitOfWorkSqlServerAdapter db;
        private readonly AppCurrentSessionContext session;

        //private string slogFilename = "";
        //private StringBuilder slog = new StringBuilder(100);
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger("Orbograph");
        private static bool isLogSetted = false;


        public OrbographRepository(UnitOfWorkSqlServerAdapter dbAdapter)
        {
            this.db = dbAdapter;
            this.session = dbAdapter.SessionCtx;
            LogSetup();
        }

        private void LogAdd(string level, string msg, params object[] args)
        {
            if (level == "d" || level.IsBlank()) logger.Debug(msg, args);
            if (level == "i") logger.Info(msg, args);
        }

        private void LogErr(string msg, Exception ex = null)
        {
            logger.Error(msg);
            if (ex != null)
                logger.Error(ex);
            return;
        }


        public ValidationResponse ValidateCheck(Byte[] pic, string picName)
        {
            ValidationResponse res;
            OrbographLogEntity logEntity = null;

            //slogFilename = "";
            //slog.Clear();
            try
            {
                LogAdd("i", "-------------------------------");
                LogAdd("i", "OrbographRepository ValidateCheck");
                LogAdd("i", "DATOS DE SESION:");
                LogAdd("i", "{@session}", session);
                LogAdd("i", "");

                var orboclient = new Orbograph();

                //Buscamos credenciales de orbograph
                LogAdd("d", "busca credenciales en GlobalAttributes");
                var user = db.GlobalAttr("OrbographUser");
                var pass = db.GlobalAttr("OrbographPass");
                orboclient.Connect(user, pass);

                LogAdd("d", "CreateValidationRequest");
                var req = orboclient.CreateValidationRequest(pic, picName);

                LogAdd("i", "REQUEST:");
                LogAdd("i", "{@req}", req);
                //LogAdd("i", ConvertTo.ToJSON(req));
                LogAdd("i", "");

                LogAdd("d", "DbSaveLog Request");
                logEntity = DbSaveLog(null, req);

                LogAdd("d", "llama WS de Orbo");
                res = orboclient.ValidateCheck(req);

                LogAdd("i", "RESPONSE:");
                LogAdd("i", ConvertTo.ToJSON(res));
                LogAdd("i", "");

                if (res == null) new Exception("Orbo Response is null");

                LogAdd("i", "DbSaveLog Response");
                DbSaveLog(logEntity, res);
    
                LogAdd("i", "return. ");
                FileSaveLog();


                #region comentado
                /*
                if (res.Reco.ErrorCode == (int)ErrorCodes.ErrorOk) // ErrorOk = 1000;
                {
                    //NotTested Test result is unknown.The reasons can be any of the following:
                    //  • Not enough input data was supplied • Engine failed to recognize a required field 
                    //Failed Test failed. Score is below the threshold.
                    //Passed Test passed.Score is above the threshold.
                    //Skipped Test skipped.The reasons can be any of the following:
                    //  • The amount in the image is less than the minimal amount defined for the entire profile • The amount in the image is less than the minimal amount defined for the specific test in the profile

                    // Resultado de las validaciones
                    switch (res.TestResult.Result)
                    {
                        case ValidationResultType.Passed:
                            break;
                        case ValidationResultType.NotTested:
                            break;
                        case ValidationResultType.Skipped:
                            break;
                        case ValidationResultType.Failed:
                            break;
                    }

                    // todo: se tiene que grabar un log en base de datos el resultado de la consulta a Orbograph
                    //AssignResultsFromOrbo(res, ref vlItemInfo);

                };*/
                #endregion
            }
            catch (FaultException<BadIntegrationFault> ex)
            {
                LogErr("BadIntegration", ex);
                FileSaveLog();
                DbSaveLog(logEntity, ex);
                throw;
            }
            catch (FaultException<ServerDisconnectedFault> ex)
            {
                LogErr("Exception:", ex);
                FileSaveLog();
                DbSaveLog(logEntity, ex);
                throw;
            }
            catch (Exception ex)
            {
                LogErr("Exception:", ex);
                FileSaveLog();
                DbSaveLog(logEntity, ex);
                throw;
            }

            return res;
        }


        public OrbographLogEntity DbSaveLog(OrbographLogEntity entity, object data)
        {
            //Usa conexion alternativa para logs en db
            SqlTransaction ltran = null;
            using (var lconn = new SqlConnection(AppSettings.ConnectionString_DbLogs))
            {
                lconn.Open();
                try
                {
                    OrbographLogEntity m;
                    ltran = lconn.BeginTransaction();

                    m = entity;
                    if (entity == null)
                    {
                        m = new OrbographLogEntity
                        {
                            DateRecord = DateTime.Now,
                            IdAgent = session.IdAgent,
                            IdUser = session.IdUser,
                        };
                    }

                    if (data is Exception ex)
                    {
                        m.Exception = ex.Message + "; " + (ex.InnerException != null ? ex.InnerException.Message : "");
                        m.ErrorCode = -1;
                        m.ErrorCodeName = "Exception";
                    }

                    if (data is ValidationRequest req)
                    {
                        m.RequestJSON = ConvertTo.ToJSON(req);
                    }

                    if (data is ValidationResponse res)
                    {
                        m.ResponseJSON = ConvertTo.ToJSON(res);
                        m.ErrorCode = res.Reco.ErrorCode;
                        m.ErrorCodeName = Enum.GetName(typeof(ErrorCodes), (ErrorCodes)res.Reco.ErrorCode);

                        if (res.Reco != null)
                        {
                            m.Micr = ExtractResponseField(res, "micr").Result;
                            m.FieldCarValue = ExtractResponseField(res, "car").Result;
                            m.FieldCarScore = ExtractResponseField(res, "car").Score;

                            if (res.Reco.Micr != null)
                            {
                                if (res.Reco.Micr.Amount != null)
                                {
                                    m.MicrAmount = res.Reco.Micr.Amount.Value;
                                    m.MicrAmountScore = res.Reco.Micr.Amount.Score;
                                }

                                m.EPC = res.Reco.Micr.Epc.ToString();

                                if (res.Reco.Micr.Routing != null)
                                    m.Routing = res.Reco.Micr.Routing.Value;

                                if (res.Reco.Micr.OnusAccount != null)
                                    m.Account = res.Reco.Micr.OnusAccount.Value;

                                if (res.Reco.Micr.OnusCheck != null)
                                    m.CheckNum = res.Reco.Micr.OnusCheck.Value;

                                if (res.Reco.Micr.Aux != null && res.Reco.Micr.OnusCheck==null)
                                    m.CheckNum = res.Reco.Micr.Aux.Value;
                            }
                        }
                    }

                    //LogAdd("", "DbSaveLog: " + JsonConvert.SerializeObject(m));

                    if (entity == null)
                        m.Id = m.Insert(lconn, ltran);
                    else
                        m.Update(lconn, ltran);

                    ltran.Commit();
                    return m;
                }
                catch(Exception ex)
                {
                    LogErr("Exception DbSaveLog", ex);
                    if (ltran != null) ltran.Commit(); //forza grabar log
                    throw; 
                }
            }
        }

        private void FileSaveLog()
        {
            NLog.LogManager.Flush();
            

            //if (slogFilename == "")
            //    slogFilename = IOUtil.AppBaseDirForce("OrboLog") + "OrboLog_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            //if (!File.Exists(slogFilename))
            //    File.WriteAllText(slogFilename, slog.ToString());
            //else
            //    File.AppendAllText(slogFilename, slog.ToString());

            //slog.Clear();
        }

        private void LogSetup()
        {
            if (isLogSetted) return;

            NLog.LogManager.Setup().SetupSerialization(s =>
            {
                s.RegisterObjectTransformation<ValidationRequest>(ex => new
                {
                    //Type = ex.GetType().ToString(),
                    ex.ExtensionData,
                    ex.FraudInput,
                    ex.MetaData,
                    ex.Profile,
                    ex.ProfileName,
                    Reco_ExtensionData = ex.Reco.ExtensionData,
                    Reco_RequiredFields = ex.Reco.RequiredFields,
                    Reco_UniqueDocName = ex.Reco.UniqueDocName,
                });
            });

            //ver si esto se hace al inicio de cada peticion
            NLog.MappedDiagnosticsLogicalContext.Set("IdAgent", session.IdAgent);
            NLog.MappedDiagnosticsLogicalContext.Set("IdUser", session.IdUser);
            NLog.MappedDiagnosticsLogicalContext.Set("FrontGuid", session.SessionGuid);

            isLogSetted = true;
            return;
        }

        private Aux_ResponseFieldValue ExtractResponseField(ValidationResponse res, string cual)
        {
            var r = new Aux_ResponseFieldValue();
            if (res.Reco == null) return r;

            foreach(var n in res.Reco.ResponseFields)
            {
                if (n.Type == cual)
                {
                    r.Result = n.Result;
                    r.Score = n.Score;
                    break;
                }
            }
            return r;
        }

    }


    class Aux_ResponseFieldValue
    {
        public string Result = "";
        public int Score = 0;
    }

}
