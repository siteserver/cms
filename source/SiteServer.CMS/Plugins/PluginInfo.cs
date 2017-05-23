using BaiRong.Core;

namespace SiteServer.CMS.Plugins
{
    public class PluginInfo
    {
        public PluginInfo(string directoryName, string directoryPath, PluginBase instance)
        {
            DirectoryName = directoryName;
            DirectoryPath = directoryPath;
            Instance = instance;

            var pluginMenuPath = PathUtils.Combine(directoryPath, "Menus.config");
            MenusPath = !FileUtils.IsFileExists(pluginMenuPath) ? string.Empty : pluginMenuPath;
        }

        public string DirectoryName { get; }
        public string DirectoryPath { get; }

        public string MenusPath { get; }

        public PluginBase Instance { get; }
    }
}
