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
    }
}
