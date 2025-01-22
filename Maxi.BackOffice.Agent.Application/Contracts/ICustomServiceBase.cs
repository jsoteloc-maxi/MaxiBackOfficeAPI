using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface ICustomServiceBase
    {
        void Initialize(dynamic ctx);
    }
}
