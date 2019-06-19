using System.IO;

namespace SS.CMS.Utils.IO
{
    public static class FileSystemUtils
    {
        public static FileSystemInfoExtendCollection GetFileSystemInfoExtendCollection(string rootPath, bool reflesh)
        {
            FileSystemInfoExtendCollection retval = null;
            if (Directory.Exists(rootPath))
            {
                var currentRoot = new DirectoryInfo(rootPath);
                var files = currentRoot.GetFileSystemInfos();
                var fsies = new FileSystemInfoExtendCollection(files);
            }

            return retval;
        }
    }
}
