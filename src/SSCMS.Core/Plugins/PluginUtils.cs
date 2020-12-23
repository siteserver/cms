using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using SSCMS.Configuration;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public static class PluginUtils
    {
        private static readonly ConcurrentDictionary<Type, IEnumerable<Type>> Types;

        static PluginUtils()
        {
            Types = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }

        public static Assembly LoadAssembly(string assemblyPath)
        {
            //var loadContext = new PluginLoadContext(assemblyPath);
            //var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));
            //return assembly;

            var assemblyNames = AssemblyLoadContext.Default.Assemblies.Select(x => x.GetName().Name).ToList();

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);

            var isValidAssembly = false;
            var extensionType = typeof(IPluginExtension);
            foreach (var exportedType in assembly.GetExportedTypes())
            {
                if (extensionType.IsAssignableFrom(exportedType) &&
                    exportedType.GetTypeInfo().IsClass)
                {
                    isValidAssembly = true;
                    break;
                }
            }

            if (!isValidAssembly)
            {
                throw new Exception("未找到集成IPluginExtension接口的实现");
            }

            assemblyNames.Add(assembly.GetName().Name);

            var dllDirectoryPath = Path.GetDirectoryName(assemblyPath);
            var assemblyFiles = Directory.GetFiles(dllDirectoryPath, "*.dll", SearchOption.TopDirectoryOnly);
            
            foreach (var assemblyFile in assemblyFiles)
            {
                var assemblyName = Path.GetFileNameWithoutExtension(assemblyFile);

                if (assemblyNames.Any(x => StringUtils.EqualsIgnoreCase(x, assemblyName))) continue;

                AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);

                assemblyNames.Add(assemblyName);
            }

            return assembly;
        }

        /// <summary>
        /// Gets the implementations of the type specified by the type parameter and located in the assemblies
        /// filtered by the assemblies.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="assemblies">The assemblies to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the same type(s) is requested.
        /// </param>
        /// <returns>Found implementations of the given type.</returns>
        private static IEnumerable<Type> GetImplementations<T>(IEnumerable<Assembly> assemblies, bool useCaching = true)
        {
            var type = typeof(T);

            if (useCaching && Types.ContainsKey(type))
                return Types[type];

            var implementations = new List<Type>();

            foreach (var assembly in assemblies)
            {
                if (assembly == null) continue;

                foreach (var exportedType in assembly.GetExportedTypes())
                {
                    if (type.GetTypeInfo().IsAssignableFrom(exportedType) && exportedType.GetTypeInfo().IsClass)
                    {
                        implementations.Add(exportedType);
                    }
                }
            }

            if (useCaching)
                Types[type] = implementations;

            return implementations;
        }

        /// <summary>
        /// Gets the new instances (using constructor that matches the arguments) of the implementations
        /// of the type specified by the type parameter and located in the assemblies filtered by the assemblies
        /// or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="plugins"></param>
        /// <param name="provider"></param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(IEnumerable<IPlugin> plugins, IServiceProvider provider, bool useCaching = true)
        {
            var instances = new List<T>();

            foreach (var implementation in GetImplementations<T>(plugins.Select(x => x.Assembly), useCaching))
            {
                //if (implementation.IsAbstract || implementation.GetConstructor(Type.EmptyTypes) == null) continue;
                //var instance = (T)Activator.CreateInstance(implementation, args);

                if (implementation.IsAbstract) continue;

                var instance = (T)ActivatorUtilities.CreateInstance(provider, implementation);

                instances.Add(instance);
            }

            return instances;
        }

        public static async Task UpdateVersionAsync(string folderPath, string version)
        {
            var packageFile = PathUtils.Combine(folderPath, Constants.PackageFileName);
            var json = await FileUtils.ReadTextAsync(packageFile);
            var obj = JObject.Parse(json);
            foreach (var prop in obj.Properties())
            {
                if (prop.Name == "version")
                {
                    prop.Value = version;
                }
            }

            json = obj.ToString(Formatting.Indented);

            await FileUtils.WriteTextAsync(packageFile, json);
        }

        public static async Task<(Plugin plugin, string errorMessage)> ValidateManifestAsync(string folderPath)
        {
            var packageFile = PathUtils.Combine(folderPath, Constants.PackageFileName);
            if (!FileUtils.IsFileExists(packageFile))
            {
                return (null, "Plugin manifest file 'package.json' not found");
            }

            var json = await FileUtils.ReadTextAsync(packageFile);
            try
            {
                JToken.Parse(json);
            }
            catch
            {
                return (null, "Error parsing 'package.json' manifest file: not a valid JSON file.");
            }

            var plugin = new Plugin(folderPath, false);

            if (string.IsNullOrEmpty(plugin.Name))
            {
                return (null, "Missing plugin name");
            }

            if (!StringUtils.IsStrictName(plugin.Name))
            {
                return (null, $"Invalid plugin name '{plugin.Name}'");
            }

            if (string.IsNullOrEmpty(plugin.Publisher))
            {
                return (null, "Missing plugin publisher");
            }

            if (!StringUtils.IsStrictName(plugin.Publisher))
            {
                return (null, $"Invalid plugin publisher '{plugin.Publisher}'");
            }

            if (string.IsNullOrEmpty(plugin.Version))
            {
                return (null, "Missing plugin version");
            }

            if (!IsSemVersion(plugin.Version))
            {
                return (null, $"Invalid plugin version '{plugin.Version}'");
            }

            if (plugin.Engines == null)
            {
                return (null, "Manifest missing field: engines");
            }

            if (!plugin.Engines.TryGetValue("sscms", out var version))
            {
                return (null, "Manifest missing field: engines.sscms");
            }

            if (string.IsNullOrEmpty(version))
            {
                return (null, "Missing sscms engine compatibility version");
            }

            const string versionRegex = "^\\*$|^(\\^|>=)?((\\d+)|x)\\.((\\d+)|x)\\.((\\d+)|x)(\\-.*)?$";
            if (!RegexUtils.IsMatch(versionRegex, version))
            {
                return (null, $"Invalid sscms engine compatibility version '{version}'");
            }

            return (plugin, null);
        }

        public static string GetPluginId(string publisher, string name)
        {
            return $"{publisher}.{name}";
        }

        public static string GetPackageId(string publisher, string name, string version)
        {
            return $"{publisher}.{name}.{version}";
        }

        public static bool IsSemVersion(string version)
        {
            return !string.IsNullOrEmpty(version) && SemVersion.TryParse(version, out _);
        }
    }
}
