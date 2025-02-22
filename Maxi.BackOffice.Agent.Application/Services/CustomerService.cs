using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.CrossCutting.Common.Common;


namespace Maxi.BackOffice.Agent.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IAplicationContext _dbContext;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;

        public CustomerService(IAplicationContext dbContext, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _dbContext = dbContext;
            _appCurrentSessionContext = appCurrentSessionContext;
        }

        public Byte[] GetImage(int idCustomer, string imageType)
        {
            throw new NotImplementedException();
        }
    }
}
