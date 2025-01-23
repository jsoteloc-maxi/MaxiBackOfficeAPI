using System.ComponentModel;
using Microsoft.Data.SqlClient;
using Dapper;
using Maxi.BackOffice.CrossCutting.Common.Attributes;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.Configurations;
using Maxi.BackOffice.CrossCutting.UnitOfWork.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.Common;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using System.Text;

namespace Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer
{
    public class UnitOfWorkSqlServerAdapter : IUnitOfWorkAdapter
    {
        private readonly SqlConnection _conn;
        private readonly SqlTransaction _tran;
        private readonly AppCurrentSessionContext _seCtx;
        private IUnitOfWorkRepository _repos;
        private SqlKata.Execution.QueryFactory _QueryFactory;
        private LangResourceLocator _langRes;

        public IUnitOfWorkRepository Repositories => _repos;


        public SqlConnection Conn => _conn;
        public SqlTransaction Tran => _tran;
        public AppCurrentSessionContext SessionCtx => _seCtx;


        public UnitOfWorkSqlServerAdapter(dynamic seCtx)
        {
            _seCtx = seCtx;

            _conn = new SqlConnection(AppSettings.ConnectionStrings(AppSettings.ConexionOperaciones));
            _conn.Open();
            _tran = _conn.BeginTransaction();

            _QueryFactory = new SqlKata.Execution.QueryFactory(_conn, new SqlKata.Compilers.SqlServerCompiler());

            _repos = new UnitOfWorkSqlServerRepository(this);
        }


        public void Dispose()
        {
            if (_tran != null)
                _tran.Dispose();

            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }
            _repos = null;
        }


        public void SaveChanges()
        {
            if (_tran != null)
                _tran.Commit();
        }


        public SqlKata.Query Query(string table)
        {
            return _QueryFactory.Query(table);
        }


        public string LangResource(string key, string def = "")
        {
            if (_langRes == null)
                _langRes = new LangResourceLocator(this);

            return _langRes.GetMessage(key, SessionCtx.IdLang, def);
        }

        public string GlobalAttr(string name)
        {
            var value = _conn.ExecuteScalar<string>("SELECT [Value] FROM dbo.GlobalAttributes WITH(NOLOCK) WHERE [Name] = @name", new { name }, _tran);
            return value ?? string.Empty;
        }


        private void _SetEntityKeyValue(object instance, object value)
        {
            int keyCount = 0;
            var t = instance.GetType();
            var props = TypeDescriptor.GetProperties(t);

            foreach (PropertyDescriptor prop in props)
            {
                var attributes = prop.Attributes;
                foreach (var attr in attributes)
                {
                    if (attr is PropEntityAtributes)
                    {
                        var d = attr as PropEntityAtributes;
                        if (d.Key == true)
                        {
                            keyCount++;
                            prop.SetValue(instance, value);
                        }
                    }
                }
            }

            if (keyCount == 0)
                throw new Exception("NO KEY PROPERTY IN ENTITY " + t.Name);

            if (keyCount > 1)
                throw new Exception($"ENTITY [{t.Name}] WITH MULTICOLUMN KEY");
        }


        private EntityAtributes _GetEntityAttributes(Type tipo)
        {
            EntityAtributes result = null;
            System.Attribute[] AtributosEntidad = System.Attribute.GetCustomAttributes(tipo);
            foreach (System.Attribute attr in AtributosEntidad)
            {
                if (attr is EntityAtributes)
                {
                    result = (EntityAtributes)attr;
                    break;
                }
            }
            return result;
        }


        public T GetEntityById<T>(int id) where T : class, IEntityType, new()
        {
            var x = new T();
            _SetEntityKeyValue(x, id);
            return x.GetById(Conn, Tran);
        }

        public List<T> GetEntityByFilter<T>(string filter, object param=null) where T : class, IEntityType, new()
        {
            var attr = _GetEntityAttributes(typeof(T));
            if (string.IsNullOrWhiteSpace(attr.Schema)) attr.Schema = "dbo";

            var sql = new StringBuilder(100);
            sql.Append($" SELECT * FROM {attr.Schema}.[{attr.Tablename}] WITH(NOLOCK) ");

            //Todo: _Resolve Select fields from entity properties validating against cached table metadata

            if (!string.IsNullOrWhiteSpace(filter))
            {
                sql.Append(" WHERE ");
                sql.Append(filter);
            }

            var res = Conn.Query<T>(sql.ToString().Trim(), param, Tran).ToList();
            return res;
        
            //var parList = new List<SqlParam>();
            //parList.Add(new SqlParam { });
            //return x.GetByFilter(filter, parList, Conn, Tran);
        }

        /*
         UpdateById ( entity, campos,  id)

         UpdateById ( entity, "campo1, campo2, campo3 ",  id)

         UpdateById ( entity, new{ entity.campo1, entity.campo2 },  id)

         UpdateById ( entity, new{ entity.campo1, entity.campo2 },  "filtro", new{ })

        */


        //public static TValue GetAttributeValue<TAttribute, TValue>(Type type, Func<TAttribute, TValue> valueSelector)
        //{
        //    var att = (TAttribute)type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();

        //    if (att != null)
        //    {
        //        return valueSelector(att);
        //    }
        //    return default(TValue);
        //}

    }
}
