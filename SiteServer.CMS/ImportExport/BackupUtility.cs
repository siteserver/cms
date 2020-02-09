using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.ImportExport
{
	public static class BackupUtility
	{
        public const string UploadFolderName = "upload"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件夹目录
        public const string UploadFileName = "upload.xml"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件名

        public static async Task BackupTemplatesAsync(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);
            await exportObject.ExportTemplatesAsync(filePath);
        }

        public static async Task BackupChannelsAndContentsAsync(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);

            var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(await DataProvider.ChannelRepository.GetAsync(siteId), EScopeType.Children);

            await exportObject.ExportChannelsAsync(channelIdList, filePath);  
        }

        public static async Task BackupFilesAsync(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);

            await exportObject.ExportFilesAsync(filePath);
        }

        public static async Task BackupSiteAsync(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);
            var siteInfo = await DataProvider.SiteRepository.GetAsync(siteId);

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
            exportObject.ExportMetadata(siteInfo.SiteName, siteInfo.GetWebUrl(), string.Empty, string.Empty, metadataPath);

            ZipUtils.CreateZip(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static async Task RecoverySiteAsync(int siteId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string administratorName)
        {
            var importObject = new ImportObject(siteId, administratorName);

            var site = await DataProvider.SiteRepository.GetAsync(siteId);

            var siteTemplatePath = path;
            if (isZip)
            {
                //解压文件
                siteTemplatePath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.Site));
                DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

                ZipUtils.ExtractZip(path, siteTemplatePath);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);

            if (isDeleteChannels)
            {
                var channelIdList = await DataProvider.ChannelRepository.GetChannelIdsAsync(await DataProvider.ChannelRepository.GetAsync(siteId), EScopeType.Children);
                foreach (var channelId in channelIdList)
                {
                    await DataProvider.ChannelRepository.DeleteAsync(siteId, channelId);
                }
            }
            if (isDeleteTemplates)
            {
                var templateInfoList =
                    await DataProvider.TemplateRepository.GetAllAsync(siteId);
                foreach (var templateInfo in templateInfoList)
                {
                    if (templateInfo.Default == false)
                    {
                        await DataProvider.TemplateRepository.DeleteAsync(site, templateInfo.Id);
                    }
                }
            }
            if (isDeleteFiles)
            {
                await DirectoryUtility.DeleteSiteFilesAsync(site);
            }

            //导入文件
            await importObject.ImportFilesAsync(siteTemplatePath, isOverride);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            await importObject.ImportTemplatesAsync(templateFilePath, isOverride, administratorName);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.Table);

            //导入站点设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            await importObject.ImportConfigurationAsync(configurationFilePath);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            await importObject.ImportChannelsAndContentsAsync(0, siteContentDirectoryPath, isOverride);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                await importObject.ImportTableStylesAsync(tableDirectoryPath);
            }
            await importObject.RemoveDbCacheAsync();

            CacheUtils.ClearAll();
        }
    }
}
