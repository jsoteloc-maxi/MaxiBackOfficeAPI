using Maxi.BackOffice.Agent.Application.Contracts;
using MaxiBackOfficeAPI.Helpers;
using MaxiBackOfficeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaxiBackOfficeAPI.Controllers
{
    /// <summary>
    /// DEPRECATED, se implementa Argo para obtener el token
    /// </summary>
    //[ApiController]
    //[Route("api/[controller]")]
    //public class LoginController : ControllerBase
    //{
    //    private readonly IApiLoginService _loginService;
    //    private readonly IConfiguration _configuration;

    //    public LoginController(IApiLoginService loginService, IConfiguration configuration)
    //    {
    //        _loginService = loginService;
    //        _configuration = configuration;
    //    }

    //    /// <summary>
    //    /// DEPRECTED
    //    /// </summary>
    //    /// <param name="login"></param>
    //    /// <returns></returns>
    //    //[HttpPost("")]
    //    //[AllowAnonymous]
    //    //public IActionResult Login(ApiLoginRequest login)
    //    //{
    //    //    if (login == null)
    //    //        return Unauthorized("Usuario y Contraseña requeridos.");

    //    //    if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
    //    //        return Unauthorized("Usuario y Contraseña requeridos.");

    //    //    var _token = TokenGenerator.GenerateTokenJwt(new UserInfo()
    //    //    {
    //    //        Nombre = login.UserName,
    //    //        Apellidos = login.UserName,
    //    //        Email = "",
    //    //        Rol = "",
    //    //        IdUser = login.IdUser,
    //    //        FrontSessionGuid = login.SessionGuid
    //    //    }, _configuration);

    //    //    return Ok(new { token = _token });
    //    //}
    //}
}
