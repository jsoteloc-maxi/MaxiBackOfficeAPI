using MaxiBackOfficeAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MaxiBackOfficeAPI.Helpers
{
    internal static class TokenGenerator
    {
        public static string GenerateTokenJwt(UserInfo usuarioInfo, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("Jwt");
            string secretKey = jwtConfig["Key"];
            string issuerToken = jwtConfig["Issuer"];
            string audienceToken = jwtConfig["Audience"];
            int expires = Convert.ToInt32(jwtConfig["Expires"]);

            var symmetricSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, usuarioInfo.IdUser.ToString()),
                new Claim("nombre", usuarioInfo.Nombre),
                new Claim("apellidos", usuarioInfo.Apellidos),
                new Claim("frontguid", usuarioInfo.FrontSessionGuid),
                new Claim(JwtRegisteredClaimNames.Email, usuarioInfo.Email),
                new Claim(ClaimTypes.Role, usuarioInfo.Rol)
            };

            var payload = new JwtPayload(
                    issuer: issuerToken,
                    audience: audienceToken,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    // Exipra a la 24 horas.
                    expires: DateTime.UtcNow.AddHours(expires)
                );

            // GENERAMOS EL TOKEN //
            var token = new JwtSecurityToken(
                    header,
                    payload
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
