using Maxi.BackOffice.Agent.Application.Contracts;
using MaxiBackOfficeAPI.Helpers;
using MaxiBackOfficeAPI.Middleware;
using MaxiBackOfficeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiBackOfficeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IApiLoginService _loginService;

        public LoginController(IApiLoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("")]
        public IActionResult Login(ApiLoginRequest login)
        {
            if (login == null)
                return BadRequest("Usuario y Contraseña requeridos.");

            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
                return BadRequest("Usuario y Contraseña requeridos.");

            // TODO verificar el username
            var appCurrentSessionContext = HttpContext.Items["appCurrentSessionContext"] as AppCurrentSessionContext;

            UserInfo _userInfo = _loginService.AutenticateSessionData(
                new UserInfo { IdUser = login.IdUser, FrontSessionGuid = login.SessionGuid }, appCurrentSessionContext.UserName, appCurrentSessionContext.UserName);
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
