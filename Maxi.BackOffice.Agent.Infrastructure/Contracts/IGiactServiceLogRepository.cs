using Maxi.BackOffice.Agent.Domain.Model;

namespace Maxi.BackOffice.Agent.Infrastructure.Contracts
{
    public interface IGiactServiceLogRepository
    {
        GiactResult ValidateCheck(GiactInquiry request);
    }
}
