using System;

namespace Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces
{
    public interface IUnitOfWorkAdapter:IDisposable
    {
        IUnitOfWorkRepository Repositories { get; }

        void SaveChanges();

        string LangResource(string key, string def = "");
        string GlobalAttr(string name);
    }
}
