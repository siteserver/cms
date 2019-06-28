using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Enums;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Serialization
{
    public class BackupUtility
    {
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";
        public const string UploadFolderName = "upload"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件夹目录
        public const string UploadFileName = "upload.xml"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件名

        public static async Task BackupTemplatesAsync(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);
            await exportObject.ExportTemplatesAsync(filePath);
        }

        public static async Task BackupChannelsAndContentsAsync(IChannelRepository channelRepository, int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);

            var channelIdList = await channelRepository.GetChannelIdListAsync(await channelRepository.GetChannelInfoAsync(siteId, siteId), ScopeType.Children, string.Empty, string.Empty, string.Empty);

            await exportObject.ExportChannelsAsync(channelIdList, filePath);
        }

        public static void BackupFiles(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);

            exportObject.ExportFiles(filePath);
        }


        public static async Task BackupSiteAsync(IPathManager pathManager, IUrlManager urlManager, ISiteRepository siteRepository, int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);
            var siteInfo = siteRepository.GetSiteInfo(siteId);

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var siteTemplatePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
            FileUtils.DeleteFileIfExists(filePath);
            var metadataPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);

            exportObject.ExportFilesToSite(siteTemplatePath, true, new ArrayList(), true);

            var siteContentDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            await exportObject.ExportSiteContentAsync(siteContentDirectoryPath, true, true, new List<int>());

            var templateFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            var tableDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            var configurationFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            exportObject.ExportConfiguration(configurationFilePath);
            exportObject.ExportMetadata(siteInfo.SiteName, urlManager.GetWebUrl(siteInfo), string.Empty, string.Empty, metadataPath);

            ZipUtils.CreateZip(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static async Task RecoverySiteAsync(ICacheManager cacheManager, IPathManager pathManager, IFileManager fileManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository, int siteId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string administratorName)
        {
            var importObject = new ImportObject(siteId, administratorName);

            var siteInfo = siteRepository.GetSiteInfo(siteId);

            var siteTemplatePath = path;
            if (isZip)
            {
                //解压文件
                siteTemplatePath = pathManager.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.Site));
                DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

                ZipUtils.ExtractZip(path, siteTemplatePath);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);

            if (isDeleteChannels)
            {
                var channelIdList = await channelRepository.GetChannelIdListAsync(await channelRepository.GetChannelInfoAsync(siteId, siteId), ScopeType.Children, string.Empty, string.Empty, string.Empty);
                foreach (var channelId in channelIdList)
                {
                    await channelRepository.DeleteAsync(siteId, channelId);
                }
            }
            if (isDeleteTemplates)
            {
                var templateInfoList =
                    templateRepository.GetTemplateInfoListBySiteId(siteId);
                foreach (var templateInfo in templateInfoList)
                {
                    if (templateInfo.IsDefault == false)
                    {
                        await templateRepository.DeleteAsync(siteId, templateInfo.Id);
                    }
                }
            }
            if (isDeleteFiles)
            {
                fileManager.DeleteSiteFiles(siteInfo);
            }

            //导入文件
            importObject.ImportFiles(siteTemplatePath, isOverride);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            await importObject.ImportTemplatesAsync(templateFilePath, isOverride, administratorName);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.Table);

            //导入站点设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            importObject.ImportConfiguration(configurationFilePath);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            await importObject.ImportChannelsAndContentsAsync(0, siteContentDirectoryPath, isOverride);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                importObject.ImportTableStyles(tableDirectoryPath);
            }
            await importObject.RemoveDbCacheAsync();

            cacheManager.ClearAll();
        }
    }
}
