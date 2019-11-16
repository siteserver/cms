using System;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Core
{
    public static class DirectoryUtility
    {
        public static void ChangeSiteDir(string parentPsPath, string oldPsDir, string newPsDir)
        {
            var oldPsPath = PathUtils.Combine(parentPsPath, oldPsDir);
            var newPsPath = PathUtils.Combine(parentPsPath, newPsDir);
            if (DirectoryUtils.IsDirectoryExists(newPsPath))
            {
                throw new ArgumentException("发布系统修改失败，发布路径文件夹已存在！");
            }
            if (DirectoryUtils.IsDirectoryExists(oldPsPath))
            {
                DirectoryUtils.MoveDirectory(oldPsPath, newPsPath, false);
            }
            else
            {
                DirectoryUtils.CreateDirectoryIfNotExists(newPsPath);
            }
        }

        public static async Task DeleteSiteFilesAsync(Site site)
        {
            if (site == null) return;

            var sitePath = PathUtility.GetSitePath(site);

            if (site.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(sitePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!PathUtility.IsSystemFile(fileName))
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                    }
                }

                var siteDirList = await DataProvider.SiteDao.GetSiteDirListAsync(0);

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(sitePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!WebUtils.IsSystemDirectory(directoryName) && !StringUtils.ContainsIgnoreCase(siteDirList, directoryName))
                    {
                        DirectoryUtils.DeleteDirectoryIfExists(subDirectoryPath);
                    }
                }
            }
            else
            {
                var direcotryPath = sitePath;
                DirectoryUtils.DeleteDirectoryIfExists(direcotryPath);
            }
        }

        public static async Task ImportSiteFilesAsync(Site site, string siteTemplatePath, bool isOverride)
        {
            var sitePath = PathUtility.GetSitePath(site);

            if (site.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!PathUtility.IsSystemFile(fileName))
                    {
                        var destFilePath = PathUtils.Combine(sitePath, fileName);
                        FileUtils.MoveFile(filePath, destFilePath, isOverride);
                    }
                }

                var siteDirList = await DataProvider.SiteDao.GetSiteDirListAsync(0);

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!WebUtils.IsSystemDirectory(directoryName) && !StringUtils.ContainsIgnoreCase(siteDirList, directoryName))
                    {
                        var destDirectoryPath = PathUtils.Combine(sitePath, directoryName);
                        DirectoryUtils.MoveDirectory(subDirectoryPath, destDirectoryPath, isOverride);
                    }
                }
            }
            else
            {
                DirectoryUtils.MoveDirectory(siteTemplatePath, sitePath, isOverride);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(sitePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplateMetadataPath);
        }

        public static async Task ChangeParentSiteAsync(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir)
        {
            if (oldParentSiteId == newParentSiteId) return;

            string oldPsPath;
            if (oldParentSiteId != 0)
            {
                var oldSite = await SiteManager.GetSiteAsync(oldParentSiteId);

                oldPsPath = PathUtils.Combine(PathUtility.GetSitePath(oldSite), siteDir);
            }
            else
            {
                var site = await SiteManager.GetSiteAsync(siteId);
                oldPsPath = PathUtility.GetSitePath(site);
            }

            string newPsPath;
            if (newParentSiteId != 0)
            {
                var newSite = await SiteManager.GetSiteAsync(newParentSiteId);

                newPsPath = PathUtils.Combine(PathUtility.GetSitePath(newSite), siteDir);
            }
            else
            {
                newPsPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, siteDir);
            }

            if (DirectoryUtils.IsDirectoryExists(newPsPath))
            {
                throw new ArgumentException("发布系统修改失败，发布路径文件夹已存在！");
            }
            if (DirectoryUtils.IsDirectoryExists(oldPsPath))
            {
                DirectoryUtils.MoveDirectory(oldPsPath, newPsPath, false);
            }
            else
            {
                DirectoryUtils.CreateDirectoryIfNotExists(newPsPath);
            }
        }

        public static async Task ChangeToRootAsync(Site site, bool isMoveFiles)
        {
            if (site.Root == false)
            {
                var sitePath = PathUtility.GetSitePath(site);

                await DataProvider.SiteDao.UpdateParentIdToZeroAsync(site.Id);

                site.Root = true;
                site.SiteDir = string.Empty;

                await DataProvider.SiteDao.UpdateAsync(site);
                if (isMoveFiles)
                {
                    DirectoryUtils.MoveDirectory(sitePath, WebConfigUtils.PhysicalApplicationPath, false);
                    DirectoryUtils.DeleteDirectoryIfExists(sitePath);
                }
            }
        }

        public static async Task ChangeToSubSiteAsync(Site site, string siteDir, IList<string> directories, IList<string> files)
        {
            if (site.Root)
            {
                site.Root = false;
                site.SiteDir = siteDir;

                await DataProvider.SiteDao.UpdateAsync(site);

                var psPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, siteDir);
                DirectoryUtils.CreateDirectoryIfNotExists(psPath);
                if (directories != null)
                {
                    foreach (var fileSystemName in directories)
                    {
                        var srcPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, fileSystemName);
                        if (DirectoryUtils.IsDirectoryExists(srcPath))
                        {
                            var destDirectoryPath = PathUtils.Combine(psPath, fileSystemName);
                            DirectoryUtils.CreateDirectoryIfNotExists(destDirectoryPath);
                            DirectoryUtils.MoveDirectory(srcPath, destDirectoryPath, false);
                            DirectoryUtils.DeleteDirectoryIfExists(srcPath);
                        }
                    }
                }

                if (files != null)
                {
                    foreach (var fileSystemName in files)
                    {
                        var srcPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, fileSystemName);
                        if (FileUtils.IsFileExists(srcPath))
                        {
                            FileUtils.CopyFile(srcPath, PathUtils.Combine(psPath, fileSystemName));
                            FileUtils.DeleteFileIfExists(srcPath);
                        }
                    }
                }
            }
        }
    }
}
