using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class CC_AgFeesRepository : ICC_AgFeesRepository
    {
        private readonly UnitOfWorkSqlServerAdapter db;
        // 
        private readonly AppCurrentSessionContext session;
        public CC_AgFeesEntity entity;

        public CC_AgFeesRepository(UnitOfWorkSqlServerAdapter dbAdapter)
        {
            db = dbAdapter;
            session = dbAdapter.SessionCtx;
            entity = new CC_AgFeesEntity();
        }


        public CC_AgFeesEntity Insert(CC_AgFeesEntity row)
        {
            //row.ACF_DateCreated = DateTime.Now;
            row.IdAgCustFee = row.Insert( db.Conn, db.Tran);
            return row;
        }

        
        public int Update(CC_AgFeesEntity row)
        {
            return row.Update(db.Conn, db.Tran);
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
            return entity.Delete(db.Conn, db.Tran);
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
            return entity.GetById(db.Conn, db.Tran);
        }


        public List<CC_AgFeesEntity> GetByAgent(int idAgent)
        {
            return
            entity.GetByFilter("IdAgent = @IdAgent",
                new List<SqlParam>
                {
                    new SqlParam(){ Name="@IdAgent", Value=idAgent}
                },
                db.Conn, db.Tran);
        }

    }
}
