using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SSCMS.Plugins;

namespace SSCMS.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UsePlugins(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetService<ILoggerFactory>()
                .CreateLogger<IApplicationBuilder>();

            foreach (var configure in AssemblyUtils.GetInstances<IConfigure>())
            {
                logger.LogInformation("Executing Configure '{0}'", configure.GetType().FullName);
                configure.Configure(app, app.ApplicationServices);
            }
        }
    }
}