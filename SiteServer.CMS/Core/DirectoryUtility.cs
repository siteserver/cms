using System;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Collections;
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

        public static void DeleteSiteFiles(SiteInfo siteInfo)
        {
            if (siteInfo == null) return;

            var sitePath = PathUtility.GetSitePath(siteInfo);

            if (siteInfo.IsRoot)
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

                var siteDirList = DataProvider.SiteDao.GetLowerSiteDirListThatNotIsRoot();

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(sitePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!DirectoryUtils.IsSystemDirectory(directoryName) && !siteDirList.Contains(directoryName.ToLower()))
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

        public static void ImportSiteFiles(SiteInfo siteInfo, string siteTemplatePath, bool isOverride)
        {
            var sitePath = PathUtility.GetSitePath(siteInfo);

            if (siteInfo.IsRoot)
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

                var siteDirList = DataProvider.SiteDao.GetLowerSiteDirListThatNotIsRoot();

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!DirectoryUtils.IsSystemDirectory(directoryName) && !siteDirList.Contains(directoryName.ToLower()))
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

        public static void ChangeParentSite(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir)
        {
            if (oldParentSiteId == newParentSiteId) return;

            string oldPsPath;
            if (oldParentSiteId != 0)
            {
                var oldSiteInfo = SiteManager.GetSiteInfo(oldParentSiteId);

                oldPsPath = PathUtils.Combine(PathUtility.GetSitePath(oldSiteInfo), siteDir);
            }
            else
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                oldPsPath = PathUtility.GetSitePath(siteInfo);
            }

            string newPsPath;
            if (newParentSiteId != 0)
            {
                var newSiteInfo = SiteManager.GetSiteInfo(newParentSiteId);

                newPsPath = PathUtils.Combine(PathUtility.GetSitePath(newSiteInfo), siteDir);
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

        public static void ChangeToHeadquarters(SiteInfo siteInfo)
        {
            if (siteInfo.IsRoot == false)
            {
                DataProvider.SiteDao.UpdateParentIdToZero(siteInfo.Id);
                siteInfo.IsRoot = true;
                DataProvider.SiteDao.Update(siteInfo);
            }
        }

        public static void ChangeToSubSite(SiteInfo siteInfo)
        {
            if (siteInfo.IsRoot)
            {
                siteInfo.IsRoot = false;
                DataProvider.SiteDao.Update(siteInfo);
            }
        }
    }
}
