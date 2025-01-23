using Dapper;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Common;


namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class PcParamsRepository : IPcParamsRepository
    {
        private readonly UnitOfWorkSqlServerAdapter db;
        private readonly AppCurrentSessionContext session;


        public PcParamsRepository(UnitOfWorkSqlServerAdapter ctx)
        {
            this.db = ctx;
            this.session = db.SessionCtx;

        }


        public string GetParam(string ident, string col)
        {
            string sql, val="";

            sql = $" SELECT PC.[{col}] FROM dbo.PcIdentifier PC WITH(NOLOCK) ";
            //sql += " JOIN dbo.AgentPc AP ON AP.IdPcIdentifier = PC.IdPcIdentifier  ";
            sql += " WHERE PC.Identifier = @ident  ";
            //sql += " AND AP.IdAgent = @IdAgent  ";
            sql += " ORDER BY PC.IdPcIdentifier  ";

            var r =  db.Conn.ExecuteScalar(sql, new { ident, session.IdAgent }, db.Tran);
            if (r != null) val = r.ToString();
            return val;
        }


        public int SetParam(string ident, string col, string value)
        {
            string[] a = { "ScannerType" };
            if (!Array.Exists(a, element => element.ToUpper().Trim() == col.ToUpper().Trim()))
                throw new Exception("PC Parameter not vaild for update");

            string sql;
            sql = $" UPDATE PC SET PC.[{col}] = @value ";
            sql += " FROM dbo.PcIdentifier PC WITH(NOLOCK) ";
            //sql += " JOIN dbo.AgentPc AP ON AP.IdPcIdentifier = PC.IdPcIdentifier  ";
            sql += " WHERE PC.Identifier = @ident  ";
            //sql += " AND AP.IdAgent = @IdAgent  ";

            db.Conn.Execute(sql, new { ident, session.IdAgent, value }, db.Tran);
            return 0;
        }



    }
}
