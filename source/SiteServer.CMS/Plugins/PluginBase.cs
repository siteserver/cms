using System.IO;

namespace SiteServer.CMS.Plugins
{
    public abstract class PluginBase
    {
        public abstract string Name { get; }
        public abstract string PluginUrl { get; }
        public abstract string Description { get; }
        public abstract string Version { get; }
        public abstract string Author { get; }
        public abstract string AuthorUrl { get; }

        public virtual void Initialize()
        {
            
        }

        public virtual void Dispose()
        {
            
        }

        public event FileSystemEventHandler FileChanged;

        public void OnFileChanged(FileSystemEventArgs e)
        {
            FileChanged?.Invoke(this, e);
        }
    }
}
