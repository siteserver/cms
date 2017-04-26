using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport
{
    public class SiteTemplateManager
    {
        private readonly string _rootPath;
        private SiteTemplateManager(string rootPath)
        {
            _rootPath = rootPath;
            DirectoryUtils.CreateDirectoryIfNotExists(_rootPath);
        }

        public static SiteTemplateManager GetInstance(string rootPath)
        {
            return new SiteTemplateManager(rootPath);
        }

        public static SiteTemplateManager Instance => new SiteTemplateManager(PathUtility.GetSiteTemplatesPath(string.Empty));


        public void DeleteSiteTemplate(string siteTemplateDir)
        {
            var directoryPath = PathUtils.Combine(_rootPath, siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            var filePath = PathUtils.Combine(_rootPath, siteTemplateDir + ".zip");
            FileUtils.DeleteFileIfExists(filePath);
        }

        public bool IsSiteTemplateDirectoryExists(string siteTemplateDir)
        {
            var siteTemplatePath = PathUtils.Combine(_rootPath, siteTemplateDir);
            return DirectoryUtils.IsDirectoryExists(siteTemplatePath);
        }

        public int GetSiteTemplateCount()
        {
            var directorys = DirectoryUtils.GetDirectoryPaths(_rootPath);
            return directorys.Length;
        }

        public List<string> GetDirectoryNameLowerList()
        {
            var directorys = DirectoryUtils.GetDirectoryNames(_rootPath);
            var list = new List<string>();
            foreach (var directoryName in directorys)
            {
                list.Add(directoryName.ToLower().Trim());
            }
            return list;
        }

        public SortedList GetSiteTemplateSortedList()
        {
            var sortedlist = new SortedList();
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
            foreach (var siteTemplatePath in directoryPaths)
            {
                var metadataXmlFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileMetadata);
                if (FileUtils.IsFileExists(metadataXmlFilePath))
                {
                    var siteTemplateInfo = Serializer.ConvertFileToObject(metadataXmlFilePath, typeof(SiteTemplateInfo)) as SiteTemplateInfo;
                    if (siteTemplateInfo != null)
                    {
                        var directoryName = PathUtils.GetDirectoryName(siteTemplatePath);
                        sortedlist.Add(directoryName, siteTemplateInfo);
                    }
                }
            }
            return sortedlist;
        }

        public void ImportSiteTemplateToEmptyPublishmentSystem(int publishmentSystemId, string siteTemplateDir, bool isUseTables, bool isImportContents, bool isImportTableStyles, string administratorName)
        {
            var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);
            if (DirectoryUtils.IsDirectoryExists(siteTemplatePath))
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

                var templateFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
                var tableDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
                var menuDisplayFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileMenuDisplay);
                var tagStyleFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTagStyle);
                var adFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileAd);
                var seoFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileSeo);
                var stlTagPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileStlTag);
                var gatherRuleFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileGatherRule);
                var inputDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Input);
                var configurationFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
                var siteContentDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);
                var contentModelPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath,DirectoryUtils.SiteTemplates.FileContentModel);

                var importObject = new ImportObject(publishmentSystemId);

                importObject.ImportFiles(siteTemplatePath, true);

                importObject.ImportTemplates(templateFilePath, true, administratorName);

                importObject.ImportAuxiliaryTables(tableDirectoryPath, isUseTables);

                importObject.ImportMenuDisplay(menuDisplayFilePath, true);

                importObject.ImportTagStyle(tagStyleFilePath, true);

                importObject.ImportAd(adFilePath, true);

                importObject.ImportSeo(seoFilePath, true);

                importObject.ImportStlTag(stlTagPath, true);

                importObject.ImportGatherRule(gatherRuleFilePath, true);

                importObject.ImportInput(inputDirectoryPath, true);

                importObject.ImportConfiguration(configurationFilePath);

                importObject.ImportContentModel(contentModelPath, true);

                var filePathArrayList = ImportObject.GetSiteContentFilePathArrayList(siteContentDirectoryPath);

                foreach (string filePath in filePathArrayList)
                {
                    importObject.ImportSiteContent(siteContentDirectoryPath, filePath, isImportContents);
                }

                DataProvider.NodeDao.UpdateContentNum(publishmentSystemInfo);

                if (isImportTableStyles)
                {
                    importObject.ImportTableStyles(tableDirectoryPath);
                }

                importObject.RemoveDbCache();
            }
        }

        public static void ExportPublishmentSystemToSiteTemplate(PublishmentSystemInfo publishmentSystemInfo, string siteTemplateDir)
        {
            var exportObject = new ExportObject(publishmentSystemInfo.PublishmentSystemId);

            var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);

            //导出模板
            var templateFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
            exportObject.ExportTemplates(templateFilePath);
            //导出辅助表及样式
            var tableDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
            exportObject.ExportTablesAndStyles(tableDirectoryPath);
            //导出下拉菜单
            var menuDisplayFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileMenuDisplay);
            exportObject.ExportMenuDisplay(menuDisplayFilePath);
            //导出模板标签样式
            var tagStyleFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTagStyle);
            exportObject.ExportTagStyle(tagStyleFilePath);
            //导出广告
            var adFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileAd);
            exportObject.ExportAd(adFilePath);
            //导出采集规则
            var gatherRuleFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileGatherRule);
            exportObject.ExportGatherRule(gatherRuleFilePath);
            //导出提交表单
            var inputDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Input);
            exportObject.ExportInput(inputDirectoryPath);
            //导出站点属性以及站点属性表单
            var configurationFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
            exportObject.ExportConfiguration(configurationFilePath);
            //导出SEO
            var seoFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileSeo);
            exportObject.ExportSeo(seoFilePath);
            //导出自定义模板语言
            var stlTagFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileStlTag);
            exportObject.ExportStlTag(stlTagFilePath);
            //导出关联字段
            var relatedFieldDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.RelatedField);
            exportObject.ExportRelatedField(relatedFieldDirectoryPath);
            //导出内容模型（自定义添加的）
            var contentModelDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileContentModel);
            exportObject.ExportContentModel(contentModelDirectoryPath);
        }
    }
}
