using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.Agent.Domain.Model;


namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface IAgCustFeesService : ICustomServiceBase
    {
        List<AgCustFeesModel> GetAll(int agent);
        AgCustFeesModel GetById(int id);
        AgCustFeesModel Insert(AgCustFeesModel row);
        int Update(AgCustFeesModel row);
        int Delete(int id);
    }
}
