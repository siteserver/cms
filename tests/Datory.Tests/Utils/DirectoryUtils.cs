using System.IO;

namespace Datory.Tests.Utils
{
    public static class DirectoryUtils
    {
        public static void CreateDirectoryIfNotExists(string path)
        {
            var directoryPath = GetDirectoryPath(path);

            if (!IsDirectoryExists(directoryPath))
            {
                try
                {
                    Directory.CreateDirectory(directoryPath);
                }
                catch
                {
                    //Scripting.FileSystemObject fso = new Scripting.FileSystemObjectClass();
                    //string[] directoryNames = directoryPath.Split('\\');
                    //string thePath = directoryNames[0];
                    //for (int i = 1; i < directoryNames.Length; i++)
                    //{
                    //    thePath = thePath + "\\" + directoryNames[i];
                    //    if (StringUtils.Contains(thePath.ToLower(), ConfigUtils.Instance.PhysicalApplicationPath.ToLower()) && !IsDirectoryExists(thePath))
                    //    {
                    //        fso.CreateFolder(thePath);
                    //    }
                    //}                    
                }
            }
        }

        private static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }

        public static string GetParentPath(string path, int upLevel = 1)
        {
            if (upLevel < 1) return path;

            for (var i = 0; i < upLevel; i++)
            {
                path = Directory.GetParent(path).FullName;
            }

            return path;
        }

        private static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
    }
}