using System;
using System.Collections.Generic;
using System.Linq;
using Maxi.BackOffice.Agent.Infrastructure.Mappings;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;

namespace Maxi.BackOffice.Agent.Application.Services
{
    public class AgCustFeesService : IAgCustFeesService
    {
        private readonly ICC_AgFeesRepository _agFeesRepository;
        public AgCustFeesService(ICC_AgFeesRepository agFeesRepository)
        {
            _agFeesRepository = agFeesRepository;
        }

        public List<AgCustFeesModel> GetAll(int agent)
        {
            return AgentFeeMapper.Map(_agFeesRepository.GetByAgent(agent));
        }

        public AgCustFeesModel GetById(int id)
        {
            return AgentFeeMapper.Map(_agFeesRepository.GetById(id));
        }

        public AgCustFeesModel Insert(AgCustFeesModel row)
        {
            var entity = _agFeesRepository.Insert(AgentFeeMapper.Map(row));
            row = AgentFeeMapper.Map(entity);
            //context.SaveChanges(); JISC TODO 
            return row;
        }

        public int Update(AgCustFeesModel row)
        {
            var rowCount = _agFeesRepository.Update(AgentFeeMapper.Map(row));
            //context.SaveChanges(); JISC TODO
            return rowCount;
        }

        public int Delete(int id)
        {
            var rowCount = _agFeesRepository.Delete(id);
            //context.SaveChanges(); JISc TODO
            return rowCount;
        }


    }
}
