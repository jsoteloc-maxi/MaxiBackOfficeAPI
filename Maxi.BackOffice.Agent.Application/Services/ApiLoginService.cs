using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.CrossCutting.Common.Common;

namespace Maxi.BackOffice.Agent.Application.Services
{
    public class ApiLoginService : CustomServiceBase, IApiLoginService
    {
        public ApiLoginService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        dynamic IApiLoginService.AutenticateSessionData(dynamic r)
        {
            //usara los datos de session obtenidos desde, ...

            using (var context = this.CreateUnitOfWork())
            {
                // Id del Usuario en el Sistema de Información (BD)
                //r.IdUser = SessionCtx.IdUser;
                r.Nombre = SessionCtx.UserName;
                r.Apellidos = SessionCtx.UserName;
                //r.FrontSessionGuid = SessionCtx.SessionGuid;
                r.Email = "";
                r.Rol = "";
                

                //Validar en base de datos algo ?

                //context.Repositories.


                // Retornamos un objeto del tipo UsuarioInfo, con toda
                // la información del usuario necesaria para el Token.
                return r;
                
            }
        }


    }
}
