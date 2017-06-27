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
        void OnChanged(object sender, FileSystemEventArgs e);
    }
}
