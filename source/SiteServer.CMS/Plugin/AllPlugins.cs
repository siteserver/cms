using System.Collections.Generic;
using System.Linq;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin
{
    internal static class AllPlugins
    {
        private static readonly SortedList<string, PluginPair> PluginSortedList = new SortedList<string, PluginPair>();

        private static readonly object AllPluginsLock = new object();

        public static void Remove(string pluginId)
        {
            lock (AllPluginsLock)
            {
                PluginSortedList.Remove(pluginId);
            }
        }

        public static void Set(string pluginId, PluginPair pair)
        {
            lock (AllPluginsLock)
            {
                if (PluginSortedList.ContainsKey(pluginId))
                {
                    PluginSortedList[pluginId] = pair;
                }
                else
                {
                    PluginSortedList.Add(pluginId, pair);
                }
            }
        }

        public static bool IsExists(string pluginId)
        {
            lock (AllPluginsLock)
            {
                return PluginSortedList.ContainsKey(pluginId);
            }
        }

        public static PluginMetadata GetMetadata(string pluginId)
        {
            lock (AllPluginsLock)
            {
                PluginPair pair;
                if (PluginSortedList.TryGetValue(pluginId, out pair))
                {
                    return pair.Metadata.Copy();
                }
                return null;
            }
        }

        public static void SetMetadata(PluginMetadata metadata)
        {
            lock (AllPluginsLock)
            {
                PluginPair pair;
                if (PluginSortedList.TryGetValue(metadata.Id, out pair))
                {
                    pair.Metadata = metadata;
                }
            }
        }

        public static List<PluginPair> AllPluginPairs
        {
            get
            {
                // Use the same synchronization that prevents concurrent modifications
                lock (AllPluginsLock)
                {
                    return
                        PluginSortedList.Values.Where(pluginPair => pluginPair?.Metadata != null && pluginPair.Plugin != null)
                            .ToList();
                }
            }
        }

        public static List<PluginPair> GetEnabledPluginPairs<T>() where T : IPlugin
        {
            lock (AllPluginsLock)
            {
                return
                    PluginSortedList.Values.Where(
                        pluginPair =>
                            pluginPair?.Metadata != null && pluginPair.Plugin != null && !pluginPair.Metadata.Disabled &&
                            pluginPair.Plugin is T).ToList();
            }
        }

        public static List<PluginPair> GetEnabledPluginPairs<T1, T2>()
        {
            lock (AllPluginsLock)
            {
                return
                    PluginSortedList.Values.Where(
                        pluginPair =>
                            pluginPair?.Metadata != null && pluginPair.Plugin != null && !pluginPair.Metadata.Disabled &&
                            (pluginPair.Plugin is T1 || pluginPair.Plugin is T2)).ToList();
            }
        }

        public static T GetHook<T>(string pluginId) where T : IPlugin
        {
            lock (AllPluginsLock)
            {
                PluginPair pair;
                var isGet = PluginSortedList.TryGetValue(pluginId, out pair);
                if (isGet && pair != null && !pair.Metadata.Disabled && pair.Plugin is T) return (T)pair.Plugin;
                return default(T);
            }
        }

        public static List<T> GetHooks<T>() where T : IPlugin
        {
            lock (AllPluginsLock)
            {
                var pairs = PluginSortedList.Values.Where(
                        pluginPair =>
                            pluginPair?.Metadata != null && pluginPair.Plugin != null && !pluginPair.Metadata.Disabled &&
                            pluginPair.Plugin is T).ToList();
                return pairs.Select(pluginPair => (T) pluginPair.Plugin).ToList();
            }
        }
    }
}
