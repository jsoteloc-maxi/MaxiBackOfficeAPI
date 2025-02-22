using Dapper;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Common;


namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class PcParamsRepository : IPcParamsRepository
    {
        private readonly IAplicationContext _dbContext;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;

        public PcParamsRepository(IAplicationContext dbContext, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _dbContext = dbContext;
            _appCurrentSessionContext = appCurrentSessionContext;
        }

        public string GetParam(string ident, string col)
        {
            string sql, val="";

            sql = $" SELECT PC.[{col}] FROM dbo.PcIdentifier PC WITH(NOLOCK) ";
            //sql += " JOIN dbo.AgentPc AP ON AP.IdPcIdentifier = PC.IdPcIdentifier  ";
            sql += " WHERE PC.Identifier = @ident  ";
            //sql += " AND AP.IdAgent = @IdAgent  ";
            sql += " ORDER BY PC.IdPcIdentifier  ";

            var r =  _dbContext.GetConnection().ExecuteScalar(sql, new { ident, _appCurrentSessionContext.IdAgent }, _dbContext.GetTransaction());
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

            _dbContext.GetConnection().Execute(sql, new { ident, _appCurrentSessionContext.IdAgent, value }, _dbContext.GetTransaction());
            return 0;
        }
    }
}
