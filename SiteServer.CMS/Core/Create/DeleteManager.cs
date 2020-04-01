using System.Collections.Generic;
using System.IO;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core.Create
{
    public class DeleteManager
    {
        public static void DeleteContentsByPage(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId);
                if (contentIdList.Count > 0)
                {
                    foreach (var contentId in contentIdList)
                    {
                        var filePath = PathUtility.GetContentPageFilePath(siteInfo, channelId, contentId, 0);
                        FileUtils.DeleteFileIfExists(filePath);
                        DeletePagingFiles(filePath);
                        DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                    }
                }
            }
        }

        public static void DeleteContents(SiteInfo siteInfo, int channelId, List<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                DeleteContent(siteInfo, channelId, contentId);
            }
        }

        public static void DeleteContent(SiteInfo siteInfo, int channelId, int contentId)
        {
            var filePath = PathUtility.GetContentPageFilePath(siteInfo, channelId, contentId, 0);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public static void DeleteChannels(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var filePath = PathUtility.GetChannelPageFilePath(siteInfo, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId);
                if (contentIdList.Count > 0)
                {
                    DeleteContents(siteInfo, channelId, contentIdList);
                }
            }
        }

        public static void DeleteChannelsByPage(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                if (channelId != siteInfo.Id)
                {
                    var filePath = PathUtility.GetChannelPageFilePath(siteInfo, channelId, 0);
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

        public static void DeleteFiles(SiteInfo siteInfo, List<int> templateIdList)
        {
            foreach (var templateId in templateIdList)
            {
                var templateInfo = TemplateManager.GetTemplateInfo(siteInfo.Id, templateId);
                if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
                {
                    return;
                }

                var filePath = PathUtility.MapPath(siteInfo, templateInfo.CreatedFileFullName);

                FileUtils.DeleteFileIfExists(filePath);
            }
        }
    }
}
