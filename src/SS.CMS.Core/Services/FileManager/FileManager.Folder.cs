using System;
using System.Collections;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class FileManager
    {
        public void ChangeSiteDir(string parentPsPath, string oldPsDir, string newPsDir)
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

        public void DeleteSiteFiles(SiteInfo siteInfo)
        {
            if (siteInfo == null) return;

            var sitePath = _pathManager.GetSitePath(siteInfo);

            if (siteInfo.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(sitePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!_pathManager.IsSystemFile(fileName))
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                    }
                }

                var siteDirList = _siteRepository.GetLowerSiteDirListThatNotIsRoot();

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(sitePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!_pathManager.IsSystemDirectory(directoryName) && !siteDirList.Contains(directoryName.ToLower()))
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

        public void ImportSiteFiles(SiteInfo siteInfo, string siteTemplatePath, bool isOverride)
        {
            var sitePath = _pathManager.GetSitePath(siteInfo);

            if (siteInfo.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!_pathManager.IsSystemFile(fileName))
                    {
                        var destFilePath = PathUtils.Combine(sitePath, fileName);
                        FileUtils.MoveFile(filePath, destFilePath, isOverride);
                    }
                }

                var siteDirList = _siteRepository.GetLowerSiteDirListThatNotIsRoot();

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!_pathManager.IsSystemDirectory(directoryName) && !siteDirList.Contains(directoryName.ToLower()))
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

        public void ChangeParentSite(ISiteRepository siteRepository, int oldParentSiteId, int newParentSiteId, int siteId, string siteDir)
        {
            if (oldParentSiteId == newParentSiteId) return;

            string oldPsPath;
            if (oldParentSiteId != 0)
            {
                var oldSiteInfo = siteRepository.GetSiteInfo(oldParentSiteId);

                oldPsPath = PathUtils.Combine(_pathManager.GetSitePath(oldSiteInfo), siteDir);
            }
            else
            {
                var siteInfo = siteRepository.GetSiteInfo(siteId);
                oldPsPath = _pathManager.GetSitePath(siteInfo);
            }

            string newPsPath;
            if (newParentSiteId != 0)
            {
                var newSiteInfo = siteRepository.GetSiteInfo(newParentSiteId);

                newPsPath = PathUtils.Combine(_pathManager.GetSitePath(newSiteInfo), siteDir);
            }
            else
            {
                newPsPath = PathUtils.Combine(_settingsManager.WebRootPath, siteDir);
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

        public void ChangeToHeadquarters(SiteInfo siteInfo, bool isMoveFiles)
        {
            if (siteInfo.Root == false)
            {
                var sitePath = _pathManager.GetSitePath(siteInfo);

                _siteRepository.UpdateParentIdToZero(siteInfo.Id);

                siteInfo.Root = true;
                siteInfo.SiteDir = string.Empty;

                _siteRepository.Update(siteInfo);
                if (isMoveFiles)
                {
                    DirectoryUtils.MoveDirectory(sitePath, _settingsManager.WebRootPath, false);
                    DirectoryUtils.DeleteDirectoryIfExists(sitePath);
                }
            }
        }

        public void ChangeToSubSite(SiteInfo siteInfo, string psDir, ArrayList fileSystemNameArrayList)
        {
            if (siteInfo.Root)
            {
                siteInfo.Root = false;
                siteInfo.SiteDir = psDir.Trim();

                _siteRepository.Update(siteInfo);

                var psPath = PathUtils.Combine(_settingsManager.WebRootPath, psDir);
                DirectoryUtils.CreateDirectoryIfNotExists(psPath);
                if (fileSystemNameArrayList != null && fileSystemNameArrayList.Count > 0)
                {
                    foreach (string fileSystemName in fileSystemNameArrayList)
                    {
                        var srcPath = PathUtils.Combine(_settingsManager.WebRootPath, fileSystemName);
                        if (DirectoryUtils.IsDirectoryExists(srcPath))
                        {
                            var destDirectoryPath = PathUtils.Combine(psPath, fileSystemName);
                            DirectoryUtils.CreateDirectoryIfNotExists(destDirectoryPath);
                            DirectoryUtils.MoveDirectory(srcPath, destDirectoryPath, false);
                            DirectoryUtils.DeleteDirectoryIfExists(srcPath);
                        }
                        else if (FileUtils.IsFileExists(srcPath))
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
