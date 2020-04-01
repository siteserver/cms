using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Linq;
using SiteServer.Utils.Enumerations;

namespace SiteServer.Utils
{
    public static class PathUtils
    {
        public const char SeparatorChar = '\\';
        public static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();

        public static bool IsExtension(string ext, params string[] extensions)
        {
            return extensions.Any(extension => StringUtils.EqualsIgnoreCase(ext, extension));
        }

        public static string GetLibraryFileName(string filePath)
        {
            return $"{StringUtils.GetShortGuid(false)}{GetExtension(filePath)}";
        }

        public static string GetLibraryVirtualPath(EUploadType uploadType, string fileName)
        {
            var uploadDirectoryName = string.Empty;

            if (uploadType == EUploadType.Image)
            {
                uploadDirectoryName = "images";
            }
            else if (uploadType == EUploadType.Video)
            {
                uploadDirectoryName = "videos";
            }
            else if (uploadType == EUploadType.File)
            {
                uploadDirectoryName = "files";
            }
            else if (uploadType == EUploadType.Special)
            {
                uploadDirectoryName = "specials";
            }

            return $"/{DirectoryUtils.SiteFiles.DirectoryName}/{DirectoryUtils.SiteFiles.Library}/{uploadDirectoryName}/{DateTime.Now.Year}/{DateTime.Now.Month}/{fileName}";
        }

        //将编辑器中图片上传至本机
        public static string SaveLibraryImage(string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, PageUtils.ApplicationPath))
                    continue;
                var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
                if (!EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName)) continue;

                var fileName = GetLibraryFileName(originalImageSrc);
                var virtualPath = GetLibraryVirtualPath(EUploadType.Image, fileName);
                var filePath = Combine(WebConfigUtils.PhysicalApplicationPath, virtualPath);

                try
                {
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        WebClientUtils.SaveRemoteFileToLocal(originalImageSrc, filePath);
                    }
                    content = content.Replace(originalImageSrc, virtualPath);
                }
                catch
                {
                    // ignored
                }
            }
            return content;
        }

        public static string Combine(params string[] paths)
        {
            var retVal = string.Empty;
            if (paths != null && paths.Length > 0)
            {
                retVal = paths[0]?.Replace(PageUtils.SeparatorChar, SeparatorChar).TrimEnd(SeparatorChar) ?? string.Empty;
                for (var i = 1; i < paths.Length; i++)
                {
                    var path = paths[i] != null ? paths[i].Replace(PageUtils.SeparatorChar, SeparatorChar).Trim(SeparatorChar) : string.Empty;
                    retVal = Path.Combine(retVal, path);
                }
            }
            return retVal;
        }

        /// <summary>
        /// 根据路径扩展名判断是否为文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsDirectoryPath(string path)
        {
            var retVal = false;
            if (!string.IsNullOrEmpty(path))
            {
                var ext = Path.GetExtension(path);
                if (string.IsNullOrEmpty(ext))		//path为文件路径
                {
                    retVal = true;
                }
            }
            return retVal;
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
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                path = PageUtils.RemoveQueryString(path);
                path = path.Trim('/', '\\').Trim();
                try
                {
                    retVal = Path.GetExtension(path);
                }
                catch
                {
                    // ignored
                }
            }
            return retVal;
        }

        public static string RemoveExtension(string fileName)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(fileName))
            {
                var index = fileName.LastIndexOf('.');
                retVal = index != -1 ? fileName.Substring(0, index) : fileName;
            }
            return retVal;
        }

        public static string RemoveParentPath(string path)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                retVal = path.Replace("../", string.Empty);
                retVal = retVal.Replace("./", string.Empty);
            }
            return retVal;
        }

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        private static char[] GetInvalidChars()
        {
            return Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Concat(new[] {' ', ';'}).ToArray();
        }

        public static string GetSafeFilename(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return StringUtils.GetShortGuid().ToLower();

            return string.Join("_", filename.Split(GetInvalidChars()));
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
                var retVal = path.Substring(rootPath.Length, path.Length - rootPath.Length);
                return retVal.Trim('/', '\\');
            }
            return string.Empty;
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
            return MapPath(Combine("~/" + DirectoryUtils.SiteFiles.DirectoryName, Combine(paths)));
        }

        public static string GetBinDirectoryPath(string relatedPath)
        {
            relatedPath = RemoveParentPath(relatedPath);
            return Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.Bin.DirectoryName, relatedPath);
        }

        public static string GetAdminDirectoryPath(string relatedPath)
        {
            relatedPath = RemoveParentPath(relatedPath);
            return Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.AdminDirectory, relatedPath);
        }

        public static string GetHomeDirectoryPath(string relatedPath)
        {
            relatedPath = RemoveParentPath(relatedPath);
            return Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.HomeDirectory, relatedPath);
        }

        public static string PluginsPath => GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins);

        public static string GetPluginPath(string pluginId, params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins, pluginId, Combine(paths));
        }

        public static string GetPluginNuspecPath(string pluginId)
        {
            return GetPluginPath(pluginId, pluginId + ".nuspec");
        }

        public static string GetPluginDllDirectoryPath(string pluginId)
        {
            var fileName = pluginId + ".dll";

            var filePaths = Directory.GetFiles(GetPluginPath(pluginId, "Bin"), fileName, SearchOption.AllDirectories);

            var dict = new Dictionary<DateTime, string>();
            foreach (var filePath in filePaths)
            {
                var lastModifiedDate = File.GetLastWriteTime(filePath);
                dict[lastModifiedDate] = filePath;
            }

            if (dict.Count > 0)
            {
                var filePath = dict.OrderByDescending(x => x.Key).First().Value;
                return Path.GetDirectoryName(filePath);
            }

            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", "net4.6.1", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", "net4.6.1", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Debug");
            //}
            //if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Release", fileName)))
            //{
            //    return GetPluginPath(pluginId, "Bin", "Release");
            //}
            
            return string.Empty;
        }

        public static string GetPackagesPath(params string[] paths)
        {
            var packagesPath = GetSiteFilesPath(DirectoryUtils.SiteFiles.Packages, Combine(paths));
            DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
            return packagesPath;
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
            string retVal;
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
                retVal = HttpContext.Current.Server.MapPath(virtualPath);
            }
            else
            {
                var rootPath = WebConfigUtils.PhysicalApplicationPath;

                virtualPath = !string.IsNullOrEmpty(virtualPath) ? virtualPath.Substring(2) : string.Empty;
                retVal = Combine(rootPath, virtualPath);
            }

            if (retVal == null) retVal = string.Empty;
            return retVal.Replace("/", "\\");
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
            return Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedPath);
        }

        public static string GetMenusPath(params string[] paths)
        {
            return Combine(SiteServerAssets.GetPath("menus"), Combine(paths));
        }

        public static string PhysicalSiteServerPath => Combine(WebConfigUtils.PhysicalApplicationPath, WebConfigUtils.AdminDirectory);

        public static string PhysicalSiteFilesPath => Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName);
    }
}
