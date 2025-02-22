using Microsoft.Data.SqlClient;
using Dapper;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.Configurations;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.ExternalServices;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.CrossCutting.Common.Extensions;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoMapper.Configuration.Annotations;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class GiactServiceLogRepository : IGiactServiceLogRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IAplicationContext _dbContext;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;

        //private string slogFilename = "";
        //private readonly StringBuilder slog = new StringBuilder(100);
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger("Giact");
        private static bool isLogSetted = false;

        //public GiactServiceLogRepository(DbContextObjects dbCtx, AppCurrentSessionContext seCtx)
        public GiactServiceLogRepository(IConfiguration configuration, IAplicationContext dbContext, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _appCurrentSessionContext = appCurrentSessionContext;
            LogSetup();
        }

        private void LogSetup()
        {
            if (isLogSetted) return;

            //ver si esto se hace al inicio de cada peticion
            NLog.MappedDiagnosticsLogicalContext.Set("IdAgent", _appCurrentSessionContext.IdAgent);
            NLog.MappedDiagnosticsLogicalContext.Set("IdUser", _appCurrentSessionContext.IdUser);
            NLog.MappedDiagnosticsLogicalContext.Set("FrontGuid", _appCurrentSessionContext.SessionGuid);

            isLogSetted = true;
            return;
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


        public GiactResult ValidateCheck(GiactInquiry request)
        {
            GiactServiceLogEntity logRow = null;
            GiactResult result;
            try
            {
                LogAdd("i", "-------------------------------");
                LogAdd("i", "Giact ValidateCheck");
                LogAdd("i", "DATOS DE SESION:");
                LogAdd("i", "{@session}", _appCurrentSessionContext);
                LogAdd("i", "");

                //Obtener Fee a cobrar por esta verificacion
                LogAdd("i", "Consulta Fee de validacion");
                var fee = new FeeChecksEntity();
                fee = fee.GetByFilter("IdAgent=@IdAgent", new List<SqlParam>
                {
                    new SqlParam{ Name="@IdAgent", Value=_appCurrentSessionContext.IdAgent }
                },
                _dbContext.GetConnection(), _dbContext.GetTransaction()).FirstOrDefault();

                if (fee == null)
                {
                    fee = new FeeChecksEntity();
                    fee.TransactionFee = 0;
                }
                //var fee = db.conn.Query<decimal>("SELECT TransactionFee FROM FeeChecks WHERE IdAgent = @IdAgent", new { _appCurrentSessionContext.IdAgent }, db.tran).firs;

                //if (fee.TransactionFee <= 0)
                //{
                //    //fee.TransactionFee = 5;
                //    throw new Exception("no hay verification fee configurado para este agente");
                //}


                //Asigna un guid por cada peticion
                request.UniqueId = Guid.NewGuid().ToString();

                LogAdd("i", "REQUEST:");
                LogAdd("i", "{@req}", request);
                //LogAdd("i", JsonConvert.SerializeObject(request));
                LogAdd("i", "");


                LogAdd("d", "DbSaveLog Request");
                logRow = DbSaveLog(null, request);


                //llama webservice
                LogAdd("d", "llama WS de Giact");
                var giactApiUsername = _configuration.GetSection("AppSettings")["GiactApiUsername"];
                var giactApiPassword = _configuration.GetSection("AppSettings")["GiactApiPassword"];
                var proxy = new Giact(giactApiUsername,giactApiPassword, GiactApiType.SOAP);
                result = proxy.GetAccInfo(request);

                LogAdd("i", "RESPONSE:");
                LogAdd("i", ConvertTo.ToJSON(result));
                LogAdd("i", "");

                if (result == null)
                    throw new Exception("Giact Response is null");

                if (result.VerificationResponse.EqualText("Error"))
                    throw new Exception(result.ErrorMessage);

                LogAdd("i", "DbSaveLog Response");
                DbSaveLog(logRow, result);

                LogAdd("i", "Db Save CC_AccVerifByAg");
                SaveSuccessResult(new CC_AccVerifByAgEntity
                {
                    Routing = request.Check.RoutingNumber,
                    Account = request.Check.AccountNumber,
                    CheckNum = request.Check.CheckNumber,
                    IdAgent = _appCurrentSessionContext.IdAgent,
                    IdUser = _appCurrentSessionContext.IdUser,
                    IdLog = logRow.Id,
                    Provider = "Giact",
                    VerificationFee = fee.TransactionFee
                });

                LogAdd("i", "i18n Translate Messages");

                foreach (var n in result.Notes)
                {
                    n.SourceText = _dbContext.LangResource("ValidationService");
                    n.Text = _dbContext.LangResource(n.TextTag, "  ");
                }
            }
            catch (Exception ex)
            {
                LogErr("Exception", ex);
                FileSaveLog();
                DbSaveLog(logRow, ex);
                throw ex;
            }

            LogAdd("i", "return. ");
            FileSaveLog();

            result.Notes.Sort((x, y) => y.StatusInt - x.StatusInt);
            return result;
        }

        private int SaveSuccessResult(CC_AccVerifByAgEntity m)
        {
            //Usa conexion alterna para asegurar que siempre grabe
            int id;
            SqlTransaction ltran = null;
            using (var lconn = new SqlConnection(_configuration.GetConnectionString("OPER-D")))
            {
                lconn.Open();
                try
                {
                    ltran = lconn.BeginTransaction();

                    var maker = lconn.Query<CC_GetMakerByAccEntity>(
                        "EXEC dbo.sp_CC_GetMakerByAcc @RoutingNum, @AccountNum", new
                        {
                            RoutingNum = m.Routing,
                            AccountNum = m.Account,
                        }, ltran).FirstOrDefault();

                    m.DateCreated = DateTime.Now;
                    m.IdIssuer = maker != null ? maker.Maker_ID : 0;
                    id = m.Insert(lconn, ltran);
                    ltran.Commit();

                    #region Inserta mov para el balance del agente
                    try
                    {
                        ltran = lconn.BeginTransaction();
                        lconn.Execute("EXEC dbo.sp_ACC_InsRecToAgBalance @MovType, @Id, @IdUser ", new
                        {
                            @MovType = "CHVF",
                            @Id = id,
                            _appCurrentSessionContext.IdUser
                        }, ltran);
                        ltran.Commit();
                    }
                    catch
                    {
                    }
                    #endregion

                    return id;
                }
                catch (Exception ex)
                {
                    LogErr("Exception SaveSuccessResult", ex);
                    if (ltran != null) ltran.Rollback();
                    throw;
                }
            }
        }

        private void FileSaveLog()
        {
            NLog.LogManager.Flush();
        }

        public GiactServiceLogEntity DbSaveLog(GiactServiceLogEntity entity, object data)
        {
            //Usa conexion alternativa para logs en db
            SqlTransaction ltran = null;
            using (var lconn = new SqlConnection(_configuration.GetConnectionString("LOGS-D")))
            {
                lconn.Open();
                try
                {
                    GiactServiceLogEntity m;
                    ltran = lconn.BeginTransaction();

                    m = entity;
                    if (entity == null)
                    {
                        m = new GiactServiceLogEntity
                        {
                            DateRecord = DateTime.Now,
                            IdAgent = _appCurrentSessionContext.IdAgent,
                            IdUser = _appCurrentSessionContext.IdUser,
                            RequestJSON = "",
                            ResponseJSON = "",
                        };
                    }

                    if (data is GiactInquiry req)
                    {
                        m.UniqueId = req.UniqueId;
                        m.RequestJSON = ConvertTo.ToJSON(req);
                    }

                    if (data is GiactResult res)
                    {
                        m.ResponseJSON = ConvertTo.ToJSON(res);
                        m.StError = false;
                        m.Error = "Operación Exitosa";
                    }

                    if (data is Exception ex)
                    {
                        m.ResponseJSON = "";
                        m.StError = true;
                        m.Error = ex.Message + "; " + (ex.InnerException != null ? ex.InnerException.Message : "");
                    }


                    if (entity == null)
                    {
                        m.Id = m.Insert(lconn, ltran);

                        //OJO: guarda la liga al ultimo log en tabla Agent en Base operativa
                        //ver si esto se puede mejorar o separar
                        //lconn.Execute(
                        //    @"UPDATE MAXI.dbo.Agent SET IdLogAccVerif = @Id  WHERE IdAgent = @IdAgent",
                        //    new { m.Id, _appCurrentSessionContext.IdAgent }, ltran);
                    }
                    else
                        m.Update(lconn, ltran);

                    ltran.Commit();
                    return m;
                }
                catch (Exception ex)
                {
                    LogErr("Exception DbSaveLog", ex);
                    if (ltran != null) ltran.Commit();
                    throw;
                }
            }
        }
    }
}
