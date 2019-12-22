using DickinsonBros.AccountAPI.Infrastructure.AccountDB;
using DickinsonBros.AccountAPI.Infrastructure.AccountDB.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using DickinsonBros.AccountAPI.Infrastructure.Logging;
using DickinsonBros.AccountAPI.Infrastructure.Redactor;
using DickinsonBros.AccountAPI.Infrastructure.DateTime;
using DickinsonBros.AccountAPI.Infrastructure.Encryption;
using DickinsonBros.AccountAPI.Infrastructure.Middleware;
using System.Net.Mail;
using System;
using System.Net;
using DickinsonBros.AccountAPI.Logic.Account;
using DickinsonBros.AccountAPI.Infrastructure.EmailSender;
using DickinsonBros.AccountAPI.Infrastructure.Sql;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using DickinsonBros.AccountAPI.Infrastructure.Encryption.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using DickinsonBros.AccountAPI.Infrastructure.PasswordEncryption;
using DickinsonBros.AccountAPI.Infrastructure.JWT.Models;
using DickinsonBros.AccountAPI.Infrastructure.JWT;

namespace DickinsonBros.AccountAPI.View
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressMapClientErrors = true;
            });
            services.AddOptions();

            services.Configure<DickinsonBrosDB>(Configuration.GetSection("DickinsonBrosDB"));
            services.Configure<JsonRedactorOptions>(Configuration.GetSection("JsonRedactorOptions"));
            services.Configure<EncryptionSettings>(Configuration.GetSection("EncryptionSettings"));
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));


            services.AddSingleton<IAccountDBService, AccountDBService>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
            services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();
            services.AddSingleton<IJWTService, JWTService>();
            services.AddSingleton<IDateTimeService, DateTimeService>();
            services.AddSingleton<IAccountManager, AccountManager>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped<ICorrelationService, CorrelationService>();
            services.AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>));
            services.AddSingleton<IRedactor, Redactor>();
            services.AddSingleton<IGuidService, GuidService>();
            services.AddSingleton<IMiddleware, Middleware>();
            services.AddSingleton<ISQLService, SQLService>();

            services.AddScoped<SmtpClient>((serviceProvider) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                return new SmtpClient()
                {
                    Host = config.GetValue<String>("Email:Smtp:Host"),
                    Port = config.GetValue<int>("Email:Smtp:Port"),
                    Credentials = new NetworkCredential(
                            config.GetValue<String>("Email:Smtp:Username"),
                            config.GetValue<String>("Email:Smtp:Password")
                        )
                };
            });

            var JWTSettings = Configuration.GetSection("JWTSettings").Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(JWTSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                       TokenValidationParameters validationParameters) =>
                    {
                        return notBefore <= DateTime.UtcNow &&
                               expires >= DateTime.UtcNow;
                    }
                };
            });

            services.AddApiVersioning();
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Account API", Version = "1"});
                options.DocInclusionPredicate((version, apiDescription) =>
                {
                    var apiVersionAttribute =
                        (ApiVersionAttribute)
                        apiDescription.ActionDescriptor
                                      .EndpointMetadata
                                      .FirstOrDefault
                                      (
                                        metaData => metaData.GetType().Equals(typeof(ApiVersionAttribute))
                                      );

                    if (apiVersionAttribute != null && apiVersionAttribute.Versions.Any(e => "v" + e.MajorVersion == version))
                    {
                        ReplaceVersionDescriptions(apiDescription, version);
                        return true;
                    }

                    return false;
                });
            });

          
        }
        private static void ReplaceVersionDescriptions(ApiDescription apiDescription, string version)
        {
            apiDescription.RelativePath = apiDescription.RelativePath.Replace("/v{version}", $"/{version}");
            var versionParameter =
                apiDescription.ParameterDescriptions.SingleOrDefault(p => p.Name == "version");

            if (versionParameter != null)
            {
                apiDescription.ParameterDescriptions.Remove(versionParameter);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseMiddleware<Middleware>();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void DepcytConfig()
        {
            //


            //


            //


            //


            //
        }
    }
}