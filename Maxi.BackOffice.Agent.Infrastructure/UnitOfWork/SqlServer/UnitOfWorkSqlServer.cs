using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;

namespace Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServer : IUnitOfWork
    {
        public UnitOfWorkSqlServer()
        {
            //
        }

        public IUnitOfWorkAdapter Create(dynamic seCtx)
        {
            return new UnitOfWorkSqlServerAdapter(seCtx);
        }
    }
}
