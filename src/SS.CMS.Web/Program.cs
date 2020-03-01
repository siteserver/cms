using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SS.CMS.Abstractions;

namespace SS.CMS.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "SS CMS";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(ConfigConfiguration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel((ctx, options) => { options.Limits.MaxRequestBodySize = null; })
                        .UseStartup<Startup>();
                });

        static void ConfigConfiguration(IConfigurationBuilder config)
        {
            config.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Constants.ConfigFileName, optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
