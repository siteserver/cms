using System.Collections;
using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.ImportExport
{
	public class BackupUtility
	{
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";
        public const string UploadFolderName = "upload"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件夹目录
        public const string UploadFileName = "upload.xml"; // 用于栏目及内容备份时记录图片、视频、文件上传所在文件名

        public static void BackupTemplates(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);
            exportObject.ExportTemplates(filePath);
        }

        public static void BackupChannelsAndContents(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);

            var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, siteId), EScopeType.Children, string.Empty, string.Empty, string.Empty);

            exportObject.ExportChannels(channelIdList, filePath);  
        }

        public static void BackupFiles(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);

            exportObject.ExportFiles(filePath);
        }


        public static void BackupSite(int siteId, string filePath, string adminName)
        {
            var exportObject = new ExportObject(siteId, adminName);
            var siteInfo = SiteManager.GetSiteInfo(siteId);

            var siteTemplateDir = PathUtils.GetFileNameWithoutExtension(filePath);
            var siteTemplatePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
            FileUtils.DeleteFileIfExists(filePath);
            var metadataPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);

            exportObject.ExportFilesToSite(siteTemplatePath, true, new ArrayList(), true);

            var siteContentDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            exportObject.ExportSiteContent(siteContentDirectoryPath, true, true, new List<int>());

            var templateFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            exportObject.ExportTemplates(templateFilePath);
            var tableDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.Table);
            exportObject.ExportTablesAndStyles(tableDirectoryPath);
            var configurationFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            exportObject.ExportConfiguration(configurationFilePath);
            exportObject.ExportMetadata(siteInfo.SiteName, siteInfo.Additional.WebUrl, string.Empty, string.Empty, metadataPath);

            ZipUtils.CreateZip(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static void RecoverySite(int siteId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string administratorName)
        {
            var importObject = new ImportObject(siteId, administratorName);

            var siteInfo = SiteManager.GetSiteInfo(siteId);

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
                var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, siteId), EScopeType.Children, string.Empty, string.Empty, string.Empty);
                foreach (var channelId in channelIdList)
                {
                    DataProvider.ChannelDao.Delete(siteId, channelId);
                }
            }
            if (isDeleteTemplates)
            {
                var templateInfoList =
                    DataProvider.TemplateDao.GetTemplateInfoListBySiteId(siteId);
                foreach (var templateInfo in templateInfoList)
                {
                    if (templateInfo.IsDefault == false)
                    {
                        DataProvider.TemplateDao.Delete(siteId, templateInfo.Id);
                    }
                }
            }
            if (isDeleteFiles)
            {
                DirectoryUtility.DeleteSiteFiles(siteInfo);
            }

            //导入文件
            importObject.ImportFiles(siteTemplatePath, isOverride);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            importObject.ImportTemplates(templateFilePath, isOverride, administratorName);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.Table);

            //导入站点设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            importObject.ImportConfiguration(configurationFilePath);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            importObject.ImportChannelsAndContents(0, siteContentDirectoryPath, isOverride);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                importObject.ImportTableStyles(tableDirectoryPath);
            }
            importObject.RemoveDbCache();

            CacheUtils.ClearAll();
        }
    }
}
