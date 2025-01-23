namespace Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces
{
    public interface IUnitOfWork
    {
        IUnitOfWorkAdapter Create(dynamic seCtx);
    }
}
