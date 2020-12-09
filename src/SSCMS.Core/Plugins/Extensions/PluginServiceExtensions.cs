using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SSCMS.Core.Services;
using SSCMS.Plugins;
using SSCMS.Services;

namespace SSCMS.Core.Plugins.Extensions
{
    public static class PluginServiceExtensions
    {
        public static IPluginManager AddPlugins(this IServiceCollection services, IConfiguration configuration, ISettingsManager settingsManager)
        {
            var pluginManager = new PluginManager(configuration, settingsManager);
            pluginManager.Load();
            services.TryAdd(ServiceDescriptor.Singleton<IPluginManager>(pluginManager));

            //foreach (var plugin in pluginManager.Plugins)
            //{
            //    if (plugin.Assembly != null)
            //    {
            //        ConfigureServices(plugin.Assembly, services);
            //    }
            //}

            //var instances = pluginManager.GetExtensions<IPluginConfigureServices>();
            //if (instances != null)
            //{
            //    foreach (var plugin in instances)
            //    {
            //        plugin.ConfigureServices(services);
            //    }
            //}

            return pluginManager;
        }

        public static void AddPluginServices(this IServiceCollection services, IPluginManager pluginManager)
        {
            var instances = pluginManager.GetExtensions<IPluginConfigureServices>();
            if (instances != null)
            {
                foreach (var plugin in instances)
                {
                    try
                    {
                        plugin.ConfigureServices(services);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        //private static void ConfigureServices(Assembly assembly, IServiceCollection services)
        //{
        //    if (assembly == null) return;

        //    //var baseType = typeof(IPlugin);
        //    //var types = assembly.GetTypes()
        //    //    .Where(x => x != baseType && baseType.IsAssignableFrom(x)).ToArray();
        //    //var implementTypes = types.Where(x => !x.IsAbstract && x.IsClass).ToArray();
        //    //var interfaceTypes = types.Where(x => x.IsInterface).ToArray();
        //    //foreach (var implementType in implementTypes)
        //    //{
        //    //    services.AddScoped(baseType, implementType);
        //    //    var interfaceType = interfaceTypes.FirstOrDefault(x => x.IsAssignableFrom(implementType));
        //    //    if (interfaceType != null && interfaceType != baseType)
        //    //    {
        //    //        services.AddScoped(interfaceType, implementType);
        //    //    }
        //    //}

        //    foreach (var type in assembly.GetTypes())
        //    {
        //        if (!typeof(IPluginConfigureServices).IsAssignableFrom(type)) continue;
        //        if (!(Activator.CreateInstance(type) is IPluginConfigureServices plugin)) continue;

        //        plugin.ConfigureServices(services);
        //    }
        //}

    }
}
