using System;
using System.Reflection;
using System.Runtime.Loader;

namespace SSCMS.Core.Plugins
{
    //https://github.com/JeringTech/Javascript.NodeJS
    //https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
    //https://github.com/epic2001/AspNetCorePlugins
    //https://github.com/lamondlu/Mystique/issues
    internal class PluginLoadContext : AssemblyLoadContext
    {

        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
        }
    }
}
