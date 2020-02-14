using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.ImportExport;

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

        public List<SiteTemplateInfo> GetSiteTemplateInfoList()
        {
            var siteTemplateInfoList = new List<SiteTemplateInfo>();
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
                siteTemplateInfoList.Add(siteTemplateInfo);
            }

            return siteTemplateInfoList.OrderBy(x => x.DirectoryName).ToList();
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

        public async Task ImportSiteTemplateToEmptySiteAsync(Site site, string siteTemplateDir, bool isImportContents, bool isImportTableStyles, int adminId, string guid)
        {
            var siteTemplatePath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);
            if (!DirectoryUtils.IsDirectoryExists(siteTemplatePath)) return;

            var templateFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
            var tableDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
            var configurationFilePath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
            var siteContentDirectoryPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);

            var importObject = new ImportObject(site, adminId);

            Caching.SetProcess(guid, $"导入站点文件: {siteTemplatePath}");
            await importObject.ImportFilesAsync(siteTemplatePath, true, guid);

            Caching.SetProcess(guid, $"导入模板文件: {templateFilePath}");
            await importObject.ImportTemplatesAsync(templateFilePath, true, adminId, guid);

            Caching.SetProcess(guid, $"导入配置文件: {configurationFilePath}");
            await importObject.ImportConfigurationAsync(configurationFilePath, guid);

            var filePathList = ImportObject.GetSiteContentFilePathList(siteContentDirectoryPath);
            foreach (var filePath in filePathList)
            {
                Caching.SetProcess(guid, $"导入栏目文件: {filePath}");
                await importObject.ImportSiteContentAsync(siteContentDirectoryPath, filePath, isImportContents, guid);
            }

            if (isImportTableStyles)
            {
                Caching.SetProcess(guid, $"导入表字段: {tableDirectoryPath}");
                await importObject.ImportTableStylesAsync(tableDirectoryPath, guid);
            }

            Caching.SetProcess(guid, "清除数据库缓存...");
            await importObject.RemoveDbCacheAsync();
        }

        public static async Task ExportSiteToSiteTemplateAsync(Site site, string siteTemplateDir, int adminId)
        {
            var exportObject = new ExportObject(site, adminId);

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
