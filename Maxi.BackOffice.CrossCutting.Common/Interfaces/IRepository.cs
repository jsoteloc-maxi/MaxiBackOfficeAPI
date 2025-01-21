using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using System.Collections.Generic;

namespace Maxi.BackOffice.CrossCutting.Common.Interfaces
{
    public interface IRepository<T> where T :class
    {

        T Insert(T row);

        void Update(T row);

        void Delete(int id);

        List<T> GetAll();

        T GetById(int id);        
    }
}
