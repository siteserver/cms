using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InstallUtils.Init(Directory.GetCurrentDirectory());

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile(Constants.PackageFileName, optional: true, reloadOnChange: true)
                        .AddJsonFile(Constants.ConfigFileName, optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables("SSCMS_")
                        .AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseSerilog((hostingContext, loggerConfiguration) =>
                        {
                            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                            loggerConfiguration.Enrich.FromLogContext();
                        })
                        .UseKestrel(options => { options.Limits.MaxRequestBodySize = long.MaxValue; })
                        .UseIIS()
                        .UseStartup<Startup>();
                });
    }
}