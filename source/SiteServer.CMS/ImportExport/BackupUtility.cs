using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.IO;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.ImportExport
{
	public class BackupUtility
	{
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";

        public static void BackupTemplates(int publishmentSystemId, string filePath)
        {
            var exportObject = new ExportObject(publishmentSystemId);
            exportObject.ExportTemplates(filePath);
        }

        public static void BackupChannelsAndContents(int publishmentSystemId, string filePath)
        {
            var exportObject = new ExportObject(publishmentSystemId);

            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(publishmentSystemId, publishmentSystemId);
            exportObject.ExportChannels(nodeIdList, filePath);
        }

        public static void BackupFiles(int publishmentSystemId, string filePath)
        {
            var exportObject = new ExportObject(publishmentSystemId);

            exportObject.ExportFiles(filePath);
        }


        public static void BackupSite(int publishmentSystemId, string filePath)
        {
            var exportObject = new ExportObject(publishmentSystemId);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

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
            var menuDisplayFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileMenuDisplay);
            exportObject.ExportMenuDisplay(menuDisplayFilePath);
            var tagStyleFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileTagStyle);
            exportObject.ExportTagStyle(tagStyleFilePath);
            var adFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileAd);
            exportObject.ExportAd(adFilePath);
            var gatherRuleFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileGatherRule);
            exportObject.ExportGatherRule(gatherRuleFilePath);
            var inputDirectoryPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.Input);
            exportObject.ExportInput(inputDirectoryPath);
            var configurationFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            exportObject.ExportConfiguration(configurationFilePath);
            var contentModelFilePath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileContentModel);
            exportObject.ExportContentModel(contentModelFilePath);
            exportObject.ExportMetadata(publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemUrl, string.Empty, string.Empty, metadataPath);

            ZipUtils.PackFiles(filePath, siteTemplatePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
        }

        public static void RecoverySite(int publishmentSystemId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string administratorName)
        {
            var importObject = new ImportObject(publishmentSystemId);

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var siteTemplatePath = path;
            if (isZip)
            {
                //解压文件
                siteTemplatePath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.Site));
                DirectoryUtils.DeleteDirectoryIfExists(siteTemplatePath);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

                ZipUtils.UnpackFiles(path, siteTemplatePath);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);

            if (isDeleteChannels)
            {
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(publishmentSystemId, publishmentSystemId);
                foreach (int nodeId in nodeIdList)
                {
                    DataProvider.NodeDao.Delete(nodeId);
                }
            }
            if (isDeleteTemplates)
            {
                var templateInfoArrayList =
                    DataProvider.TemplateDao.GetTemplateInfoArrayListByPublishmentSystemId(publishmentSystemId);
                foreach (TemplateInfo templateInfo in templateInfoArrayList)
                {
                    if (templateInfo.IsDefault == false)
                    {
                        DataProvider.TemplateDao.Delete(publishmentSystemId, templateInfo.TemplateId);
                    }
                }
            }
            if (isDeleteFiles)
            {
                DirectoryUtility.DeletePublishmentSystemFiles(publishmentSystemInfo);
            }

            //导入文件
            importObject.ImportFiles(siteTemplatePath, isOverride);

            //导入模板
            var templateFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileTemplate);
            importObject.ImportTemplates(templateFilePath, isOverride, administratorName);

            //导入辅助表
            var tableDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.Table);
            importObject.ImportAuxiliaryTables(tableDirectoryPath, isUseTable);

            //导入菜单
            var menuDisplayFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileMenuDisplay);
            importObject.ImportMenuDisplay(menuDisplayFilePath, isOverride);

            //导入标签样式
            var tagStyleFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileTagStyle);
            importObject.ImportTagStyle(tagStyleFilePath, isOverride);

            //导入固定广告
            var adFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileAd);
            importObject.ImportAd(adFilePath, isOverride);

            //导入采集规则
            var gatherRuleFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileGatherRule);
            importObject.ImportGatherRule(gatherRuleFilePath, isOverride);

            //导入提交表单
            var inputDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.Input);
            importObject.ImportInput(inputDirectoryPath, isOverride);

            //导入站点设置
            var configurationFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileConfiguration);
            importObject.ImportConfiguration(configurationFilePath);

            //导入内容模型
            var contentModelFilePath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.FileContentModel);
            importObject.ImportContentModel(contentModelFilePath, true);

            //导入栏目及内容
            var siteContentDirectoryPath = PathUtils.Combine(siteTemplateMetadataPath, DirectoryUtils.SiteTemplates.SiteContent);
            importObject.ImportChannelsAndContents(0, siteContentDirectoryPath, isOverride);

            DataProvider.NodeDao.UpdateContentNum(publishmentSystemInfo);

            //导入表样式及清除缓存
            if (isUseTable)
            {
                importObject.ImportTableStyles(tableDirectoryPath);
            }
            importObject.RemoveDbCache();

            CacheUtils.Clear();
        }
    }
}
