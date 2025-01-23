using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Repositories;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;

namespace Maxi.BackOffice.CrossCutting.UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServerRepository :IUnitOfWorkRepository
    {
        private readonly UnitOfWorkSqlServerAdapter dbAdapter;

        //Constructor
        public UnitOfWorkSqlServerRepository(UnitOfWorkSqlServerAdapter uwAdapter)
        {
            this.dbAdapter = uwAdapter;
        }


        ICheckRepository _CheckRepo;
        public ICheckRepository CheckRepository =>
            _CheckRepo ?? (_CheckRepo = new CheckRepository(dbAdapter));


        IGiactServiceLogRepository _GiactRepo;
        public IGiactServiceLogRepository GiactServiceLogRepository =>
            _GiactRepo ?? (_GiactRepo = new GiactServiceLogRepository(dbAdapter));


        IGlobalAttributesRepository _GlobalAttributesRepo;
        public IGlobalAttributesRepository GlobalAttributesRepository =>
            _GlobalAttributesRepo ?? (_GlobalAttributesRepo = new GlobalAttributesRepository(dbAdapter));


        ICC_AgFeesRepository _AgFeesRepo;
        public ICC_AgFeesRepository CC_AgFeesRepository =>
            _AgFeesRepo ?? (_AgFeesRepo = new CC_AgFeesRepository(dbAdapter));


        IOrbographRepository _OrboRepo;
        public IOrbographRepository OrboRepository =>
            _OrboRepo ?? (_OrboRepo = new OrbographRepository(dbAdapter));


        IIrdRepository _IrdRepo;
        public IIrdRepository IrdRepository =>
            _IrdRepo ?? (_IrdRepo = new IrdRepository(dbAdapter));





        IPcParamsRepository _PcParamsRepo;
        public IPcParamsRepository PcParamsRepository =>
            _PcParamsRepo ?? (_PcParamsRepo = new PcParamsRepository(dbAdapter));

        ICheckImagePendingRepository _CheckImagePendingRepository;
        public ICheckImagePendingRepository CheckImagePendingRepository => _CheckImagePendingRepository ?? (_CheckImagePendingRepository = new CheckImagePendingRepository(dbAdapter));


    }
}
