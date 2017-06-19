using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BaiRong.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin
{
    public static class PluginsLoader
    {
        public static List<PluginPair> Plugins(List<PluginMetadata> metadatas)
        {
            var csharpPlugins = CSharpPlugins(metadatas).ToList();
            var plugins = csharpPlugins.ToList();
            return plugins;
        }

        public static IEnumerable<PluginPair> CSharpPlugins(List<PluginMetadata> metadatas)
        {
            var plugins = new List<PluginPair>();

            foreach (var metadata in metadatas)
            {
                var s = Stopwatch.StartNew();


#if DEBUG
                var assembly = Assembly.Load(AssemblyName.GetAssemblyName(metadata.ExecuteFilePath));
                var types = assembly.GetTypes();
                var type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
                var plugin = (IPlugin)Activator.CreateInstance(type);
#else
                Assembly assembly;
                try
                {
                    assembly = Assembly.Load(AssemblyName.GetAssemblyName(metadata.ExecuteFilePath));
                }
                catch (Exception e)
                {
                    LogUtils.AddErrorLog(e, $"Couldn't load assembly for {metadata.Name}");
                    continue;
                }
                var types = assembly.GetTypes();
                Type type;
                try
                {
                    type = types.First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
                }
                catch (InvalidOperationException e)
                {
                    LogUtils.AddErrorLog(e, $"Can't find class implement IPlugin for <{metadata.Name}>");
                    continue;
                }
                IPlugin plugin;
                try
                {
                    plugin = (IPlugin)Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    LogUtils.AddErrorLog(e, $"Can't create instance for <{metadata.Name}>");
                    continue;
                }
#endif
                plugins.Add(new PluginPair(metadata, plugin));

                var milliseconds = s.ElapsedMilliseconds;

                metadata.InitTime += milliseconds;

            }
            return plugins;
        }

    }
}