using Maxi.BackOffice.Agent.Domain.Model;


namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface IAgCustFeesService
    {
        List<AgCustFeesModel> GetAll(int agent);
        AgCustFeesModel GetById(int id);
        AgCustFeesModel Insert(AgCustFeesModel row);
        int Update(AgCustFeesModel row);
        int Delete(int id);
    }
}
