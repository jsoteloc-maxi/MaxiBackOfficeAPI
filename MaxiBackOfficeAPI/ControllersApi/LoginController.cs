using Maxi.BackOffice.Agent.Application.Contracts;
using MaxiBackOfficeAPI.Models;
using System.Web.Http;

namespace MaxiBackOfficeAPI.ControllersApi
{
    [RoutePrefix("api/Login")]
    public class LoginController : ApiControllerBase
    {
        private readonly IApiLoginService _svc;

        public LoginController(IApiLoginService svc)
        {
            _svc = svc;
            InitService(_svc);
        }



        // POST: api/login
        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public IHttpActionResult Login(ApiLoginRequest login)
        {
            if (login == null)
                return BadRequest("Usuario y Contraseña requeridos.");

            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Usuario y Contraseña requeridos.");

            UserInfo _userInfo = _svc.AutenticateSessionData(new UserInfo { IdUser = login.IdUser, FrontSessionGuid = login.SessionGuid });
            //var _userInfo = AutenticarUsuario(login.UserName, login.Password);

            if (_userInfo != null)
            {
                return Ok(new { token = TokenGenerator.GenerateTokenJwt(_userInfo) });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
