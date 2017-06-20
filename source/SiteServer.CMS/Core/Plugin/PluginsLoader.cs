using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BaiRong.Core;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core.Plugin
{
    public static class PluginsLoader
    {
        public static List<PluginPair> Plugins(List<PluginMetadata> metadatas)
        {
            var plugins = new List<PluginPair>();

            foreach (var metadata in metadatas)
            {
                var s = Stopwatch.StartNew();

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
                catch (Exception e)
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

                plugins.Add(new PluginPair(metadata, plugin));

                var milliseconds = s.ElapsedMilliseconds;

                metadata.InitTime += milliseconds;

            }
            return plugins;
        }

    }
}