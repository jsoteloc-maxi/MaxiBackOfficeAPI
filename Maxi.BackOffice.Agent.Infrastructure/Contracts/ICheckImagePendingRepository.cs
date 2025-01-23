using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.CrossCutting.Common.Interfaces;

namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{
    public interface ICheckImagePendingRepository : ITableRepositoryBase<CheckImagePendingEntity>
    {
        int ChecImageProcessed(int id);

        void UploadImage(List<UploadFileDto> uploadChecks);
    }
}
