using System.Collections.Generic;
using System.IO;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Db;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core.Create
{
    public class DeleteManager
    {
        public static void DeleteContentsByPage(Site site, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var tableName = ChannelManager.GetTableName(site, channelId);
                var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId);
                if (contentIdList.Count > 0)
                {
                    foreach (var contentId in contentIdList)
                    {
                        var filePath = PathUtility.GetContentPageFilePath(site, channelId, contentId, 0);
                        FileUtils.DeleteFileIfExists(filePath);
                        DeletePagingFiles(filePath);
                        DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                    }
                }
            }
        }

        public static void DeleteContents(Site site, int channelId, List<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                DeleteContent(site, channelId, contentId);
            }
        }

        public static void DeleteContent(Site site, int channelId, int contentId)
        {
            var filePath = PathUtility.GetContentPageFilePath(site, channelId, contentId, 0);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public static void DeleteChannels(Site site, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var filePath = PathUtility.GetChannelPageFilePath(site, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var tableName = ChannelManager.GetTableName(site, channelId);
                var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId);
                if (contentIdList.Count > 0)
                {
                    DeleteContents(site, channelId, contentIdList);
                }
            }
        }

        public static void DeleteChannelsByPage(Site site, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                if (channelId != site.Id)
                {
                    var filePath = PathUtility.GetChannelPageFilePath(site, channelId, 0);
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

        public static void DeleteFiles(Site site, List<int> templateIdList)
        {
            foreach (var templateId in templateIdList)
            {
                var templateInfo = TemplateManager.GetTemplateInfo(site.Id, templateId);
                if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
                {
                    return;
                }

                var filePath = PathUtility.MapPath(site, templateInfo.CreatedFileFullName);

                FileUtils.DeleteFileIfExists(filePath);
            }
        }
    }
}
