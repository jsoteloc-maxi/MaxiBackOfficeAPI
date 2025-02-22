
using Maxi.BackOffice.Agent.Application.Contracts;
using Maxi.BackOffice.Agent.Application.Services;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Repositories;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Common;
using MaxiBackOfficeAPI.ExceptionHandler;
using MaxiBackOfficeAPI.MessageHandler;
using MaxiBackOfficeAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Text;


namespace MaxiBackOfficeAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info("Initialize API");
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                #region [   Configuración de servicios  ]
                // AutomaticAuthentication = false para desactivar la autenticación de ISS/Active Directory
                // builder.Services.Configure<IISServerOptions>(options => options.AutomaticAuthentication = false);

                // Habilita el soporte a controladores
                builder.Services.AddControllers(opts =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        // deshabilita autenticación para todos los controladores
                        opts.Filters.Add<AllowAnonymousFilter>();
                    }
                    else
                    {
                        // habilita autenticación para todos los controladores
                        var authenticatedUserPolicy = new AuthorizationPolicyBuilder()
                                  .RequireAuthenticatedUser()
                                  .Build();
                        opts.Filters.Add(new AuthorizeFilter(authenticatedUserPolicy));
                    }
                });
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Evita que las propiedades se conviertan a camelCase
                //});

                // Agrega authenticación configurada para JwtBearer
                var jwtSettings = builder.Configuration.GetSection("JWT");
                var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

                builder.Services.AddAuthorization();
                // Habilita el soporte a Swagger/OpenAPI en los servicios
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Maxi.BackOffice.API", Version = "v1" });
                });
                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
                builder.Services.AddProblemDetails();

                // Registrar el TokenValidatorHandler como un servicio
                builder.Services.AddTransient<TokenValidatorHandler>();
                // Registrar el LogRequestResponseHandler como un servicio
                builder.Services.AddTransient<LogRequestResponseHandler>();

                /// Configuración DbContext en un EF Core
                /// Agregar DbContext con la conexión desde appsettings.json
                // builder.Services.AddDbContext<ApplicationDbContext>(options =>
                //    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                builder.Services.AddScoped<IAplicationContext, ApplicationContext>();

                // Sesión context para obtener los datos de los headers en cada request
                builder.Services.AddScoped<IAppCurrentSessionContext, AppCurrentSessionContext>();

                // Inyección de repositorios
                builder.Services.AddScoped<ICheckRepository, CheckRepository>();
                builder.Services.AddScoped<IGiactServiceLogRepository, GiactServiceLogRepository>();
                builder.Services.AddScoped<IGlobalAttributesRepository, GlobalAttributesRepository>();
                builder.Services.AddScoped<ICC_AgFeesRepository, CC_AgFeesRepository>();
                builder.Services.AddScoped<IOrbographRepository, OrbographRepository>();
                //builder.Services.AddScoped<IIrdRepository, IrdRepository>();
                builder.Services.AddScoped<IPcParamsRepository, PcParamsRepository>();
                builder.Services.AddScoped<ICheckImagePendingRepository, CheckImagePendingRepository>();

                // Inyección de servicios
                builder.Services.AddScoped<IRpcService, RpcService>();
                builder.Services.AddScoped<IAgCustFeesService, AgCustFeesService>();
                builder.Services.AddScoped<ICustomerService, CustomerService>();
                #endregion

                builder.Logging.ClearProviders();

                builder.Host.UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false });

                #region [   Configuración de aplicación     ]
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                if (app.Environment.IsProduction())
                {
                    app.UseHttpsRedirection(); // Redirige HTTP a HTTPS
                    app.UseExceptionHandler("/error");
                }

                app.UseExceptionHandler();
                app.UseAuthentication(); // Habilita autenticación (JWT, OAuth, etc.)
                app.UseAuthorization(); // Habilita autorización (roles, claims, políticas)
                // app.UseCors("DefaultPolicy"); // Habilita CORS si está configurado}
                // app.UseRouting(); // Habilita el enrutamiento de las solicitudes HTTP
                // Configura el middleware para session context desde los headers
                //app.UseMiddleware<SessionContextMiddleware>();
                // Configurar el middleware para enrutamiento
                app.MapControllers();
                #endregion

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Program has stopped because there was an exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}
