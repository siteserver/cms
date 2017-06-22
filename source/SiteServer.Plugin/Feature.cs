using System.IO;

namespace SiteServer.Plugin
{
    public interface IFeatures { }

    public interface IRestful : IFeatures
    {
        object Get(IRequestContext context);
        object Post(IRequestContext context);
        object Put(IRequestContext context);
        object Delete(IRequestContext context);
        object Patch(IRequestContext context);
    }

    public interface IFileSystemWatcher : IFeatures
    {
        /// <summary>
        /// Fired after file changed events
        /// if you want to hook something when file changed, you should use this event
        /// </summary>
        void OnFileChanged(object sender, FileSystemEventArgs e);
    }
}
