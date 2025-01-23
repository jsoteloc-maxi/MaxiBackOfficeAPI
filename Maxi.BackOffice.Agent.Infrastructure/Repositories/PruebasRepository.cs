using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class PruebasRepository
    {
        private readonly UnitOfWorkSqlServerAdapter db;

        public PruebasRepository(UnitOfWorkSqlServerAdapter dbAdapter)
        {
            db = dbAdapter;
        }

    }
}
