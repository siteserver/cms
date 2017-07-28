using System;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public class DirectoryUtility
    {
        public static string GetIndexesDirectoryPath(string siteFilesDirectoryPath)
        {
            return PathUtils.Combine(siteFilesDirectoryPath, "Indexes");
        }

        public static void ChangePublishmentSystemDir(string parentPsPath, string oldPsDir, string newPsDir)
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

        public static void DeletePublishmentSystemFiles(PublishmentSystemInfo publishmentSystemInfo)
        {
            var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);

            if (publishmentSystemInfo.IsHeadquarters)
            {
                var filePaths = DirectoryUtils.GetFilePaths(publishmentSystemPath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!PathUtility.IsSystemFile(fileName))
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                    }
                }

                var publishmentSystemDirList = DataProvider.PublishmentSystemDao.GetLowerPublishmentSystemDirListThatNotIsHeadquarters();

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(publishmentSystemPath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath);
                    if (!DirectoryUtils.IsSystemDirectory(directoryName) && !publishmentSystemDirList.Contains(directoryName.ToLower()))
                    {
                        DirectoryUtils.DeleteDirectoryIfExists(subDirectoryPath);
                    }
                }
            }
            else
            {
                var direcotryPath = publishmentSystemPath;
                DirectoryUtils.DeleteDirectoryIfExists(direcotryPath);
            }
        }

        public static void ImportPublishmentSystemFiles(PublishmentSystemInfo publishmentSystemInfo, string siteTemplatePath, bool isOverride)
        {
            var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);

            if (publishmentSystemInfo.IsHeadquarters)
            {
                var filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!PathUtility.IsSystemFile(fileName))
                    {
                        var destFilePath = PathUtils.Combine(publishmentSystemPath, fileName);
                        FileUtils.MoveFile(filePath, destFilePath, isOverride);
                    }
                }

                var publishmentSystemDirList = DataProvider.PublishmentSystemDao.GetLowerPublishmentSystemDirListThatNotIsHeadquarters();

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath);
                    if (!DirectoryUtils.IsSystemDirectory(directoryName) && !publishmentSystemDirList.Contains(directoryName.ToLower()))
                    {
                        var destDirectoryPath = PathUtils.Combine(publishmentSystemPath, directoryName);
                        DirectoryUtils.MoveDirectory(subDirectoryPath, destDirectoryPath, isOverride);
                    }
                }
            }
            else
            {
                DirectoryUtils.MoveDirectory(siteTemplatePath, publishmentSystemPath, isOverride);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(publishmentSystemPath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplateMetadataPath);
        }

        public static void ChangeParentPublishmentSystem(int oldParentPublishmentSystemId, int newParentPublishmentSystemId, int publishmentSystemId, string publishmentSystemDir)
        {
            if (oldParentPublishmentSystemId == newParentPublishmentSystemId) return;

            string oldPsPath;
            if (oldParentPublishmentSystemId != 0)
            {
                var oldPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(oldParentPublishmentSystemId);

                oldPsPath = PathUtils.Combine(PathUtility.GetPublishmentSystemPath(oldPublishmentSystemInfo), publishmentSystemDir);
            }
            else
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                oldPsPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
            }

            string newPsPath;
            if (newParentPublishmentSystemId != 0)
            {
                var newPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(newParentPublishmentSystemId);

                newPsPath = PathUtils.Combine(PathUtility.GetPublishmentSystemPath(newPublishmentSystemInfo), publishmentSystemDir);
            }
            else
            {
                newPsPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, publishmentSystemDir);
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

        public static void DeleteContentsByPage(PublishmentSystemInfo publishmentSystemInfo, List<int> nodeIdList)
        {
            foreach (var nodeId in nodeIdList)
            {
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(tableName, nodeId);
                if (contentIdList.Count > 0)
                {
                    foreach (var contentId in contentIdList)
                    {
                        var filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, nodeId, contentId, 0);
                        FileUtils.DeleteFileIfExists(filePath);
                        DeletePagingFiles(filePath);
                        DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                    }
                }
            }
        }

        public static void DeleteContents(PublishmentSystemInfo publishmentSystemInfo, int nodeId, List<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                var filePath = PathUtility.GetContentPageFilePath(publishmentSystemInfo, nodeId, contentId, 0);
                FileUtils.DeleteFileIfExists(filePath);
            }
        }

        public static void DeleteChannels(PublishmentSystemInfo publishmentSystemInfo, List<int> nodeIdList)
        {
            foreach (var nodeId in nodeIdList)
            {
                var filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, nodeId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(tableName, nodeId);
                if (contentIdList.Count > 0)
                {
                    DeleteContents(publishmentSystemInfo, nodeId, contentIdList);
                }
            }
        }

        public static void DeleteChannelsByPage(PublishmentSystemInfo publishmentSystemInfo, List<int> nodeIdList)
        {
            foreach (var nodeId in nodeIdList)
            {
                if (nodeId != publishmentSystemInfo.PublishmentSystemId)
                {
                    var filePath = PathUtility.GetChannelPageFilePath(publishmentSystemInfo, nodeId, 0);
                    FileUtils.DeleteFileIfExists(filePath);
                    DeletePagingFiles(filePath);
                    DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                }
            }
        }

        public static void DeletePagingFiles(string filePath)
        {
            var fileName = (new FileInfo(filePath)).Name;
            fileName = fileName.Substring(0, fileName.IndexOf('.'));
            var filesPath = DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(filePath));
            foreach (var otherFilePath in filesPath)
            {
                var otherFileName = (new FileInfo(otherFilePath)).Name;
                otherFileName = otherFileName.Substring(0, otherFileName.IndexOf('.'));
                if (otherFileName.Contains(fileName + "_"))
                {
                    var isNum = otherFileName.Replace(fileName + "_", string.Empty);
                    if (ConvertHelper.GetInteger(isNum) > 0)
                    {
                        FileUtils.DeleteFileIfExists(otherFilePath);
                    }
                }
            }
        }

        public static void DeleteFiles(PublishmentSystemInfo publishmentSystemInfo, ArrayList templateIdArrayList)
        {
            foreach (int templateId in templateIdArrayList)
            {
                var templateInfo = TemplateManager.GetTemplateInfo(publishmentSystemInfo.PublishmentSystemId, templateId);
                if (templateInfo == null || templateInfo.TemplateType != ETemplateType.FileTemplate)
                {
                    return;
                }

                var filePath = PathUtility.MapPath(publishmentSystemInfo, templateInfo.CreatedFileFullName);

                FileUtils.DeleteFileIfExists(filePath);
            }
        }

        public static void ChangeToHeadquarters(PublishmentSystemInfo publishmentSystemInfo, bool isMoveFiles)
        {
            if (publishmentSystemInfo.IsHeadquarters == false)
            {
                var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);

                DataProvider.PublishmentSystemDao.UpdateParentPublishmentSystemIdToZero(publishmentSystemInfo.PublishmentSystemId);

                publishmentSystemInfo.IsHeadquarters = true;
                publishmentSystemInfo.PublishmentSystemDir = string.Empty;
                publishmentSystemInfo.PublishmentSystemUrl = WebConfigUtils.ApplicationPath;

                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
                if (isMoveFiles)
                {
                    DirectoryUtils.MoveDirectory(publishmentSystemPath, WebConfigUtils.PhysicalApplicationPath, false);
                    DirectoryUtils.DeleteDirectoryIfExists(publishmentSystemPath);
                }
            }
        }

        public static void ChangeToSubSite(PublishmentSystemInfo publishmentSystemInfo, string psDir, ArrayList fileSystemNameArrayList)
        {
            if (publishmentSystemInfo.IsHeadquarters)
            {
                publishmentSystemInfo.IsHeadquarters = false;
                publishmentSystemInfo.PublishmentSystemDir = psDir.Trim();
                publishmentSystemInfo.PublishmentSystemUrl = PageUtils.Combine(WebConfigUtils.ApplicationPath, psDir.Trim());

                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);

                var psPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, psDir);
                DirectoryUtils.CreateDirectoryIfNotExists(psPath);
                if (fileSystemNameArrayList != null && fileSystemNameArrayList.Count > 0)
                {
                    foreach (string fileSystemName in fileSystemNameArrayList)
                    {
                        var srcPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, fileSystemName);
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

        public static string GetBlogSystemPath(PublishmentSystemInfo publishmentSystemInfo, string blogSystemDir)
        {
            var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);

            return PathUtils.Combine(publishmentSystemPath, blogSystemDir);
        }
    }
}
