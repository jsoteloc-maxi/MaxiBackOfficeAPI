using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class CC_AgFeesRepository : ICC_AgFeesRepository
    {
        private readonly IAplicationContext _dbContext;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;
        public CC_AgFeesEntity entity;

        public CC_AgFeesRepository(IAplicationContext dbContext, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _dbContext = dbContext;
            _appCurrentSessionContext = appCurrentSessionContext;
            entity = new CC_AgFeesEntity();
        }


        public CC_AgFeesEntity Insert(CC_AgFeesEntity row)
        {
            //row.ACF_DateCreated = DateTime.Now;
            row.IdAgCustFee = row.Insert( _dbContext.GetConnection(), _dbContext.GetTransaction());
            _dbContext.SaveChanges();
            return row;
        }

        
        public int Update(CC_AgFeesEntity row)
        {
            var result = row.Update(_dbContext.GetConnection(), _dbContext.GetTransaction());
            _dbContext.SaveChanges();
            return result;
        }
        
        /*
        public int Delete(CC_AgFeesEntity row)
        {
            return row.Delete(connection, transaction);
        }
        */
        public int Delete(int id)
        {
            entity.IdAgCustFee = id;
            var result = entity.Delete(_dbContext.GetConnection(), _dbContext.GetTransaction());
            _dbContext.SaveChanges();
            return result;
        }

        /*
        public CC_AgFeesEntity GetById(CC_AgFeesEntity row)
        {
            return row.GetById(connection, transaction);
        }
        */
        public CC_AgFeesEntity GetById(int id)
        {
            entity.IdAgCustFee = id;
            return entity.GetById(_dbContext.GetConnection(), _dbContext.GetTransaction());
        }


        public List<CC_AgFeesEntity> GetByAgent(int idAgent)
        {
            return
            entity.GetByFilter("IdAgent = @IdAgent",
                new List<SqlParam>
                {
                    new SqlParam(){ Name="@IdAgent", Value=idAgent}
                },
                _dbContext.GetConnection(), _dbContext.GetTransaction());
        }

    }
}
