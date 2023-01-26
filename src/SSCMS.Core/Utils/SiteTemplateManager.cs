using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Serialization;
using SSCMS.Core.Utils.Serialization.Components;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class SiteTemplateManager
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly string _rootPath;

        public SiteTemplateManager(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _rootPath = _pathManager.GetSiteTemplatesPath(string.Empty);
            DirectoryUtils.CreateDirectoryIfNotExists(_rootPath);
        }

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

        public List<SiteTemplate> GetSiteTemplates()
        {
            var siteTemplates = new List<SiteTemplate>();
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
            foreach (var siteTemplatePath in directoryPaths)
            {
                var directoryName = PathUtils.GetDirectoryName(siteTemplatePath, false);
                SiteTemplate siteTemplate = null;
                var metadataXmlFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.FileMetadata);
                if (FileUtils.IsFileExists(metadataXmlFilePath))
                {
                    siteTemplate = XmlUtils.ConvertFileToObject<SiteTemplate>(metadataXmlFilePath);
                }

                if (siteTemplate == null)
                {
                    siteTemplate = new SiteTemplate
                    {
                        SiteTemplateName = directoryName
                    };
                }

                siteTemplate.DirectoryName = directoryName;
                siteTemplates.Add(siteTemplate);
            }

            return siteTemplates.OrderBy(x => x.DirectoryName).ToList();
        }

        public List<string> GetZipSiteTemplateList()
        {
            var list = new List<string>();
            foreach (var fileName in DirectoryUtils.GetFileNames(_rootPath))
            {
                if (FileUtils.IsZip(PathUtils.GetExtension(fileName)))
                {
                    list.Add(fileName);
                }
            }
            return list;
        }

        public async Task ImportSiteTemplateToEmptySiteAsync(Site site, string siteTemplateDir, bool isImportContents, bool isImportTableStyles, int adminId, string guid)
        {
            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
            if (!DirectoryUtils.IsDirectoryExists(siteTemplatePath)) return;

            var templateFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.FileTemplate);
            var tableDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.Table);
            var configurationFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.FileConfiguration);
            var siteContentDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.SiteContent);
            var formDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.Form);

            var importObject = new ImportObject(_pathManager, _databaseManager, _caching, site, adminId);

            _caching.SetProcess(guid, $"导入站点文件: {siteTemplatePath}");
            await importObject.ImportFilesAsync(siteTemplatePath, true, guid);

            _caching.SetProcess(guid, $"导入模板文件: {templateFilePath}");
            await importObject.ImportTemplatesAsync(templateFilePath, true, adminId, guid);

            var filePathList = ImportObject.GetSiteContentFilePathList(siteContentDirectoryPath);
            foreach (var filePath in filePathList)
            {
                _caching.SetProcess(guid, $"导入栏目文件: {filePath}");
                await importObject.ImportSiteContentAsync(siteContentDirectoryPath, filePath, isImportContents, guid);
            }

            var channels = await _databaseManager.ChannelRepository.GetChannelsAsync(site.Id);
            foreach (var channel in channels)
            {
                var contentIds = await _databaseManager.ContentRepository.GetContentIdsByLinkTypeAsync(site, channel, LinkType.LinkToChannel);
                foreach (var contentId in contentIds)
                {
                    var content = await _databaseManager.ContentRepository.GetAsync(site, channel, contentId);
                    var linkToChannelName = content.Get<string>(ContentIe.LinkToChannelName);
                    if (!string.IsNullOrEmpty(linkToChannelName))
                    {
                        var linkToChannel = channels.FirstOrDefault(x => x.ChannelName == linkToChannelName);
                        if (linkToChannel != null)
                        {
                            content.LinkUrl = ListUtils.ToString(linkToChannel.ParentsPath) + "," + linkToChannel.Id;
                            await _databaseManager.ContentRepository.UpdateAsync(site, channel, content);
                        }
                    }
                }
            }

            if (isImportTableStyles)
            {
                _caching.SetProcess(guid, $"导入表字段: {tableDirectoryPath}");
                await importObject.ImportTableStylesAsync(tableDirectoryPath, guid);
            }

            _caching.SetProcess(guid, $"导入表单: {formDirectoryPath}");
            await importObject.ImportFormsAsync(formDirectoryPath, guid);

            _caching.SetProcess(guid, $"导入配置文件: {configurationFilePath}");
            await importObject.ImportConfigurationAsync(configurationFilePath, guid);
        }

        public static async Task ExportSiteToSiteTemplateAsync(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string siteTemplateDir)
        {
            var exportObject = new ExportObject(pathManager, databaseManager, caching, site);

            var siteTemplatePath = pathManager.GetSiteTemplatesPath(siteTemplateDir);

            //导出模板
            var templateFilePath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.FileTemplate);
            await exportObject.ExportTemplatesAsync(templateFilePath);
            //导出辅助表及样式
            var tableDirectoryPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.Table);
            await exportObject.ExportTablesAndStylesAsync(tableDirectoryPath);
            //导出站点属性以及站点属性表单
            var configurationFilePath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.FileConfiguration);
            await exportObject.ExportConfigurationAsync(configurationFilePath);
            //导出关联字段
            var relatedFieldDirectoryPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.RelatedField);
            await exportObject.ExportRelatedFieldAsync(relatedFieldDirectoryPath);
            //导出表单
            var formDirectoryPath = pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.Form);
            await exportObject.ExportFormsAsync(formDirectoryPath);
        }
    }
}
