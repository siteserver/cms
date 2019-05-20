using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;

namespace SiteServer.Utils
{
    public static class PathUtils
    {
        public const char SeparatorChar = '\\';
        public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static string Combine(params string[] paths)
        {
            var retval = string.Empty;
            if (paths != null && paths.Length > 0)
            {
                retval = paths[0]?.Replace(PageUtils.SeparatorChar, SeparatorChar).TrimEnd(SeparatorChar) ?? string.Empty;
                for (var i = 1; i < paths.Length; i++)
                {
                    var path = paths[i] != null ? paths[i].Replace(PageUtils.SeparatorChar, SeparatorChar).Trim(SeparatorChar) : string.Empty;
                    retval = Path.Combine(retval, path);
                }
            }
            return retval;
        }

        /// <summary>
        /// 根据路径扩展名判断是否为文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryPath(string path)
        {
            var retval = false;
            if (!string.IsNullOrEmpty(path))
            {
                var ext = Path.GetExtension(path);
                if (string.IsNullOrEmpty(ext))		//path为文件路径
                {
                    retval = true;
                }
            }
            return retval;
        }

        public static bool IsFilePath(string val)
        {
            try
            {
                return FileUtils.IsFileExists(val);
            }
            catch
            {
                return false;
            }
        }

        public static string GetExtension(string path)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                path = PageUtils.RemoveQueryString(path);
                path = path.Trim('/', '\\').Trim();
                try
                {
                    retval = Path.GetExtension(path);
                }
                catch
                {
                    // ignored
                }
            }
            return retval;
        }

        public static string RemoveExtension(string fileName)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(fileName))
            {
                var index = fileName.LastIndexOf('.');
                retval = index != -1 ? fileName.Substring(0, index) : fileName;
            }
            return retval;
        }

        public static string RemoveParentPath(string path)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                retval = path.Replace("../", string.Empty);
                retval = retval.Replace("./", string.Empty);
            }
            return retval;
        }

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

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

        public static string GetPathDifference(string rootPath, string path)
        {
            if (!string.IsNullOrEmpty(path) && StringUtils.StartsWithIgnoreCase(path, rootPath))
            {
                var retval = path.Substring(rootPath.Length, path.Length - rootPath.Length);
                return retval.Trim('/', '\\');
            }
            return string.Empty;
        }

        public static string GetBinDirectoryPath(string relatedPath)
        {
            relatedPath = RemoveParentPath(relatedPath);
            return Combine(AppSettings.PhysicalApplicationPath, DirectoryUtils.Bin.DirectoryName, relatedPath);
        }

        public static string GetAdminDirectoryPath(string relatedPath)
        {
            relatedPath = RemoveParentPath(relatedPath);
            return Combine(AppSettings.PhysicalApplicationPath, AppSettings.AdminDirectory, relatedPath);
        }

        public static string GetHomeDirectoryPath(string relatedPath)
        {
            relatedPath = RemoveParentPath(relatedPath);
            return Combine(AppSettings.PhysicalApplicationPath, AppSettings.HomeDirectory, relatedPath);
        }

        public static string RemovePathInvalidChar(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;
            var invalidChars = new string(Path.GetInvalidPathChars());
            string invalidReStr = $"[{Regex.Escape(invalidChars)}]";
            return Regex.Replace(filePath, invalidReStr, "");
        }

        public static bool IsFileExtenstionAllowed(string sAllowedExt, string sExt)
        {
            if (sExt != null && sExt.StartsWith("."))
            {
                sExt = sExt.Substring(1, sExt.Length - 1);
            }
            sAllowedExt = sAllowedExt.Replace("|", ",");
            var aExt = sAllowedExt.Split(',');
            return aExt.Any(t => StringUtils.EqualsIgnoreCase(sExt, t));
        }

        public static string GetTemporaryFilesPath(string relatedPath)
        {
            return Combine(AppSettings.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedPath);
        }

        public static string PhysicalSiteServerPath => Combine(AppSettings.PhysicalApplicationPath, AppSettings.AdminDirectory);

        public static string PhysicalSiteFilesPath => Combine(AppSettings.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName);
    }
}
