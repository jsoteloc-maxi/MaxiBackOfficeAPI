using Maxi.BackOffice.CrossCutting.Common.Attributes;
using Microsoft.Data.SqlClient;
using System;
using System.Drawing;

namespace Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces
{
    public interface IAplicationContext : IDisposable
    {
        SqlConnection GetConnection();
        SqlTransaction GetTransaction();
        void SaveChanges();
        string LangResource(string key, string def = "");
        string GlobalAttr(string name);
        List<T> GetEntityByFilter<T>(string filter, object param = null) where T : class, IEntityType, new();
        T GetEntityById<T>(int id) where T : class, IEntityType, new();
    }
}
