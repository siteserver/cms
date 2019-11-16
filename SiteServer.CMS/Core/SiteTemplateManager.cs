using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
    public class SiteTemplateManager
    {
        private readonly string _rootPath;
        private SiteTemplateManager(string rootPath)
        {
            _rootPath = rootPath;
            DirectoryUtils.CreateDirectoryIfNotExists(_rootPath);
        }

        public static SiteTemplateManager Instance => new SiteTemplateManager(PathUtility.GetSiteTemplatesPath(string.Empty));


        public void DeleteSiteTemplate(string siteTemplateDir)
        {
            var directoryPath = PathUtils.Combine(_rootPath, siteTemplateDir);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            var filePath = PathUtils.Combine(_rootPath, siteTemplateDir + ".zip");
            FileUtils.DeleteFileIfExists(filePath);
        }

        public void DeleteZipSiteTemplate(string fileName)
        {
            var filePath = PathUtils.Combine(_rootPath, fileName);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public bool IsSiteTemplateDirectoryExists(string siteTemplateDir)
        {
            var siteTemplatePath = PathUtils.Combine(_rootPath, siteTemplateDir);
            return DirectoryUtils.IsDirectoryExists(siteTemplatePath);
        }

        public SortedList GetSiteTemplateSortedList()
        {
            var sortedList = new SortedList();
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
            foreach (var siteTemplatePath in directoryPaths)
            {
                var directoryName = PathUtils.GetDirectoryName(siteTemplatePath, false);
                SiteTemplateInfo siteTemplateInfo = null;
                var metadataXmlFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileMetadata);
                if (FileUtils.IsFileExists(metadataXmlFilePath))
                {
                    siteTemplateInfo = Serializer.ConvertFileToObject<SiteTemplateInfo>(metadataXmlFilePath);
                }

                if (siteTemplateInfo == null)
                {
                    siteTemplateInfo = new SiteTemplateInfo
                    {
                        SiteTemplateName = directoryName
                    };
                }

                siteTemplateInfo.DirectoryName = directoryName;
                sortedList.Add(directoryName, siteTemplateInfo);
            }
            return sortedList;
        }

        public List<string> GetZipSiteTemplateList()
        {
            var list = new List<string>();
            foreach (var fileName in DirectoryUtils.GetFileNames(_rootPath))
            {
                if (EFileSystemTypeUtils.IsZip(PathUtils.GetExtension(fileName)))
                {
                    list.Add(fileName);
                }
            }
            return list;
        }

        public async Task ImportSiteTemplateToEmptySiteAsync(int siteId, string siteTemplateDir, bool isImportContents, bool isImportTableStyles, string administratorName)
        {
            var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);
            if (DirectoryUtils.IsDirectoryExists(siteTemplatePath))
            {
                var templateFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
                var tableDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
                var configurationFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
                var siteContentDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);

                var importObject = new ImportObject(siteId, administratorName);

                await importObject.ImportFilesAsync(siteTemplatePath, true);

                await importObject.ImportTemplatesAsync(templateFilePath, true, administratorName);

                await importObject.ImportConfigurationAsync(configurationFilePath);

                var filePathList = ImportObject.GetSiteContentFilePathList(siteContentDirectoryPath);

                foreach (var filePath in filePathList)
                {
                    await importObject.ImportSiteContentAsync(siteContentDirectoryPath, filePath, isImportContents);
                }

                if (isImportTableStyles)
                {
                    await importObject.ImportTableStylesAsync(tableDirectoryPath);
                }

                await importObject.RemoveDbCacheAsync();
            }
        }

        public static async Task ExportSiteToSiteTemplateAsync(Site site, string siteTemplateDir, string adminName)
        {
            var exportObject = new ExportObject(site.Id, adminName);

            var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);

            //导出模板
            var templateFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            //导出辅助表及样式
            var tableDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            //导出站点属性以及站点属性表单
            var configurationFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
            await exportObject.ExportConfigurationAsync(configurationFilePath);
            //导出关联字段
            var relatedFieldDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.RelatedField);
            await exportObject.ExportRelatedFieldAsync(relatedFieldDirectoryPath);
        }
    }
}
