using System.Collections;
using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Settings
{
    public static class EnvManager
    {
        public static string GetAdminPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(AppSettings.ContentRootPath, AppSettings.AdminDirectory), paths);
        }

        public static string GetHomePath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(AppSettings.ContentRootPath, AppSettings.HomeDirectory), paths);
        }

        public static string GetMenusPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(AppSettings.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.Menus), paths);
        }

        public static string GetBackupFilesPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(AppSettings.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.BackupFiles), paths);
        }

        public static string GetTemporaryFilesPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(AppSettings.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles), paths);
        }

        public static string GetSiteTemplatesPath(params string[] paths)
        {
            return PathUtils.Add(PathUtils.Combine(AppSettings.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.SiteTemplates), paths);
        }

        public static string PhysicalSiteServerPath => PathUtils.Combine(AppSettings.ContentRootPath, AppSettings.AdminDirectory);

        public static string PhysicalSiteFilesPath => PathUtils.Combine(AppSettings.ContentRootPath, DirectoryUtils.SiteFiles.DirectoryName);
    }
}
