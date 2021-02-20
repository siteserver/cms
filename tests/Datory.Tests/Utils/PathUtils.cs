using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Datory.Tests.Utils
{
    public static class PathUtils
    {
        public static string GetDirectoryName(string path, bool isFile)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            if (isFile)
            {
                path = Path.GetDirectoryName(path);
            }
            if (!string.IsNullOrEmpty(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                return directoryInfo.Name;
            }
            return string.Empty;
        }
    }
}
