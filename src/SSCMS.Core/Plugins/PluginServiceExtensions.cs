using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SSCMS.Core.Services;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public static class PluginServiceExtensions
    {
        public static IPluginManager AddPlugins(this IServiceCollection services, ISettingsManager settingsManager)
        {
            var pluginManager = new PluginManager(settingsManager);
            services.TryAdd(ServiceDescriptor.Singleton<IPluginManager>(pluginManager));

            foreach (var plugin in pluginManager.Plugins)
            {
                if (plugin.Assembly != null)
                {
                    ConfigureServices(plugin.Assembly, services);
                }
            }

            return pluginManager;
        }

        private static void ConfigureServices(Assembly assembly, IServiceCollection services)
        {
            if (assembly == null) return;

            var baseType = typeof(IOldPlugin);
            var types = assembly.GetTypes()
                .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
            var implementTypes = types.Where(x => !x.IsAbstract && x.IsClass).ToArray();
            var interfaceTypes = types.Where(x => x.IsInterface).ToArray();
            foreach (var implementType in implementTypes)
            {
                services.AddScoped(baseType, implementType);
                var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
                if (interfaceType != null && interfaceType != baseType)
                {
                    services.AddScoped(interfaceType, implementType);
                }
            }

            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IPluginConfigureServices).IsAssignableFrom(type)) continue;
                if (!(Activator.CreateInstance(type) is IPluginConfigureServices pluginStartup)) continue;

                pluginStartup.ConfigureServices(services);
            }
        }

    }
}
