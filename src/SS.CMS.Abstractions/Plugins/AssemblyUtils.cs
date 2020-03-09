using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SS.CMS.Abstractions.Plugins
{
    public static class AssemblyUtils
    {
        private static IEnumerable<Assembly> _assemblies;
        private static ConcurrentDictionary<Type, IEnumerable<Type>> _types;

        public static string GetPluginId(Type type)
        {
            var assemblyName = type.Assembly.GetName();
            return assemblyName.Name;
        }

        public static string GetPluginName(Type type)
        {
            var name = GetPluginId(type);
            return StringUtils.Contains(name, ".") ? name.Substring(name.LastIndexOf('.') + 1) : name;
        }

        public static string GetPluginVersion(Type type)
        {
            var assemblyName = type.Assembly.GetName();
            if (assemblyName.Version == null)
            {
                return "1.0.0";
            }

            return StringUtils.TrimEnd(assemblyName.Version.ToString(), ".0");
        }

        /// <summary>
        /// Gets the cached assemblies that have been set by the SetAssemblies method.
        /// </summary>
        public static IEnumerable<Assembly> Assemblies => _assemblies;

        /// <summary>
        /// Sets the assemblies and invalidates the type cache.
        /// </summary>
        /// <param name="assemblies">The assemblies to set.</param>
        public static void SetAssemblies(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
            _types = new ConcurrentDictionary<Type, IEnumerable<Type>>();
        }

        /// <summary>
        /// Gets the first implementation of the type specified by the type parameter, or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the same type(s) is requested.
        /// </param>
        /// <returns>The first found implementation of the given type.</returns>
        public static Type GetImplementation<T>(bool useCaching = false)
        {
            return GetImplementation<T>(null, useCaching);
        }

        /// <summary>
        /// Gets the first implementation of the type specified by the type parameter and located in the assemblies
        /// filtered by the predicate, or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the same type(s) is requested.
        /// </param>
        /// <returns>The first found implementation of the given type.</returns>
        public static Type GetImplementation<T>(Func<Assembly, bool> predicate, bool useCaching = false)
        {
            return GetImplementations<T>(predicate, useCaching).FirstOrDefault();
        }

        /// <summary>
        /// Gets the implementations of the type specified by the type parameter.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the same type(s) is requested.
        /// </param>
        /// <returns>Found implementations of the given type.</returns>
        public static IEnumerable<Type> GetImplementations<T>(bool useCaching = false)
        {
            return GetImplementations<T>(null, useCaching);
        }

        /// <summary>
        /// Gets the implementations of the type specified by the type parameter and located in the assemblies
        /// filtered by the predicate.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the same type(s) is requested.
        /// </param>
        /// <returns>Found implementations of the given type.</returns>
        public static IEnumerable<Type> GetImplementations<T>(Func<Assembly, bool> predicate, bool useCaching = false)
        {
            Type type = typeof(T);

            if (useCaching && _types.ContainsKey(type))
                return _types[type];

            List<Type> implementations = new List<Type>();

            foreach (Assembly assembly in GetAssemblies(predicate))
                foreach (Type exportedType in assembly.GetExportedTypes())
                    if (type.GetTypeInfo().IsAssignableFrom(exportedType) && exportedType.GetTypeInfo().IsClass)
                        implementations.Add(exportedType);

            if (useCaching)
                _types[type] = implementations;

            return implementations;
        }

        /// <summary>
        /// Gets the new instance of the first implementation of the type specified by the type parameter,
        /// or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(bool useCaching = false)
        {
            return GetInstance<T>(null, useCaching, new object[] { });
        }

        /// <summary>
        /// Gets the new instance (using constructor that matches the arguments) of the first implementation
        /// of the type specified by the type parameter or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructor.</param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(bool useCaching = false, params object[] args)
        {
            return GetInstance<T>(null, useCaching, args);
        }

        /// <summary>
        /// Gets the new instance of the first implementation of the type specified by the type parameter
        /// and located in the assemblies filtered by the predicate or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(Func<Assembly, bool> predicate, bool useCaching = false)
        {
            return GetInstances<T>(predicate, useCaching).FirstOrDefault();
        }

        /// <summary>
        /// Gets the new instance (using constructor that matches the arguments) of the first implementation
        /// of the type specified by the type parameter and located in the assemblies filtered by the predicate
        /// or null if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementation of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructor.</param>
        /// <returns>The instance of the first found implementation of the given type.</returns>
        public static T GetInstance<T>(Func<Assembly, bool> predicate, bool useCaching = false, params object[] args)
        {
            return GetInstances<T>(predicate, useCaching, args).FirstOrDefault();
        }

        /// <summary>
        /// Gets the new instances of the implementations of the type specified by the type parameter
        /// or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(bool useCaching = false)
        {
            return GetInstances<T>(null, useCaching, new object[] { });
        }

        /// <summary>
        /// Gets the new instances (using constructor that matches the arguments) of the implementations
        /// of the type specified by the type parameter or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructors.</param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(bool useCaching = false, params object[] args)
        {
            return GetInstances<T>(null, useCaching, args);
        }

        /// <summary>
        /// Gets the new instances of the implementations of the type specified by the type parameter
        /// and located in the assemblies filtered by the predicate or empty enumeration
        /// if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(Func<Assembly, bool> predicate, bool useCaching = false)
        {
            return GetInstances<T>(predicate, useCaching, new object[] { });
        }

        /// <summary>
        /// Gets the new instances (using constructor that matches the arguments) of the implementations
        /// of the type specified by the type parameter and located in the assemblies filtered by the predicate
        /// or empty enumeration if no implementations found.
        /// </summary>
        /// <typeparam name="T">The type parameter to find implementations of.</typeparam>
        /// <param name="predicate">The predicate to filter the assemblies.</param>
        /// <param name="useCaching">
        /// Determines whether the type cache should be used to avoid assemblies scanning next time,
        /// when the instance(s) of the same type(s) is requested.
        /// </param>
        /// <param name="args">The arguments to be passed to the constructors.</param>
        /// <returns>The instances of the found implementations of the given type.</returns>
        public static IEnumerable<T> GetInstances<T>(Func<Assembly, bool> predicate, bool useCaching = false,
            params object[] args)
        {
            List<T> instances = new List<T>();

            foreach (Type implementation in GetImplementations<T>(predicate, useCaching))
            {
                if (!implementation.GetTypeInfo().IsAbstract)
                {
                    T instance = (T)Activator.CreateInstance(implementation, args);

                    instances.Add(instance);
                }
            }

            return instances;
        }

        private static IEnumerable<Assembly> GetAssemblies(Func<Assembly, bool> predicate)
        {
            if (predicate == null)
                return Assemblies;

            return Assemblies.Where(predicate);
        }
    }
}
