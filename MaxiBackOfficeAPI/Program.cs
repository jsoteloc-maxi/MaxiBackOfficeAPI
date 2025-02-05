
using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Application.Services;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using MaxiBackOfficeAPI.ExceptionHandler;
using MaxiBackOfficeAPI.MessageHandler;
using MaxiBackOfficeAPI.Middleware;

namespace MaxiBackOfficeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region [   Configuración de servicios  ]
            
            // Registrar el TokenValidatorHandler como un servicio
            builder.Services.AddTransient<TokenValidatorHandler>();
            // Registrar el LogRequestResponseHandler como un servicio
            builder.Services.AddTransient<LogRequestResponseHandler>(); 

            builder.Services.AddSingleton<IUnitOfWork, UnitOfWorkSqlServer>();
            builder.Services.AddSingleton<IApiLoginService, ApiLoginService>();
            builder.Services.AddSingleton<IRpcService, RpcService>();
            builder.Services.AddSingleton<IAgCustFeesService, AgCustFeesService>();
            builder.Services.AddSingleton<ICustomerService, CustomerService>();
            // Habilita el soporte a controladores
            builder.Services.AddControllers();
            // Habilita el soporte a Swagger/OpenAPI en los servicios
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            var app = builder.Build();
            #endregion

            #region [   Configurar la tubería de middleware (equivalente a Configure en Startup.cs NET 5 o inferior)  ]
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            if (app.Environment.IsProduction())
            {
                app.UseHttpsRedirection(); // Solo habilitado en producción
            }

            app.UseExceptionHandler();

            //app.UseAuthentication();
            app.UseAuthorization();
            // Configura el middleware para session context desde los headers
            app.UseMiddleware<HeaderMiddleware>();
            // Configurar el middleware para enrutamiento
            app.MapControllers();
            // TODO: revisar la validación de token
            // Añadir el handler de validación de token a la cadena de MessageHandlers
            //app.UseHttpRequestMiddleware(async (context, next) =>
            //{
            //    var tokenValidatorHandler = context.RequestServices.GetRequiredService<TokenValidatorHandler>();
            //    var response = await tokenValidatorHandler.SendAsync(context.Request, new CancellationToken());
            //    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //    {
            //        // Si el token es inválido, puedes devolver una respuesta personalizada
            //        context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            //        await context.Response.WriteAsync("Token no válido o expirado.");
            //    }
            //    else
            //    {
            //        await next();
            //    }
            //});
            #endregion

            app.Run();
        }
    }
}
