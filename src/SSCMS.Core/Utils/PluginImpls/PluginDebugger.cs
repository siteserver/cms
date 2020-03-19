using System.IO;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.PluginImpls
{
    public class PluginDebugger
    {
        private static FileSystemWatcher _watcher;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;

        public PluginDebugger(IPluginManager pluginManager, IPathManager pathManager)
        {
            _pluginManager = pluginManager;
            _pathManager = pathManager;
        }

        public void Run()
        {
            if (_watcher != null) return;

            _watcher = new FileSystemWatcher
            {
                Path = _pathManager.PluginsPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true
            };
            _watcher.Created += Watcher_EventHandler;
            _watcher.Changed += Watcher_EventHandler;
            _watcher.Renamed += Watcher_EventHandler;
            _watcher.EnableRaisingEvents = true;
        }

        private void Watcher_EventHandler(object sender, FileSystemEventArgs e)
        {
            var fileName = PathUtils.GetFileNameWithoutExtension(e.FullPath);
            if (!_pluginManager.IsExists(fileName)) return;
            if (!e.FullPath.EndsWith(".nuspec") && !e.FullPath.EndsWith(".dll")) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }
}
