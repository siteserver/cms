using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.Core.Create
{
    public static class DeleteManager
    {
        public static async Task DeleteContentsByPageAsync(Site site, IEnumerable<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelId);
                var contentIdList = await DataProvider.ContentRepository.GetContentIdListAsync(tableName, channelId);
                if (contentIdList != null)
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

        public static async Task DeleteContentsAsync(Site site, int channelId, IEnumerable<int> contentIdList)
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

        public static async Task DeleteChannelsAsync(Site site, IEnumerable<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var filePath = await PathUtility.GetChannelPageFilePathAsync(site, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelId);
                var contentIdList = await DataProvider.ContentRepository.GetContentIdListAsync(tableName, channelId);
                await DeleteContentsAsync(site, channelId, contentIdList);
            }
        }

        public static async Task DeleteChannelsByPageAsync(Site site, IEnumerable<int> channelIdList)
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

        public static async Task DeleteFilesAsync(Site site, IEnumerable<int> templateIdList)
        {
            foreach (var templateId in templateIdList)
            {
                var templateInfo = await DataProvider.TemplateRepository.GetAsync(templateId);
                if (templateInfo == null || templateInfo.TemplateType != TemplateType.FileTemplate)
                {
                    return;
                }

                var filePath = await PathUtility.MapPathAsync(site, templateInfo.CreatedFileFullName);

                FileUtils.DeleteFileIfExists(filePath);
            }
        }
    }
}
