using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using SSCMS.Utils;

namespace SSCMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SS CMS";

            var filePath = PathUtils.Combine(Directory.GetCurrentDirectory(), Constants.ConfigFileName);
            var json = FileUtils.ReadText(filePath);
            if (json.Contains(@"""SecurityKey"": """","))
            {
                var securityKey = StringUtils.GetShortGuid(false) + StringUtils.GetShortGuid(false) + StringUtils.GetShortGuid(false);
                FileUtils.WriteText(filePath, json.Replace(@"""SecurityKey"": """",", $@"""SecurityKey"": ""{securityKey}"","));
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("permissions.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("menus.json", optional: true, reloadOnChange: true)
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
                        .UseKestrel((ctx, options) => { options.Limits.MaxRequestBodySize = null; })
                        .UseIIS()
                        .UseStartup<Startup>();
                });
    }
}