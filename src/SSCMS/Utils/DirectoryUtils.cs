using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SSCMS.Utils
{
    /// <summary>
    /// 封装操作文件夹代码的类
    /// </summary>
    public static class DirectoryUtils
    {
        public const string Plugins = "plugins";
        public const string Packages = "packages";
        public const string BinDirectoryName = "bin";

        public static class PublishmentSytem
        {
            public const string Include = "Include";
            public const string Template = "Template";
            public const string Content = "Content";
        }

        public static class SiteFiles
        {
            public const string DirectoryName = "sitefiles";
            public const string Library = "library";
            public const string BackupFiles = "backupfiles";
            public const string TemporaryFiles = "temporaryfiles";
            
            public const string Home = "home";
            public const string Administrators = "administrators";
            public const string Users = "users";

            public static class SiteTemplates
            {
                public const string DirectoryName = "sitetemplates";
                //文件夹
                public const string SiteTemplateMetadata = "SiteTemplateMetadata";      //存储频道模板元数据的文件夹名称
                public const string SiteContent = "SiteContent";                        //频道内容导入导出临时文件夹名
                public const string Table = "Table";                                    //辅助表导入导出临时文件夹名
                public const string RelatedField = "RelatedField";                      //关联字段导入导出临时文件夹名

                //文件
                public const string FileTemplate = "Template.xml";                      //序列化模板的文件名
                public const string FileMetadata = "Metadata.xml";                      //频道模板元数据文件
                public const string FileConfiguration = "Configuration.xml";            //站点配置
            }
        }

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
                    // ignored
                }
            }
        }

        public static void Copy(string sourcePath, string targetPath)
        {
            Copy(sourcePath, targetPath, true);
        }

        public static void Copy(string sourcePath, string targetPath, bool isOverride)
        {
            if (!Directory.Exists(sourcePath)) return;

            CreateDirectoryIfNotExists(targetPath);
            var directoryInfo = new DirectoryInfo(sourcePath);
            foreach (var fileSystemInfo in directoryInfo.GetFileSystemInfos())
            {
                var destPath = Path.Combine(targetPath, fileSystemInfo.Name);
                if (fileSystemInfo is FileInfo)
                {
                    FileUtils.CopyFile(fileSystemInfo.FullName, destPath, isOverride);
                }
                else if (fileSystemInfo is DirectoryInfo)
                {
                    Copy(fileSystemInfo.FullName, destPath, isOverride);
                }
            }
        }

        /// <summary>
        /// 验证此字符串是否合作作为文件夹名称
        /// </summary>
        public static bool IsDirectoryNameCompliant(string directoryName)
        {
            if (string.IsNullOrEmpty(directoryName)) return false;
            return -1 == directoryName.IndexOfAny(PathUtils.InvalidPathChars);
        }

        /// <summary>
        /// 获取文件的文件夹路径，如果path为文件夹，返回自身。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string GetDirectoryPath(string path)
        {
            var ext = Path.GetExtension(path);
            var directoryPath = !string.IsNullOrEmpty(ext) ? Path.GetDirectoryName(path) : path;
            return directoryPath;
        }


        public static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static bool IsInDirectory(string parentDirectoryPath, string path)
        {
            if (string.IsNullOrEmpty(parentDirectoryPath) || string.IsNullOrEmpty(path)) return false;

            parentDirectoryPath = StringUtils.ToLower(parentDirectoryPath.Trim().TrimEnd(Path.DirectorySeparatorChar));
            path = StringUtils.ToLower(path.Trim().TrimEnd(Path.DirectorySeparatorChar));

            return parentDirectoryPath == path || path.StartsWith(parentDirectoryPath);
        }

        public static void MoveDirectory(string srcDirectoryPath, string destDirectoryPath, bool isOverride)
        {
            //如果提供的路径中不存在末尾分隔符，则添加末尾分隔符。
            if (!srcDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                srcDirectoryPath += Path.DirectorySeparatorChar;
            }
            if (!destDirectoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                destDirectoryPath += Path.DirectorySeparatorChar;
            }

            //如果目标目录不存在，则予以创建。 
            CreateDirectoryIfNotExists(destDirectoryPath);

            //从当前父目录中获取目录列表。 
            foreach (var srcDir in GetDirectoryPaths(srcDirectoryPath))
            {
                var directoryName = PathUtils.GetDirectoryName(srcDir, false);

                var destDir = destDirectoryPath + directoryName;
                //如果该目录不存在，则创建该目录。 
                CreateDirectoryIfNotExists(destDir);
                //由于我们处于递归模式下，因此还要复制子目录
                MoveDirectory(srcDir, destDir, isOverride);
            }

            //从当前父目录中获取文件。
            foreach (var srcFile in GetFilePaths(srcDirectoryPath))
            {
                var srcFileInfo = new FileInfo(srcFile);
                var destFileInfo = new FileInfo(srcFile.Replace(srcDirectoryPath, destDirectoryPath));
                //如果文件不存在，则进行复制。 
                var isExists = destFileInfo.Exists;
                if (isOverride)
                {
                    if (isExists)
                    {
                        FileUtils.DeleteFileIfExists(destFileInfo.FullName);
                    }
                    FileUtils.CopyFile(srcFileInfo.FullName, destFileInfo.FullName);
                }
                else if (!isExists)
                {
                    FileUtils.CopyFile(srcFileInfo.FullName, destFileInfo.FullName);
                }
            }
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

        public static List<string> GetDirectoryNames(string directoryPath)
        {
            var directoryPaths = GetDirectoryPaths(directoryPath);
            return directoryPaths.Select(directory => new DirectoryInfo(directory))
                .Select(directoryInfo => directoryInfo.Name).ToList();
        }

        public static List<string> GetFileNames(string directoryPath)
        {
            var filePaths = GetFilePaths(directoryPath);
            return filePaths.Select(filePath => new FileInfo(filePath)).Select(fileInfo => fileInfo.Name).ToList();
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns>删除过程中是否出错</returns>
        public static bool DeleteDirectoryIfExists(string directoryPath)
        {
            var retVal = true;
            try
            {
                if (IsDirectoryExists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch
            {
                retVal = false;
            }
            return retVal;
        }

        public static void DeleteEmptyDirectory(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            if (directoryInfo.Exists)
            {
                if (directoryInfo.GetFileSystemInfos().Length == 0)
                {
                    try
                    {
                        DeleteDirectoryIfExists(directoryPath);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        public static string[] GetDirectoryPaths(string directoryPath)
        {
            CreateDirectoryIfNotExists(directoryPath);
            return Directory.GetDirectories(directoryPath);
        }

        public static string[] GetFilePaths(string directoryPath)
        {
            CreateDirectoryIfNotExists(directoryPath);
            return Directory.GetFiles(directoryPath);
        }
    }
}
