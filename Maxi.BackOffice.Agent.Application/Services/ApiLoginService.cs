using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Maxi.BackOffice.Agent.Application.Services
{
    public class ApiLoginService : CustomServiceBase, IApiLoginService
    {
        public ApiLoginService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        dynamic IApiLoginService.AutenticateSessionData(dynamic r, string userName, string lastName)
        {
            //usara los datos de session obtenidos desde, ...

            using (var context = this.CreateUnitOfWork())
            {
                // Id del Usuario en el Sistema de Información (BD)
                //r.IdUser = SessionCtx.IdUser;
                r.Nombre = userName;
                r.Apellidos = lastName;
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
