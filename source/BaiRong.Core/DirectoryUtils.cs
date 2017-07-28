using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BaiRong.Core
{
    /// <summary>
    /// 封装操作文件夹代码的类
    /// </summary>
    public class DirectoryUtils
    {
        public class AspnetClient
        {
            public const string DirectoryName = "aspnet_client";
        }

        public class Bin
        {
            public const string DirectoryName = "Bin";
        }

        public class Obj
        {
            public const string DirectoryName = "obj";
        }

        public class SiteServer
        {
            public const string DirectoryName = "SiteServer";
        }

        public class Api
        {
            public const string DirectoryName = "Api";
        }

        public class PublishmentSytem
        {
            public const string Include = "Include";
            public const string Template = "Template";
            public const string Content = "Content";
        }

        public class WebConfig
        {
            public const string DirectoryName = "Web.config";
        }

        public class SiteFiles
        {
            public const string DirectoryName = "SiteFiles";

            public const string UserFiles = "UserFiles";
            public const string BackupFiles = "BackupFiles";
            public const string TemporaryFiles = "TemporaryFiles";
            public const string Configuration = "Configuration";
        }

        public class SiteTemplates
        {
            public const string DirectoryName = "SiteTemplates";
            //文件夹
            public const string SiteTemplateMetadata = "SiteTemplateMetadata";//存储频道模板元数据的文件夹名称
            public const string SiteContent = "SiteContent";//频道内容导入导出临时文件夹名
            public const string Input = "Input";//提交表单导入导出临时文件夹名
            public const string Table = "Table";//辅助表导入导出临时文件夹名
            public const string RelatedField = "RelatedField";//关联字段导入导出临时文件夹名
            public const string Photo = "Photo";//相册导入导出临时文件夹名

            //文件
            public const string FileTemplate = "Template.xml";//序列化模板的文件名
            public const string FileDisplayMode = "DisplayMode.xml";//序列化显示方式的文件名
            public const string FileMenuDisplay = "MenuDisplay.xml";//序列化菜单显示方式的文件名
            public const string FileTagStyle = "TagStyle.xml";//序列化模板标签样式的文件名
            public const string FileGatherRule = "GatherRule.xml";//序列化采集规则的文件名
            public const string FileAd = "Ad.xml";//序列化固定广告的文件名
            public const string FileMetadata = "Metadata.xml";//频道模板元数据文件
            public const string FileConfiguration = "Configuration.xml";
            public const string FileSeo = "Seo.xml";
            public const string FileStlTag = "StlTag.xml";
            public const string FileContentModel = "ContentModel.xml";//自定义添加的内容模型
        }

        public static char DirectorySeparatorChar = Path.DirectorySeparatorChar;

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
            string directoryPath;
            var ext = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(ext))		//path为文件路径
            {
                directoryPath = Path.GetDirectoryName(path);
            }
            else									//path为文件夹路径
            {
                directoryPath = path;
            }
            return directoryPath;
        }


        public static bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static bool IsInDirectory(string parentDirectoryPath, string path)
        {
            if (string.IsNullOrEmpty(parentDirectoryPath) || string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException();
            }

            parentDirectoryPath = parentDirectoryPath.Trim().ToLower();
            path = path.Trim().ToLower();

            var ch1 = parentDirectoryPath[parentDirectoryPath.Length - 1];
            if (ch1 == Path.DirectorySeparatorChar)
            {
                parentDirectoryPath = parentDirectoryPath.Substring(0, parentDirectoryPath.Length - 1);
            }

            var ch2 = path[path.Length - 1];
            if (ch2 == Path.DirectorySeparatorChar)
            {
                path = path.Substring(0, path.Length - 1);
            }

            return path.StartsWith(parentDirectoryPath);
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
            foreach (var srcDir in Directory.GetDirectories(srcDirectoryPath))
            {
                var directoryName = PathUtils.GetDirectoryName(srcDir);

                var destDir = destDirectoryPath + directoryName;
                //如果该目录不存在，则创建该目录。 
                CreateDirectoryIfNotExists(destDir);
                //由于我们处于递归模式下，因此还要复制子目录
                MoveDirectory(srcDir, destDir, isOverride);
            }

            //从当前父目录中获取文件。
            foreach (var srcFile in Directory.GetFiles(srcDirectoryPath))
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


        public static string[] GetDirectoryNames(string directoryPath)
        {
            var directorys = Directory.GetDirectories(directoryPath);
            var retval = new string[directorys.Length];
            var i = 0;
            foreach (var directory in directorys)
            {
                var directoryInfo = new DirectoryInfo(directory);
                retval[i++] = directoryInfo.Name;
            }
            return retval;
        }

        public static ArrayList GetLowerDirectoryNames(string directoryPath)
        {
            var arraylist = new ArrayList();
            var directorys = Directory.GetDirectories(directoryPath);
            foreach (var directory in directorys)
            {
                var directoryInfo = new DirectoryInfo(directory);
                arraylist.Add(directoryInfo.Name.ToLower());
            }
            return arraylist;
        }

        public static string[] GetFileNames(string directoryPath)
        {
            var filePaths = Directory.GetFiles(directoryPath);
            var retval = new string[filePaths.Length];
            var i = 0;
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                retval[i++] = fileInfo.Name;
            }
            return retval;
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="directoryPath">文件夹路径</param>
        /// <returns>删除过程中是否出错</returns>
        public static bool DeleteDirectoryIfExists(string directoryPath)
        {
            var retval = true;
            try
            {
                if (IsDirectoryExists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }
            }
            catch
            {
                retval = false;
            }
            return retval;
        }

        public static void DeleteFilesSync(string rootDirectoryPath, string syncDirectoryPath)
        {
            if (IsDirectoryExists(syncDirectoryPath))
            {
                var directoryInfo = new DirectoryInfo(syncDirectoryPath);
                foreach (var fileSystemInfo in directoryInfo.GetFileSystemInfos())
                {
                    var fileSystemPath = PathUtils.Combine(rootDirectoryPath, fileSystemInfo.Name);
                    if (fileSystemInfo is FileInfo)
                    {
                        try
                        {
                            FileUtils.DeleteFileIfExists(fileSystemPath);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    else if (fileSystemInfo is DirectoryInfo)
                    {
                        DeleteFilesSync(fileSystemPath, fileSystemInfo.FullName);
                        DeleteEmptyDirectory(fileSystemPath);
                    }
                }
            }
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

        public static void CreateUrlRedirectDirectory(string sourceUrlRedirectFilePath, string directoryPath)
        {
            CreateDirectoryIfNotExists(directoryPath);
            var filePath = PathUtils.Combine(directoryPath, "default.aspx");
            if (!FileUtils.IsFileExists(filePath))
            {
                FileUtils.CopyFile(sourceUrlRedirectFilePath, filePath);
            }
        }

        public static string[] GetDirectoryPaths(string directoryPath)
        {
            CreateDirectoryIfNotExists(directoryPath);
            return Directory.GetDirectories(directoryPath);
        }

        public static string[] GetDirectoryPaths(string directoryPath, string searchPattern)
        {
            CreateDirectoryIfNotExists(directoryPath);
            return Directory.GetDirectories(directoryPath, searchPattern);
        }

        public static string[] GetFilePaths(string directoryPath)
        {
            return Directory.GetFiles(directoryPath);
        }

        public static long GetDirectorySize(string directoryPath)
        {
            long size = 0;
            var filePaths = GetFilePaths(directoryPath);
            //通过GetFiles方法,获取目录中所有文件的大小
            foreach (var filePath in filePaths)
            {
                var info = new FileInfo(filePath);
                size += info.Length;
            }
            var directoryPaths = GetDirectoryPaths(directoryPath);
            //获取目录下所有文件夹大小,并存到一个新的对象数组中
            foreach (var path in directoryPaths)
            {
                size += GetDirectorySize(path);
            }
            return size;
        }

        public static bool IsSystemDirectory(string directoryName)
        {
            if (StringUtils.EqualsIgnoreCase(directoryName, AspnetClient.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, Bin.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, SiteFiles.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, FileConfigManager.Instance.AdminDirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, SiteTemplates.SiteTemplateMetadata)
                || StringUtils.EqualsIgnoreCase(directoryName, Api.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, "obj")
                || StringUtils.EqualsIgnoreCase(directoryName, "Properties"))
            {
                return true;
            }
            return false;
        }

        public static bool IsWebSiteDirectory(string directoryName)
        {
            return StringUtils.EqualsIgnoreCase(directoryName, "channels")
                   || StringUtils.EqualsIgnoreCase(directoryName, "contents")
                   || StringUtils.EqualsIgnoreCase(directoryName, "Template")
                   || StringUtils.EqualsIgnoreCase(directoryName, "include")
                   || StringUtils.EqualsIgnoreCase(directoryName, "upload");
        }

        public static List<string> GetLowerSystemDirectoryNames()
        {
            return new List<string>
            {
                AspnetClient.DirectoryName.ToLower(),
                Bin.DirectoryName.ToLower(),
                SiteFiles.DirectoryName.ToLower(),
                FileConfigManager.Instance.AdminDirectoryName.ToLower(),
                SiteTemplates.SiteTemplateMetadata.ToLower()
            };
        }
    }
}
