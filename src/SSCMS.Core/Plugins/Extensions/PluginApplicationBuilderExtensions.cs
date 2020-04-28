using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins.Extensions
{
    public static class PluginApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePlugins(this IApplicationBuilder app,
            IPluginManager pluginManager)
        {
            var logger = app.ApplicationServices.GetService<ILoggerFactory>()
                .CreateLogger<IApplicationBuilder>();

            var instances = pluginManager.GetExtensions<IPluginConfigure>();
            if (instances != null)
            {
                foreach (var pluginConfigure in instances)
                {
                    pluginConfigure.Configure(app);
                }
            }

            foreach (var plugin in pluginManager.Plugins)
            {
                logger.LogInformation("Using Plugin '{0}'", plugin.PluginId);

                var directoryPath = PathUtils.Combine(pluginManager.DirectoryPath, plugin.FolderName, "wwwroot");
                DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

                var fileProvider = new PhysicalFileProvider(directoryPath);
                app.UseStaticFiles(
                    new StaticFileOptions
                    {
                        FileProvider = fileProvider
                    });
            }

            return app;
        }
    }
}
