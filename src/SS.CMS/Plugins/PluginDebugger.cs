using System.IO;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Plugins
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
                Path = WebUtils.PluginsPath,
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
            if (!PluginManager.IsExistsAsync(fileName).GetAwaiter().GetResult()) return;
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
