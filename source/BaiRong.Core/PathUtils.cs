using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System;
using System.Xml;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core
{
    public class PathUtils
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

        public static string GetCurrentFileNameWithoutExtension()
        {
            if (HttpContext.Current != null)
            {
                return Path.GetFileNameWithoutExtension(HttpContext.Current.Request.PhysicalPath);
            }
            return string.Empty;
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

        public static string GetExtension(string path)
        {
            var retval = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
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

        public static string GetDirectoryName(string directoryPath)
        {
            if (!string.IsNullOrEmpty(directoryPath))
            {
                var directoryInfo = new DirectoryInfo(directoryPath);
                return directoryInfo.Name;
            }
            return string.Empty;
        }

        public static string GetDirectoryDifference(string rootDirectoryPath, string path)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(path);
            if (!string.IsNullOrEmpty(directoryPath) && StringUtils.StartsWithIgnoreCase(directoryPath, rootDirectoryPath))
            {
                var retval = directoryPath.Substring(rootDirectoryPath.Length, directoryPath.Length - rootDirectoryPath.Length);
                return retval.Trim('/', '\\');
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

        public static string AddVirtualToPath(string path)
        {
            var resolvedPath = path;
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("../", string.Empty);
                if (!path.StartsWith("~"))
                {
                    resolvedPath = "~" + path;
                }
            }
            return resolvedPath;
        }

        public static string GetCurrentPagePath()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.PhysicalPath;
            }
            return string.Empty;
        }

        public static string GetSiteFilesPath(params string[] paths)
        {
            return MapPath(Combine("~/sitefiles", Combine(paths)));
        }

        public static string RemovePathInvalidChar(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;
            var invalidChars = new string(Path.GetInvalidPathChars());
            string invalidReStr = $"[{Regex.Escape(invalidChars)}]";
            return Regex.Replace(filePath, invalidReStr, "");
        }

        public static string MapPath(string virtualPath)
        {
            virtualPath = RemovePathInvalidChar(virtualPath);
            string retval;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if (virtualPath.StartsWith("~"))
                {
                    virtualPath = virtualPath.Substring(1);
                }
                virtualPath = PageUtils.Combine("~", virtualPath);
            }
            else
            {
                virtualPath = "~/";
            }
            if (HttpContext.Current != null)
            {
                retval = HttpContext.Current.Server.MapPath(virtualPath);
            }
            else
            {
                var rootPath = WebConfigUtils.PhysicalApplicationPath;

                if (!string.IsNullOrEmpty(virtualPath))
                {
                    virtualPath = virtualPath.Substring(2);
                }
                else
                {
                    virtualPath = string.Empty;
                }
                retval = Combine(rootPath, virtualPath);
            }

            if (retval == null) retval = string.Empty;
            return retval.Replace("/", "\\");
        }

        public static bool IsFileExtenstionAllowed(string sAllowedExt, string sExt)
        {
            var allow = false;
            if (sExt != null && sExt.StartsWith("."))
            {
                sExt = sExt.Substring(1, sExt.Length - 1);
            }
            sAllowedExt = sAllowedExt.Replace("|", ",");
            var aExt = sAllowedExt.Split(',');
            for (var i = 0; i < aExt.Length; i++)
            {
                if (StringUtils.EqualsIgnoreCase(sExt, aExt[i]))
                {
                    allow = true;
                    break;
                }
            }
            return allow;
        }

        public static bool IsFileExtenstionNotAllowed(string sNotAllowedExt, string sExt)
        {
            var allow = true;
            if (sExt != null && sExt.StartsWith("."))
            {
                sExt = sExt.Substring(1, sExt.Length - 1);
            }
            sNotAllowedExt = sNotAllowedExt.Replace("|", ",");
            var aExt = sNotAllowedExt.Split(',');
            for (var i = 0; i < aExt.Length; i++)
            {
                if (StringUtils.EqualsIgnoreCase(sExt, aExt[i]))
                {
                    allow = false;
                    break;
                }
            }
            return allow;
        }

        public static string GetClientUserPath(string applicationName, string userName, string relatedPath)
        {
            string systemName;
            if (!string.IsNullOrEmpty(applicationName) && applicationName.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                systemName = applicationName.Split('_')[1];
            }
            else
            {
                systemName = applicationName;
            }
            return GetSiteFilesPath($"{DirectoryUtils.SiteFiles.UserFiles}/{userName}/{systemName}/{relatedPath}");
        }

        public static string GetTemporaryFilesPath(string relatedPath)
        {
            return Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedPath);
        }

        public static string GetMenusPath(params string[] paths)
        {
            var directoryPath = WebConfigUtils.PhysicalApplicationPath;
            return Combine(directoryPath, DirectoryUtils.SiteFiles.DirectoryName, "Configuration/Menus", Combine(paths));
        }

        public static string GetUpgradeSqlFilePath(bool isMySql, bool isTable)
        {
            var relatedPath = isMySql ? "sql/mysql/" : "sql/sqlserver/";
            relatedPath += isTable ? "upgrade_tables.sql" : "upgrade.sql";
            return SiteServerAssets.GetPath(relatedPath);
        }

        public static string GetInstallSqlFilePath(bool isMySql)
        {
            return SiteServerAssets.GetPath(isMySql ? "sql/mysql/install.sql" : "sql/sqlserver/install.sql");
        }

        public static string GetUserFilesPath(string userName, string relatedPath)
        {
            return Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.UserFiles, userName, relatedPath);
        }

        public static string GetUserUploadDirectoryPath(string userName)
        {
            string directoryPath;
            var dateFormatType = EDateFormatTypeUtils.GetEnumType(ConfigManager.UserConfigInfo.UploadDateFormatString);
            var datetime = DateTime.Now;
            var userFilesPath = GetUserFilesPath(userName, string.Empty);
            if (dateFormatType == EDateFormatType.Year)
            {
                directoryPath = Combine(userFilesPath, datetime.Year.ToString());
            }
            else if (dateFormatType == EDateFormatType.Day)
            {
                directoryPath = Combine(userFilesPath, datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day.ToString());
            }
            else
            {
                directoryPath = Combine(userFilesPath, datetime.Year.ToString(), datetime.Month.ToString());
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            return directoryPath;
        }

        public static string GetUserUploadFileName(string filePath)
        {
            string retval;
            if (ConfigManager.UserConfigInfo.IsUploadChangeFileName)
            {
                var dt = DateTime.Now;
                string strDateTime = $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}";
                retval = $"{strDateTime}{GetExtension(filePath)}";
            }
            else
            {
                retval = GetFileName(filePath);
            }
            return retval;
        }

        public static string PhysicalSiteServerPath => Combine(WebConfigUtils.PhysicalApplicationPath, FileConfigManager.Instance.AdminDirectoryName);

        public static string PhysicalSiteFilesPath => Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName);
    }
}
