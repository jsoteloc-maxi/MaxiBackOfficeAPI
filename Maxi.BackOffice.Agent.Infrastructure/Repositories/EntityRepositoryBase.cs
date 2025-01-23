using Microsoft.Data.SqlClient;
using Maxi.BackOffice.CrossCutting.Common.Attributes;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    //Todo: ver si se puede crear repositorio base para entidades de tabla estandar con operaciones crud
    public abstract class EntityRepositoryBase<T> where T : IEntityType
    {
        public readonly SqlConnection dbConn;
        public readonly SqlTransaction dbTran;

        public EntityRepositoryBase(SqlConnection aConnection, SqlTransaction aTransaction)
        {
            dbConn = aConnection;
            dbTran = aTransaction;
        }

        public int Insert(T row)
        {
            return row.Insert(dbConn, dbTran);
        }

        public int Update(T row)
        {
            return row.Update(dbConn, dbTran);
        }

        public int Delete(T row)
        {
            return row.Delete(dbConn, dbTran);
        }

        public void GetById(T row)
        {
            //row.GetScalar()
            //row.GetById(connection, transaction);
        }
    }
}
