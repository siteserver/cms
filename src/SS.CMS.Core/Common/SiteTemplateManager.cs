using System.Collections;
using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Serialization;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Common
{
    public class SiteTemplateManager
    {
        private readonly string _rootPath;
        private readonly IPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly ITemplateRepository _templateRepository;

        private SiteTemplateManager(string rootPath)
        {
            _rootPath = rootPath;
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

        public bool IsSiteTemplateExists
        {
            get
            {
                var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
                foreach (var siteTemplatePath in directoryPaths)
                {
                    var metadataXmlFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileMetadata);
                    if (FileUtils.IsFileExists(metadataXmlFilePath))
                    {
                        var siteTemplateInfo = Serializer.ConvertFileToObject(metadataXmlFilePath, typeof(SiteTemplateInfo)) as SiteTemplateInfo;
                        if (siteTemplateInfo != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public SortedList GetSiteTemplateSortedList()
        {
            var sortedlist = new SortedList();
            var directoryPaths = DirectoryUtils.GetDirectoryPaths(_rootPath);
            foreach (var siteTemplatePath in directoryPaths)
            {
                var metadataXmlFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileMetadata);
                if (FileUtils.IsFileExists(metadataXmlFilePath))
                {
                    var siteTemplateInfo = Serializer.ConvertFileToObject(metadataXmlFilePath, typeof(SiteTemplateInfo)) as SiteTemplateInfo;
                    if (siteTemplateInfo != null)
                    {
                        var directoryName = PathUtils.GetDirectoryName(siteTemplatePath, false);
                        siteTemplateInfo.DirectoryName = directoryName;
                        sortedlist.Add(directoryName, siteTemplateInfo);
                    }
                }
            }
            return sortedlist;
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

        public void ImportSiteTemplateToEmptySite(int siteId, string siteTemplateDir, bool isImportContents, bool isImportTableStyles, string administratorName)
        {
            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);
            if (DirectoryUtils.IsDirectoryExists(siteTemplatePath))
            {
                var templateFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
                var tableDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
                var configurationFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
                var siteContentDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteContent);

                var importObject = new ImportObject(siteId, administratorName);

                importObject.ImportFiles(siteTemplatePath, true);

                importObject.ImportTemplates(templateFilePath, true, administratorName);

                importObject.ImportConfiguration(configurationFilePath);

                var filePathList = ImportObject.GetSiteContentFilePathList(siteContentDirectoryPath);

                foreach (var filePath in filePathList)
                {
                    importObject.ImportSiteContent(siteContentDirectoryPath, filePath, isImportContents);
                }

                if (isImportTableStyles)
                {
                    importObject.ImportTableStyles(tableDirectoryPath);
                }

                importObject.RemoveDbCache();
            }
        }

        public void ExportSiteToSiteTemplate(SiteInfo siteInfo, string siteTemplateDir, string adminName)
        {
            var exportObject = new ExportObject(siteInfo.Id, adminName);

            var siteTemplatePath = _pathManager.GetSiteTemplatesPath(siteTemplateDir);

            //导出模板
            var templateFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileTemplate);
            exportObject.ExportTemplates(templateFilePath);
            //导出辅助表及样式
            var tableDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.Table);
            exportObject.ExportTablesAndStyles(tableDirectoryPath);
            //导出站点属性以及站点属性表单
            var configurationFilePath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.FileConfiguration);
            exportObject.ExportConfiguration(configurationFilePath);
            //导出关联字段
            var relatedFieldDirectoryPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, DirectoryUtils.SiteTemplates.RelatedField);
            exportObject.ExportRelatedField(relatedFieldDirectoryPath);
        }
    }
}
