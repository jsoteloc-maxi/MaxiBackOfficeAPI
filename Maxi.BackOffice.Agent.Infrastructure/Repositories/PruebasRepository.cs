using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class PruebasRepository
    {
        private readonly ApplicationContext db;

        public PruebasRepository(ApplicationContext dbAdapter)
        {
            db = dbAdapter;
        }

    }
}
