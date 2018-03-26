using System.IO;
using SiteServer.Utils;

namespace SiteServer.CMS.Plugin
{
    public class PluginDebugger
    {
        private FileSystemWatcher _watcher;

        private PluginDebugger()
        {
            
        }

        public void Run()
        {
            if (_watcher != null) return;

            _watcher = new FileSystemWatcher
            {
                Path = PathUtils.PluginsPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true
            };
            _watcher.Created += Watcher_EventHandler;
            _watcher.Changed += Watcher_EventHandler;
            _watcher.Renamed += Watcher_EventHandler;
            _watcher.EnableRaisingEvents = true;
        }

        private static PluginDebugger _instance;

        public static PluginDebugger Instance => _instance ?? (_instance = new PluginDebugger());

        private void Watcher_EventHandler(object sender, FileSystemEventArgs e)
        {
            var fileName = PathUtils.GetFileNameWithoutExtension(e.FullPath);
            if (!PluginManager.IsExists(fileName)) return;
            if (!e.FullPath.EndsWith(".nuspec") && !e.FullPath.EndsWith(".dll")) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                PluginManager.ClearCache();
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }
}
