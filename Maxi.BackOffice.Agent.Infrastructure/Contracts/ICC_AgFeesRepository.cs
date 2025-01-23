using Maxi.BackOffice.CrossCutting.Common.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.Entities;

namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{
    public interface ICC_AgFeesRepository : ITableRepositoryBase<CC_AgFeesEntity>
    {
        List<CC_AgFeesEntity> GetByAgent(int idAgent);
    }
}
