using System.IO;

namespace SiteServer.Plugin.Hooks
{
    public interface IFileSystemWatcher : IHooks
    {
        void OnChanged(object sender, FileSystemEventArgs e);
    }
}
