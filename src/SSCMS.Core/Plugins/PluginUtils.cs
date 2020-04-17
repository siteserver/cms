using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public static class PluginUtils
    {
        private static ConcurrentDictionary<Type, IEnumerable<Type>> _types;

        static PluginUtils()
        {
            _types = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }

        public static string GetPluginId(IPlugin plugin)
        {
            return $"{plugin.Publisher ?? plugin.FolderName}.{plugin.Name ?? plugin.FolderName}".ToLower();
        }

        public static Assembly LoadAssembly(string assemblyPath)
        {
            //var loadContext = new PluginLoadContext(assemblyPath);
            //var assembly = loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath)));

            var dllPath = Path.GetDirectoryName(assemblyPath);

            var assemblyFiles = Directory.GetFiles(dllPath, "*.dll", SearchOption.AllDirectories);
            foreach (var assemblyFile in assemblyFiles)
            {
                //Assembly.LoadFile(assemblyFile);
                if (AssemblyLoadContext.Default.Assemblies.All(a => !StringUtils.EqualsIgnoreCase(Path.GetFileName(a.Location), Path.GetFileName(assemblyFile))))
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFile);
                }
            }

            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
        }

        /// <summary>
        /// Gets the first implementation of the type specified by the type parameter and located in the assemblies
        /// filtered by the assemblies, or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="assemblies">The assemblies to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the same type(s) is requested.
        /// </param>
        /// <returns>The first found implementation of the given type.</returns>
        public static Type GetImplementation<T>(IEnumerable<Assembly> assemblies, bool useCaching = false)
        {
            return GetImplementations<T>(assemblies, useCaching).FirstOrDefault();
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
        public static IEnumerable<Type> GetImplementations<T>(IEnumerable<Assembly> assemblies, bool useCaching = false)
        {
            var type = typeof(T);

            if (useCaching && _types.ContainsKey(type))
                return _types[type];

            var implementations = new List<Type>();

            foreach (var assembly in assemblies)
            {
                foreach (var exportedType in assembly.GetExportedTypes())
                {
                    if (type.GetTypeInfo().IsAssignableFrom(exportedType) && exportedType.GetTypeInfo().IsClass)
                    {
                        implementations.Add(exportedType);
                    }
                }
            }

            if (useCaching)
                _types[type] = implementations;

            return implementations;
        }

        /// <summary>
        /// Gets the new instance of the first implementation of the type specified by the type parameter
        /// and located in the assemblies filtered by the assemblies or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="assemblies">The assemblies to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(IEnumerable<Assembly> assemblies, bool useCaching = false)
        {
            return GetInstances<T>(assemblies, useCaching).FirstOrDefault();
        }

        /// <summary>
        /// Gets the new instance (using constructor that matches the arguments) of the first implementation
        /// of the type specified by the type parameter and located in the assemblies filtered by the assemblies
        /// or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="assemblies">The assemblies to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructor.</param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(IEnumerable<Assembly> assemblies, bool useCaching = false, params object[] args)
        {
            return GetInstances<T>(assemblies, useCaching, args).FirstOrDefault();
        }

        /// <summary>
        /// Gets the new instances of the implementations of the type specified by the type parameter
        /// and located in the assemblies filtered by the assemblies or empty enumeration
        /// if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="assemblies">The assemblies to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(IEnumerable<Assembly> assemblies, bool useCaching = false)
        {
            return GetInstances<T>(assemblies, useCaching, new object[] { });
        }

        /// <summary>
        /// Gets the new instances (using constructor that matches the arguments) of the implementations
        /// of the type specified by the type parameter and located in the assemblies filtered by the assemblies
        /// or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="assemblies">The assemblies to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructors.</param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(IEnumerable<Assembly> assemblies, bool useCaching = false,
            params object[] args)
        {
            var instances = new List<T>();

            foreach (var implementation in GetImplementations<T>(assemblies, useCaching))
            {
                if (!implementation.GetTypeInfo().IsAbstract)
                {
                    var instance = (T)Activator.CreateInstance(implementation, args);

                    instances.Add(instance);
                }
            }

            return instances;
        }
    }
}
