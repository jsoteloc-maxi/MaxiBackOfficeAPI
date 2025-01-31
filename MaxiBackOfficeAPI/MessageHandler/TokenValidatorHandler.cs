using Maxi.BackOffice.Agent.Infrastructure.Common;
using MaxiBackOfficeAPI.Helpers;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MaxiBackOfficeAPI.MessageHandler
{
    internal class TokenValidatorHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            dynamic _err = null;
            bool _TokenExpired = false;
            HttpStatusCode statusCode;

            if (!TryRetrieveToken(request, out string token))
            {
                statusCode = HttpStatusCode.Unauthorized;
                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                var jwtCfg = Utils.GetJwtConfig();
                var claveSecreta = jwtCfg.SecretKey;
                var issuer = jwtCfg.Issuer;
                var audience = jwtCfg.Audience;


                var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(claveSecreta));
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = audience,
                    ValidIssuer = issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // DELEGADO PERSONALIZADO PERA COMPROBAR
                    // LA CADUCIDAD EL TOKEN.
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey
                };

                // COMPRUEBA LA VALIDEZ DEL TOKEN
                SecurityToken securityToken;
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                // TODO HttpContext.Current.User es imcompatible en NET 8
                //HttpContext.Current.User = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                statusCode = HttpStatusCode.Unauthorized;
                _TokenExpired = (ex is SecurityTokenInvalidLifetimeException);
                _err = new JObject();
                _err.Message = ex.Message;
            }
            catch (Exception ex)
            {
                //hay errores que no se logean con el filtro => se pone esto aqui
                GLogger.Info("--------");
                GLogger.Error(ex);
                throw ex;
                //statusCode = HttpStatusCode.InternalServerError;
            }

            var m = new HttpResponseMessage(statusCode);

            if (_TokenExpired)
                m.Headers.Add("TokenExpired", "true");

            if (_err != null)
                m.Content = new StringContent(_err.ToString());

            return Task<HttpResponseMessage>.Factory.StartNew(() => m);
            //return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }


        // RECUPERA EL TOKEN DE LA PETICIÓN
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;

            if (!request.Headers.TryGetValues("Authorization", out IEnumerable<string> authzHeaders) || authzHeaders.Count() > 1)
                return false;

            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }


        // COMPRUEBA LA CADUCIDAD DEL TOKEN
        public bool LifetimeValidator(DateTime? notBefore,
                                      DateTime? expires,
                                      SecurityToken securityToken,
                                      TokenValidationParameters validationParameters)
        {
            //return false; //prueba expirado

            var valid = false;

            if ((expires.HasValue && DateTime.UtcNow < expires) && (notBefore.HasValue && DateTime.UtcNow > notBefore))
            {
                valid = true;
            }

            return valid;
        }



    }
}
