using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.CrossCutting.Common.Interfaces
{
    public interface ITableRepositoryBase<T> where T : class
    {
        T Insert(T row);

        int Update(T row);

        //int Delete(T row);
        int Delete(int id);

        //T GetById(T row);
        T GetById(int id);
    }
}
