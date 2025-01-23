using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    class GlobalAttributesRepository : IGlobalAttributesRepository
    {
        UnitOfWorkSqlServerAdapter db;
        private readonly AppCurrentSessionContext session;

        public GlobalAttributesRepository(UnitOfWorkSqlServerAdapter uwAdapter)
        {
            this.db = uwAdapter;
            this.session = uwAdapter.SessionCtx;
        }

        public string GetValue(string aName)
        {
            var entity = new GlobalAttributesEntity();

            /* GetScalar no funciona pk trata de regresar un objeto T pero debe regresar solo un valor simple
            string r =
            entity.GetScalar(" SELECT Value FROM dbo.[GlobalAttributes] WITH(NOLOCK) WHERE [Name] = @Name ",
                new List<SqlParam>
                {
                    new SqlParam() { Name = "@Name" , Value = aName }
                }, connection, transaction);

            if (r == null) return "";
            return r;
            */
            
            var r = entity.GetByFilter("Name = @Name",
                new List<SqlParam>
                {
                    new SqlParam() { Name = "@Name" , Value = aName }
                }, db.Conn, db.Tran);

            if (r.Count == 0) return "";
            return r.First().Value;
        }

    }
}
