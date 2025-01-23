using Maxi.BackOffice.Agent.Infrastructure.Contracts;

namespace Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces
{
    public interface IUnitOfWorkRepository
    {
        ICheckRepository CheckRepository { get; }
        IGiactServiceLogRepository GiactServiceLogRepository { get; }
        IGlobalAttributesRepository GlobalAttributesRepository { get; }
        ICC_AgFeesRepository CC_AgFeesRepository { get; }
        IOrbographRepository OrboRepository { get; }
        IIrdRepository IrdRepository { get; }
        IPcParamsRepository PcParamsRepository { get; }
        ICheckImagePendingRepository CheckImagePendingRepository { get; }

    }
}
