using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization
{
	public static class BackupUtility
	{
        public const string UploadFolderName = "upload"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件夹目录
        public const string UploadFileName = "upload.xml"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件名

        public static async Task BackupTemplatesAsync(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);
            await exportObject.ExportTemplatesAsync(filePath);
        }

        public static async Task BackupChannelsAndContentsAsync(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            var channelIdList = await databaseManager.ChannelRepository.GetChannelIdsAsync(site.Id, site.Id, ScopeType.Children);

            await exportObject.ExportChannelsAsync(channelIdList, filePath);  
        }

        public static async Task BackupFilesAsync(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            await exportObject.ExportFilesAsync(filePath);
        }

        public static async Task BackupSiteAsync(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string filePath)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var siteTemplatePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
            FileUtils.DeleteFileIfExists(filePath);
            var metadataPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);

            await exportObject.ExportFilesToSiteAsync(siteTemplatePath, true, null, null, true);

            var siteContentDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.SiteTemplates.SiteContent);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, true, true, new List<int>());

            var templateFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.SiteTemplates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            var tableDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.SiteTemplates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            var configurationFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.SiteTemplates.FileConfiguration);
            await exportObject.ExportConfigurationAsync(configurationFilePath);
            exportObject.ExportMetadata(site.SiteName, await pathManager.GetWebUrlAsync(site), string.Empty, string.Empty, metadataPath);

            pathManager.CreateZip(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static async Task RecoverySiteAsync(ICacheManager cacheManager, IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, int adminId, string guid)
        {
            var importObject = new ImportObject(pathManager, databaseManager, caching, site, adminId);

            var siteTemplatePath = path;
            if (isZip)
            {
                //解压文件
                siteTemplatePath = pathManager.GetTemporaryFilesPath(BackupType.Site.GetValue());
                DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

                pathManager.ExtractZip(path, siteTemplatePath);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.SiteTemplateMetadata);

            if (isDeleteChannels)
            {
                var channelIdList = await databaseManager.ChannelRepository.GetChannelIdsAsync(site.Id, site.Id, ScopeType.Children);
                foreach (var channelId in channelIdList)
                {
                    await databaseManager.ContentRepository.TrashContentsAsync(site, channelId, adminId);
                    await databaseManager.ChannelRepository.DeleteAsync(site, channelId, adminId);
                }
            }
            if (isDeleteTemplates)
            {
                var summaries = await databaseManager.TemplateRepository.GetSummariesAsync(site.Id);
                foreach (var summary in summaries)
                {
                    if (summary.DefaultTemplate == false)
                    {
                        await databaseManager.TemplateRepository.DeleteAsync(pathManager, site, summary.Id);
                    }
                }
            }
            if (isDeleteFiles)
            {
                await pathManager.DeleteSiteFilesAsync(site);
            }

            //导入文件
            await importObject.ImportFilesAsync(siteTemplatePath, isOverride, guid);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.SiteTemplates.FileTemplate);
            await importObject.ImportTemplatesAsync(templateFilePath, isOverride, adminId, guid);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.SiteTemplates.Table);

            //导入站点设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.SiteTemplates.FileConfiguration);
            await importObject.ImportConfigurationAsync(configurationFilePath, guid);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteFiles.SiteTemplates.SiteContent);
            await importObject.ImportChannelsAndContentsAsync(0, siteContentDirectoryPath, isOverride, guid);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                await importObject.ImportTableStylesAsync(tableDirectoryPath, guid);
            }

            cacheManager.Clear();
        }
    }
}
