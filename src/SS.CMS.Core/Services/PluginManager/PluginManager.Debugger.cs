using System.IO;
using SS.CMS.Core.Common;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PluginManager
    {
        private FileSystemWatcher _watcher;

        public void RunDebugger()
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
            if (!IsExists(fileName)) return;
            if (!e.FullPath.EndsWith(".nuspec") && !e.FullPath.EndsWith(".dll")) return;

            try
            {
                _watcher.EnableRaisingEvents = false;
                ClearCache();
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }
}
