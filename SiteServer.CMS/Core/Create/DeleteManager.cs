using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core.Create
{
    public static class DeleteManager
    {
        public static async Task DeleteContentsByPageAsync(Site site, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
                var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId);
                if (contentIdList.Count > 0)
                {
                    foreach (var contentId in contentIdList)
                    {
                        var filePath = await PathUtility.GetContentPageFilePathAsync(site, channelId, contentId, 0);
                        FileUtils.DeleteFileIfExists(filePath);
                        DeletePagingFiles(filePath);
                        DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                    }
                }
            }
        }

        public static async Task DeleteContentsAsync(Site site, int channelId, List<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                await DeleteContentAsync(site, channelId, contentId);
            }
        }

        public static async Task DeleteContentAsync(Site site, int channelId, int contentId)
        {
            var filePath = await PathUtility.GetContentPageFilePathAsync(site, channelId, contentId, 0);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public static async Task DeleteChannelsAsync(Site site, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var filePath = await PathUtility.GetChannelPageFilePathAsync(site, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
                var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId);
                if (contentIdList.Count > 0)
                {
                    await DeleteContentsAsync(site, channelId, contentIdList);
                }
            }
        }

        public static async Task DeleteChannelsByPageAsync(Site site, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                if (channelId != site.Id)
                {
                    var filePath = await PathUtility.GetChannelPageFilePathAsync(site, channelId, 0);
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

        public static async Task DeleteFilesAsync(Site site, List<int> templateIdList)
        {
            foreach (var templateId in templateIdList)
            {
                var templateInfo = await TemplateManager.GetTemplateAsync(site.Id, templateId);
                if (templateInfo == null || templateInfo.Type != TemplateType.FileTemplate)
                {
                    return;
                }

                var filePath = PathUtility.MapPath(site, templateInfo.CreatedFileFullName);

                FileUtils.DeleteFileIfExists(filePath);
            }
        }
    }
}
