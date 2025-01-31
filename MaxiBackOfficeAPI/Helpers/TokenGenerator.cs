using MaxiBackOfficeAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MaxiBackOfficeAPI.Helpers
{
    internal static class TokenGenerator
    {
        // GENERAMOS EL TOKEN CON LA INFORMACIÓN DEL USUARIO
        public static string GenerateTokenJwt(UserInfo usuarioInfo)
        {
            // RECUPERAMOS LAS VARIABLES DE CONFIGURACIÓN
            var jwtConfig = Utils.GetJwtConfig(); // TODO mejora la implementación de Utils 
            string secretKey = jwtConfig.SecretKey;
            string issuerToken = jwtConfig.Issuer;
            string audienceToken = jwtConfig.Audience;
            int _Expires = jwtConfig.Expires;


            // CREAMOS EL HEADER //
            var _symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var _signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var _Header = new JwtHeader(_signingCredentials);

            // CREAMOS LOS CLAIMS //
            var _claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, usuarioInfo.IdUser.ToString()),
                new Claim("nombre", usuarioInfo.Nombre),
                new Claim("apellidos", usuarioInfo.Apellidos),
                new Claim("frontguid", usuarioInfo.FrontSessionGuid),
                new Claim(JwtRegisteredClaimNames.Email, usuarioInfo.Email),
                new Claim(ClaimTypes.Role, usuarioInfo.Rol)
            };

            // CREAMOS EL PAYLOAD //
            var _payload = new JwtPayload(
                    issuer: issuerToken,
                    audience: audienceToken,
                    claims: _claims,
                    notBefore: DateTime.UtcNow,
                    // Exipra a la 24 horas.
                    expires: DateTime.UtcNow.AddHours(_Expires)
                );

            // GENERAMOS EL TOKEN //
            var _Token = new JwtSecurityToken(
                    _Header,
                    _payload
                );

            return new JwtSecurityTokenHandler().WriteToken(_Token);
        }
    }
}
