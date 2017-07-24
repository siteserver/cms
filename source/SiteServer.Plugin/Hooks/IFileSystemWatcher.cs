using System.IO;

namespace SiteServer.Plugin.Hooks
{
    public interface IFileSystemWatcher : IPlugin
    {
        void OnChanged(object sender, FileSystemEventArgs e);
    }
}
