using System;
using System.IO;

namespace SiteServer.Plugin.Features
{
    public interface IFileSystem : IPlugin
    {
        Action<object, FileSystemEventArgs> OnFileSystemChanged { get; }
    }
}
