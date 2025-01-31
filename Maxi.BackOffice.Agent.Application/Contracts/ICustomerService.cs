using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.Agent.Domain.Model;

namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface ICustomerService : ICustomServiceBase
    {
        Byte[] GetImage(int idCustomer, string imageType);
    }
}
