using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Application.Contracts;


namespace Maxi.BackOffice.Agent.Application
{
    //base para las clases service
    public class CustomServiceBase : ICustomServiceBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public AppCurrentSessionContext SessionCtx { get; private set; }


        public CustomServiceBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize(dynamic ctx)
        {
            SessionCtx = ctx;
        }

        protected IUnitOfWorkAdapter CreateUnitOfWork()
        {
            return _unitOfWork.Create(SessionCtx);
        }

    }
}
