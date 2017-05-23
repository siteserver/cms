using System;
using System.Collections.Generic;
using BaiRong.Core;

namespace SiteServer.CMS.Plugins
{
    public class PluginManager
    {
        private static List<PluginInfo> _pluginInfoList;

        public static List<PluginInfo> GetPluginInfoList()
        {
            if (_pluginInfoList != null) return _pluginInfoList;

            _pluginInfoList = new List<PluginInfo>();
            var pluginsPath = PathUtils.MapPath("~/SiteFiles/Plugins");
            if (!DirectoryUtils.IsDirectoryExists(pluginsPath)) return new List<PluginInfo>();

            foreach (var directoryName in DirectoryUtils.GetDirectoryNames(pluginsPath))
            {
                if (!StringUtils.StartsWithIgnoreCase(directoryName, "Plugin.")) continue;

                var directoryPath = PathUtils.Combine(pluginsPath, directoryName);
                var dllPath = PathUtils.Combine(directoryPath, "bin", directoryName + ".dll");
                if (!FileUtils.IsFileExists(dllPath)) continue;

                foreach (var fileName in DirectoryUtils.GetFileNames(PathUtils.Combine(directoryPath, "bin")))
                {
                    if (StringUtils.EndsWithIgnoreCase(fileName, ".dll"))
                    {
                        var srcDllPath = PathUtils.Combine(directoryPath, "bin", fileName);
                        var descDllPath = PathUtils.MapPath("~/bin/" + fileName);
                        var isOverride = StringUtils.EqualsIgnoreCase(fileName, directoryName + ".dll");
                        FileUtils.CopyFile(srcDllPath, descDllPath, isOverride);
                    }
                }

                var assembly = System.Reflection.Assembly.LoadFrom(PathUtils.Combine(PathUtils.MapPath("~/bin/"), directoryName + ".dll"));
                var type = assembly.GetType(directoryName + ".Plugin", false, true);
                if (type == null) continue;

                if (!typeof(PluginBase).IsAssignableFrom(type)) continue;

                var plugin = (PluginBase)Activator.CreateInstance(type);
                if (plugin == null) continue;

                var pluginInfo = new PluginInfo(directoryName, directoryPath, plugin);

                _pluginInfoList.Add(pluginInfo);
            }

            return _pluginInfoList;
        }
    }
}
