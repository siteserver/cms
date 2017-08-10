using System.IO;

namespace SiteServer.Plugin.Features
{
    public interface IFileSystemWatcher : IPlugin
    {
        void OnChanged(object sender, FileSystemEventArgs e);
    }
}
