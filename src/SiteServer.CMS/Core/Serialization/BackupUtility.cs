using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.CMS.Serialization
{
	public static class BackupUtility
	{
        public const string UploadFolderName = "upload"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件夹目录
        public const string UploadFileName = "upload.xml"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件名

        public static async Task BackupTemplatesAsync(Site site, string filePath, int adminId)
        {
            var exportObject = new ExportObject(site, adminId);
            await exportObject.ExportTemplatesAsync(filePath);
        }

        public static async Task BackupChannelsAndContentsAsync(Site site, string filePath, int adminId)
        {
            var exportObject = new ExportObject(site, adminId);

            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(site.Id, site.Id, ScopeType.Children);

            await exportObject.ExportChannelsAsync(channelIdList, filePath);  
        }

        public static async Task BackupFilesAsync(Site site, string filePath, int adminId)
        {
            var exportObject = new ExportObject(site, adminId);

            await exportObject.ExportFilesAsync(filePath);
        }

        public static async Task BackupSiteAsync(Site site, string filePath, int adminId)
        {
            var exportObject = new ExportObject(site, adminId);

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var siteTemplatePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
            FileUtils.DeleteFileIfExists(filePath);
            var metadataPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);

            await exportObject.ExportFilesToSiteAsync(siteTemplatePath, true, null, null, true);

            var siteContentDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, true, true, new List<int>());

            var templateFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            var tableDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            var configurationFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            await exportObject.ExportConfigurationAsync(configurationFilePath);
            exportObject.ExportMetadata(site.SiteName, await site.GetWebUrlAsync(), string.Empty, string.Empty, metadataPath);

            ZipUtils.CreateZip(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static async Task RecoverySiteAsync(Site site, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, int adminId, string guid)
        {
            var importObject = new ImportObject(site, adminId);

            var siteTemplatePath = path;
            if (isZip)
            {
                //解压文件
                siteTemplatePath = PathUtility.GetTemporaryFilesPath(BackupType.Site.GetValue());
                DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

                ZipUtils.ExtractZip(path, siteTemplatePath);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);

            if (isDeleteChannels)
            {
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(site.Id, site.Id, ScopeType.Children);
                foreach (var channelId in channelIdList)
                {
                    await DataProvider.ContentRepository.RecycleAllAsync(site, channelId, adminId);
                    await DataProvider.ChannelRepository.DeleteAsync(site, channelId, adminId);
                }
            }
            if (isDeleteTemplates)
            {
                var summaries = await DataProvider.TemplateRepository.GetSummariesAsync(site.Id);
                foreach (var summary in summaries)
                {
                    if (summary.Default == false)
                    {
                        await DataProvider.TemplateRepository.DeleteAsync(site, summary.Id);
                    }
                }
            }
            if (isDeleteFiles)
            {
                await DirectoryUtility.DeleteSiteFilesAsync(site);
            }

            //导入文件
            await importObject.ImportFilesAsync(siteTemplatePath, isOverride, guid);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            await importObject.ImportTemplatesAsync(templateFilePath, isOverride, adminId, guid);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.Table);

            //导入站点设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            await importObject.ImportConfigurationAsync(configurationFilePath, guid);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            await importObject.ImportChannelsAndContentsAsync(0, siteContentDirectoryPath, isOverride, guid);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                await importObject.ImportTableStylesAsync(tableDirectoryPath, guid);
            }
            await importObject.RemoveDbCacheAsync();

            CacheUtils.ClearAll();
        }
    }
}
