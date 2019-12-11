using System;
using System.Diagnostics.CodeAnalysis;
using DickinsonBros.AccountAPI.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DickinsonBros.AccountAPI.View
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggingService<Program>? logger = null;
            try
            {
                var hostBuilder = CreateHostBuilder(args);
                var host = hostBuilder.Build();
                logger = (ILoggingService<Program>)host.Services.GetService(typeof(ILoggingService<Program>));
                logger.LogInformationRedacted("Account API - Starting");

                host.Run();
            }
            catch(Exception exception)
            {
                if(logger != null)
                {
                    logger.LogErrorRedacted($"Error In Main of {typeof(Program).Namespace}.{nameof(Program)}", exception);
                }
               
            }
            if (logger != null)
            {
                logger.LogInformationRedacted("Account API - Closing");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
            });
    }
}
