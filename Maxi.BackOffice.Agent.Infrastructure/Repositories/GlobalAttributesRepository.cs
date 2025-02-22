using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class GlobalAttributesRepository : IGlobalAttributesRepository
    {
        private readonly IAplicationContext _dbContext;
        
        public GlobalAttributesRepository(IAplicationContext dbContext)
        {
            _dbContext = dbContext;
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
                }, _dbContext.GetConnection(), _dbContext.GetTransaction());

            if (r.Count == 0) return "";
            return r.First().Value;
        }
    }
}
