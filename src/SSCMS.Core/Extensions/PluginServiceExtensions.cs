using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SSCMS.Core.Plugins;
using SSCMS.Core.Services;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Extensions
{
    public static class PluginServiceExtensions
    {
        public static IPluginManager AddPlugins(this IServiceCollection services, ISettingsManager settingsManager)
        {
            var pluginManager = new PluginManager(settingsManager);
            services.TryAdd(ServiceDescriptor.Singleton<IPluginManager>(pluginManager));

            foreach (var folderPath in Directory.GetDirectories(pluginManager.DirectoryPath))
            {
                if (string.IsNullOrEmpty(folderPath)) continue;
                var folderName = Path.GetFileName(folderPath);
                if (string.IsNullOrEmpty(folderName) || StringUtils.StartsWith(folderName, ".")) continue;
                var configPath = PathUtils.Combine(folderPath, Constants.PluginPackageFileName);
                if (!FileUtils.IsFileExists(configPath)) continue;

                Assembly assembly = null;
                var assemblyPath = PathUtils.Combine(folderPath, $"{folderName}.dll");
                if (FileUtils.IsFileExists(assemblyPath))
                {
                    assembly = LoadAssembly(assemblyPath);
                    ConfigureServices(assembly, services);
                }
                
                pluginManager.Add(new PluginMetadata(folderName, folderPath, assembly));
            }
            return pluginManager;
        }

        private static Assembly LoadAssembly(string assemblyPath)
        {
            var loadContext = new PluginLoadContext(assemblyPath);
            var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));

            var dllPath = Path.GetDirectoryName(assemblyPath);

            var assemblyFiles = Directory.GetFiles(dllPath, "*.dll", SearchOption.AllDirectories);
            foreach (var assemblyFile in assemblyFiles)
            {
                Assembly.LoadFile(assemblyFile);
                if (AssemblyLoadContext.Default.Assemblies.All(a => !StringUtils.EqualsIgnoreCase(Path.GetFileName(a.Location), Path.GetFileName(assemblyFile))))
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);
                }
            }

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
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
