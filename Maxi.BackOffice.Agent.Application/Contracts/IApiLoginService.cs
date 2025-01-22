using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.CrossCutting.Common.Common;


namespace Maxi.BackOffice.Agent.Application.Contracts
{
    public interface IApiLoginService : ICustomServiceBase
    {
        dynamic AutenticateSessionData(dynamic r);
    }
}
