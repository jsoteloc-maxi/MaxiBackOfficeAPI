using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.CrossCutting.Common.Extensions;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Configurations;
using Dapper;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{

    public class CheckRepository : ICheckRepository
    {
        private readonly UnitOfWorkSqlServerAdapter db;

        private readonly AppCurrentSessionContext session;
        //private readonly DbContextObjects db;
        //private readonly SqlConnection connection;
        //private readonly SqlTransaction transaction;

        //private readonly CCAgFeeCommResEntity entity;

        //public CheckRepository(DbContextObjects dbCtx, AppCurrentSessionContext seCtx)
        public CheckRepository(UnitOfWorkSqlServerAdapter ctx)
        {
            this.db = ctx;
            this.session = db.SessionCtx;
            //connection = db.Conn;
            //transaction = db.Tran;

            //this.db = dbCtx;
            //this.session = seCtx;
            //connection = db.conn;
            //transaction = db.tran;
        }

        public CCAgFeeCommResEntity GetCCAgFeeComm(int IdAgente, decimal CheckAmount, int State)
        {
            //CCAgFeeCommResEntity entity = new CCAgFeeCommResEntity();

            //return entity.GetQuery("EXEC sp_CC_GetAgentFees @IdAgente,@CheckAmount,@State",
            //    new List<SqlParam>
            //    {
            //        new SqlParam() { Name ="@IdAgente" , Value= IdAgente },
            //        new SqlParam() { Name ="@CheckAmount" , Value= CheckAmount },
            //        new SqlParam() { Name ="@State" ,Value=  State },
            //    }, connection, transaction).FirstOrDefault();

            return db.Conn.Query<CCAgFeeCommResEntity>(
                "EXEC sp_CC_GetAgentFees @IdAgente, @CheckAmount, @State ",
                new
                {
                    IdAgente = IdAgente,
                    CheckAmount = CheckAmount,
                    State = State,
                },
                db.Tran).FirstOrDefault();
        }


        public CC_GetMakerByAccEntity GetMakerByAcc(string routing, string account)
        {
            var result = new CC_GetMakerByAccEntity();

            dynamic r = db.Conn.Query(
                "SELECT TOP 1 * FROM dbo.IssuerChecks WITH(NOLOCK) WHERE RoutingNumber=@routing AND AccountNumber=@account " +
                "ORDER BY IdIssuer DESC",
                new { routing, account }, db.Tran).FirstOrDefault();

            if (r != null)
            {
                result.Maker_ID = r.IdIssuer;
                result.MAK_DateCreated = r.DateOfCreation;
                result.MAK_IdUserCreated = r.EnteredByIdUser;
                result.MAK_Name = r.Name;
                result.IdState = 0;
                result.MAK_Active = (r.StActive == true || r.StActive == null) ? true : false;
            }

            return result;
        }

        public List<CC_GetMakerByAccEntity> GetMakerByAcc(int RoutingNum, long AccountNum)
        {
            CC_GetMakerByAccEntity entity = new CC_GetMakerByAccEntity();

            //CC_GetMakerByAccEntity.
            //entity.GetFullTableName()

            return entity.GetQuery("EXEC sp_CC_GetMakerByAcc @RoutingNum, @AccountNum",
                new List<SqlParam>
                {
                    new SqlParam() { Name ="@RoutingNum" , Value= RoutingNum },
                    new SqlParam() { Name ="@AccountNum" , Value= AccountNum }
                }, db.Conn, db.Tran);
        }

        public List<CC_CheckTypeEntity> GetAllCheckTypes()
        {
            var entity = new CC_CheckTypeEntity();
            //return entity.GetAll(connection, transaction);
            return entity.GetByFilter("", new List<SqlParam> { }, db.Conn, db.Tran);
        }

        public List<CC_CheckCompanyFilter> GetAllWordFilter()
        {
            var entity = new CC_CheckCompanyFilter();
            //return entity.GetAll(connection, transaction);
            return entity.GetByFilter("", new List<SqlParam> { }, db.Conn, db.Tran);
        }


        public List<SpCC_GetCautionNotesEntity> GetCautionNotes(string rout, string acc, string checkNum)
        {
            //Todo: ver si esta rutina deberia regresar on objeto AccountValidationInfo o AccountCautionNotes
            //ya que en realidad regresara toda la info de notas de maxi y giact e informacion de header
            //como BankName, ValidationDate etc
            
            var maker = GetMakerByAcc(rout, acc);
            var IdIssuer = maker.Maker_ID;

            var rows = db.Conn.Query<SpCC_GetCautionNotesEntity>(
                "EXEC dbo.sp_CC_GetCautionNotes @rout, @acc, @IdIssuer, @checkNum ",
                new { rout, acc, IdIssuer, checkNum  },
                db.Tran,true,Convert.ToInt32(AppSettings.CommandTimeout)).ToList();

            //-------------------
            #region Extrae notas recientes de giact
            if (IdIssuer > 0)
            {
                //asegurar que el issuer este bien asignado
                db.Conn.Execute(
                " UPDATE CC_AccVerifByAg SET IdIssuer = @IdIssuer" +
                " WHERE Routing=@rout AND Account=@acc AND (IdIssuer IS NULL OR IdIssuer=0)",
                new { IdIssuer, rout, acc }, db.Tran);


                //Extraer notas recientes de giact
                var json = db.Conn.Query(@"
                    SELECT TOP 1 D.ResponseJSON, [ReviewDate]=CAST(V.DateCreated AS DATE)
                    FROM dbo.CC_AccVerifByAg V WITH(NOLOCK)
                    JOIN MAXILOG.dbo.GiactServiceLog D WITH(NOLOCK) ON (D.Id = V.IdLog)
                    WHERE V.IdIssuer = @IdIssuer
                    AND CAST(V.DateCreated AS DATE) >= CAST(DATEADD(dd,-3,GETDATE()) AS DATE)
                    ORDER BY IdAccVerifByAg DESC
                    ",
                    new { IdIssuer }, db.Tran).FirstOrDefault();
                if (json != null)
                {
                    GiactResult verif = ConvertTo.FromJSON<GiactResult>(json.ResponseJSON);
                    foreach (var note in verif.Notes)
                    {
                        if (note.ResName.IsBlank())
                        {
                            var s = note.Text.Remove(0, 5);
                            var i = s.IndexOf('_');
                            if (i >= 0) note.ResType = s.Substring(0, i);
                            if (i >= 0) note.ResName = s.Substring(i + 1);
                        }

                        if (note.TextTag.IsBlank())
                            note.TextTag = note.Text;

                        var noteEntity = new SpCC_GetCautionNotesEntity
                        {
                            Source = note.Source,
                            Text = note.Text,
                            TextTag = note.TextTag,
                            Status = ExternalServices.Giact.ResolveStatusForNote(note.ResType, note.ResName),
                            ReviewDate = json.ReviewDate,
                        };

                        rows.Add(noteEntity);
                    }
                }
            }
            #endregion

            foreach (var n in rows)
            {
                n.SourceText = "MAXI";

                if (!n.Source.IsBlank() && !n.Source.EqualText("MAXI"))
                    n.SourceText = db.LangResource("ValidationService");

                n.Text = db.LangResource(n.Text);
            }

            return rows;
        }

        public SpCC_GetAccountCheckSummaryEntity GetAccountCheckSummary(string rout, string acc)
        {
            
            return
                db.Conn.Query<SpCC_GetAccountCheckSummaryEntity>(
                "EXEC dbo.sp_CC_GetAccountCheckSummary @RoutingNum=@R, @AccountNum=@A, @Idlang=@L",
                new { R=rout, A=acc, L=session.IdLang },
                db.Tran).FirstOrDefault();

            //var entity = new SpCC_GetAccountCheckSummaryEntity();

            //return
            //entity.GetQuery("EXEC dbo.sp_CC_GetAccountCheckSummary @RoutingNum, @AccountNum",
            //    new List<SqlParam>
            //    {
            //        new SqlParam() { Name ="@RoutingNum" , Value= rout },
            //        new SqlParam() { Name ="@AccountNum" , Value= acc }
            //    }, connection, transaction).FirstOrDefault();
        }

        public List<CC_IssuerActionCheck> GetIssuerAction(int idIssuer)
        {


            var rows = db.Conn.Query<CC_IssuerActionCheck>(
              "EXEC dbo.st_VerifyDenyIssuers @IdIssuer ",
              new { idIssuer },
              db.Tran).ToList();

            return rows;

     
        }


        string SqlRecentChecks(string kind)
        {
            var filtro = "CH.IdIssuer = @IdIssuer";

            if (kind == "customer")
                filtro = "CH.IdCustomer = @IdCustomer";

            var sql = $@"SELECT TOP 30
                CH.IdCheck,
                CH.DateOfIssue,
                CH.DateOfMovement,
                CH.Amount,
                CH.IdIssuer,
                CH.IssuerName,
                CH.IdCustomer,
                CONCAT(CH.Name,' ',CH.FirstLastName,' ',CH.SecondLastName) [CustomerName],
                CH.CheckNumber,
                CH.RoutingNumber,
                CH.Account,
                CH.IdStatus,
                ST.StatusName,
                CD.Note[LastNote],
                CH.IdAgent,
				AG.AgentName
            FROM Checks CH WITH(NOLOCK)
            JOIN [dbo].[CheckDetails] CD WITH(NOLOCK) ON CD.IdCheck=CH.IdCheck AND CD.IdStatus=CH.IdStatus
            JOIN [dbo].[Status] ST WITH(NOLOCK) ON ST.IdStatus=CH.IdStatus
            LEFT JOIN [dbo].[Agent] AG WITH(NOLOCK) ON AG.IdAgent=CH.IdAgent
            WHERE CAST(CH.DateOfMovement AS DATE) >= CAST(DATEADD(M,-3,GETDATE()) AS DATE)
            AND {filtro}
            ORDER BY CH.IdCheck DESC";
            return sql;
        }
                
        public dynamic GetRecentChecksByCustomer(int idCustomer)
        {
            var sql = SqlRecentChecks("customer");
            return db.Conn.Query(sql, new { IdCustomer = idCustomer }, db.Tran);
        }
        public dynamic GetRecentChecksByCustomer(int idCustomer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null)
        {
            PaginationResponse<List<CheckTiny>> result = new PaginationResponse<List<CheckTiny>>();
            result.Error = new List<ErrorDto>();
            //Si las fechas son diferentes de null y la fecha de inicio es mayor  a la final regresar error.
            if ((startDate != null && endDate != null) && (startDate > endDate))
                result.Error.Add(new ErrorDto { Code = "1", Description = "The StartDate parameter must be less than the EndDate parameter" });
            //Si el parametro sortColumn es diferente de null y no esta en la lista de columnas para ordenar regresar un error
            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn.ToUpper())
                {
                    case "STATUS":
                        break;
                    case "DATE":
                        break;
                    case "ISSUER":
                        break;
                    default:
                        result.Error.Add(new ErrorDto { Code = "2", Description = "The SortColumn parameter must be Status, Date, Issuer" });
                        break;
                }
            }
            //Si el parametro sortOrder es diferente de null y no esta en la lista de ordenamiento regresar un error
            if (!string.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder.ToUpper())
                {
                    case "ASC":
                        break;
                    case "DESC":
                        break;
                    default:
                        result.Error.Add(new ErrorDto { Code = "3", Description = "The SortOrder parameter must be ASC or DESC" });
                        break;
                }

            }
            if (result.Error.Count > 0)
                return result;
            var rows = db.Conn.QueryMultiple(
             "EXEC dbo.st_GetCheckHistorybyIdCustomer @IdCustomer, @StarDate, @EndDate, @Paged, @Offset,@Limit,@sortColumn, @sortOrder",
             new { IdCustomer = idCustomer, StarDate = startDate, EndDate = endDate, Paged = paged, Offset = offset, Limit = limit, SortColumn = sortColumn, SortOrder = sortOrder  },
             db.Tran);
            result.Data = (List<CheckTiny>)rows.Read<CheckTiny>();
            result.Pagination = rows.Read<Pagination>().FirstOrDefault();
            return result;
        }

        public dynamic GetRecentChecksByIssuer(int idIssuer)
        {
            var sql = SqlRecentChecks("issuer");
            return db.Conn.Query(sql, new { IdIssuer = idIssuer }, db.Tran);
        }
        public dynamic GetRecentChecksByIssuer(int idIssuer, DateTime? startDate, DateTime? endDate, bool? paged, int? offset, int? limit, string sortColumn = null, string sortOrder = null)
        {
            PaginationResponse<List<CheckTiny>> result = new PaginationResponse<List<CheckTiny>>();
            result.Error = new List<ErrorDto>();

            //Si las fechas son diferentes de null y la fecha de inicio es mayor  a la final regresar error.
            if ((startDate != null && endDate != null) && (startDate > endDate))
                result.Error.Add(new ErrorDto { Code = "1", Description = "The StartDate parameter must be less than the EndDate parameter" });
            //Si el parametro sortColumn es diferente de null y no esta en la lista de columnas para ordenar regresar un error
            if (!string.IsNullOrEmpty(sortColumn))
            {
                switch (sortColumn.ToUpper())
                {
                    case "STATUS":
                        break;
                    case "DATE":
                        break;                    
                    default:
                        result.Error.Add(new ErrorDto { Code = "2", Description = "The SortColumn parameter must be Status, Date" });
                        break;
                }
            }
            //Si el parametro sortOrder es diferente de null y no esta en la lista de ordenamiento regresar un error
            if (!string.IsNullOrEmpty(sortOrder))
            {
                switch (sortOrder.ToUpper())
                {
                    case "ASC":
                        break;
                    case "DESC":
                        break;
                    default:
                        result.Error.Add(new ErrorDto { Code = "3", Description = "The SortOrder parameter must be ASC or DESC" });
                        break;
                }

            }
            if (result.Error.Count > 0)
                return result;
            var rows = db.Conn.QueryMultiple(
             "EXEC dbo.st_GetCheckHistorybyIdIssuer @IdIssuer, @StarDate, @EndDate, @Paged, @Offset,@Limit,@SortColumn,@SortOrder",
             new { IdIssuer = idIssuer, StarDate = startDate, EndDate = endDate, Paged = paged, Offset = offset, Limit = limit, SortColumn= sortColumn, SortOrder = sortOrder },
             db.Tran);
            result.Data = (List<CheckTiny>)rows.Read<CheckTiny>(); 
            result.Pagination = rows.Read<Pagination>().FirstOrDefault();
            return result;
        }


        public dynamic GetChecksProcessedReport(DateTime date1, DateTime date2, string custName, string checkNum)
        {
            int idAgent = session.IdAgent;
            //idAgent = 1242; //test debug

            dynamic rows = db.Conn.Query(
                "EXEC dbo.sp_CC_GetChecksProcessed @date1, @date2, @idAgent, @custName, @checkNum",
                new { date1, date2, idAgent, custName, checkNum },
                db.Tran);

            return rows;

            //var n = new List<dynamic>();
            //foreach(var row in rows)
            //{
            //    dynamic v = new ExpandoObject();
            //    v.Hola = row.xxx;
            //    v.dir = "";
            //    n.Add(v);
            //}
            //return n;
        }


        public dynamic GetChecksRejectedReport(DateTime date1, DateTime date2, string custName, string checkNum, string printed)
        {
            int idAgent = session.IdAgent;
            //idAgent = 1242; //test debug

            if (printed.IsBlank()) printed = "2"; //todos

            dynamic rows = db.Conn.Query(
                "EXEC dbo.sp_CC_GetChecksRejected @date1, @date2, @idAgent, @custName, @checkNum, @printed",
                new { date1, date2, idAgent, custName, checkNum, printed },
                db.Tran);

            return rows;
        }

        public dynamic GetCheckEditedElements(int idCheck)
        {
            dynamic rows = db.Conn.Query("SELECT * FROM CheckEdits WITH(NOLOCK) WHERE IdCheck = @idCheck", new { idCheck }, db.Tran);
            return rows;
        }

        /*07-Sep-2021*/
        /*UCF*/
        /*TSI_MAXI_013*/
        /*Realiza una busqueda en la tabla de cheques con las columnas CheckNumber, RoutingNumber y Account, hace un count con el resultado, si es mayor a 0, retorna true, de lo contrario returna false*/
        public dynamic GetCheckByMircData(String ChNum, String RoutNum, String AccNum) 
        {
            dynamic result = db.Conn.Query(
                "EXEC dbo.sp_CC_GetCheckByMircData @ChNum, @RoutNum, @AccNum",
                new { ChNum, RoutNum, AccNum },
                db.Tran);
            return result;
        }
    }
}
