using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Application.Contracts;


namespace Maxi.BackOffice.Agent.Application.Services
{
    public class CustomerService : CustomServiceBase, ICustomerService
    {
        public CustomerService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        
        public Byte[] GetImage(int idCustomer, string imageType)
        {
            using (var context = CreateUnitOfWork())
            {

                throw new NotImplementedException();
            }
        }


    }
}
