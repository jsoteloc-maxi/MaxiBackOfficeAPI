using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.Mappings;
using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.Agent.Application.Contracts;

namespace Maxi.BackOffice.Agent.Application.Services
{
    public class AgCustFeesService : CustomServiceBase, IAgCustFeesService
    {

        public AgCustFeesService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public List<AgCustFeesModel> GetAll(int agent)
        {
            using(var context = CreateUnitOfWork())
            {
                return AgentFeeMapper.Map(context.Repositories.CC_AgFeesRepository.GetByAgent(agent));
            }
        }

        public AgCustFeesModel GetById(int id)
        {
            using (var context = CreateUnitOfWork())
            {
                return AgentFeeMapper.Map(context.Repositories.CC_AgFeesRepository.GetById(id));
            }
        }

        public AgCustFeesModel Insert(AgCustFeesModel row)
        {
            using (var context = CreateUnitOfWork())
            {
                var entity = context.Repositories.CC_AgFeesRepository.Insert(AgentFeeMapper.Map(row));
                row = AgentFeeMapper.Map(entity);
                context.SaveChanges();
                return row;
            }
        }

        public int Update(AgCustFeesModel row)
        {
            using (var context = CreateUnitOfWork())
            {
                var rowCount = context.Repositories.CC_AgFeesRepository.Update(AgentFeeMapper.Map(row));
                context.SaveChanges();
                return rowCount;
            }
        }

        public int Delete(int id)
        {
            using (var context = CreateUnitOfWork())
            {
                var rowCount = context.Repositories.CC_AgFeesRepository.Delete(id);
                context.SaveChanges();
                return rowCount;
            }
        }


    }
}
