using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Core.Utils.Serialization.Components;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using FileUtils = SSCMS.Utils.FileUtils;

namespace SSCMS.Core.Utils.Serialization
{
    public class ExportObject
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly Site _site;

        public ExportObject(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _site = site;
        }

        public async Task ExportFilesToSiteAsync(string siteTemplatePath, bool isAllFiles, IList<string> directories, IList<string> files, bool isCreateMetadataDirectory)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

            var siteDirList = await _databaseManager.SiteRepository.GetSiteDirsAsync(0);
            var sitePath = await _pathManager.GetSitePathAsync(_site);

            var directoryNames = DirectoryUtils.GetDirectoryNames(sitePath);
            var fileNames = DirectoryUtils.GetFileNames(sitePath);
            foreach (var directoryName in directoryNames)
            {
                var srcPath = PathUtils.Combine(sitePath, directoryName);
                var destPath = PathUtils.Combine(siteTemplatePath, directoryName);

                if (StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.Site.Template)) continue;
                if (!isAllFiles && !ListUtils.ContainsIgnoreCase(directories, directoryName)) continue;

                var isSiteDirectory = false;

                if (_site.Root)
                {
                    foreach (var siteDir in siteDirList)
                    {
                        if (StringUtils.EqualsIgnoreCase(siteDir, directoryName))
                        {
                            isSiteDirectory = true;
                        }
                    }
                }
                if (!isSiteDirectory && !_pathManager.IsSystemDirectory(directoryName))
                {
                    DirectoryUtils.CreateDirectoryIfNotExists(destPath);
                    DirectoryUtils.MoveDirectory(srcPath, destPath, false);
                }
            }

            var templateFileNames = await _databaseManager.TemplateRepository.GetRelatedFileNamesAsync(_site.Id, TemplateType.IndexPageTemplate);
            templateFileNames.AddRange(await _databaseManager.TemplateRepository.GetRelatedFileNamesAsync(_site.Id, TemplateType.IndexPageTemplate));

            foreach (var fileName in fileNames)
            {
                var srcPath = PathUtils.Combine(sitePath, fileName);
                var destPath = PathUtils.Combine(siteTemplatePath, fileName);

                if (ListUtils.ContainsIgnoreCase(templateFileNames, fileName)) continue;
                if (!isAllFiles && !ListUtils.ContainsIgnoreCase(files, fileName)) continue;

                FileUtils.CopyFile(srcPath, destPath);
            }

            //var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(await _pathManager.GetSitePathAsync(_site));
            //foreach (FileSystemInfoExtend fileSystem in fileSystems)
            //{
            //    var srcPath = PathUtils.Combine(sitePath, fileSystem.Name);
            //    var destPath = PathUtils.Combine(siteTemplatePath, fileSystem.Name);

            //    if (fileSystem.IsDirectory)
            //    {
            //        if (!isAllFiles && !StringUtils.ContainsIgnoreCase(directories, fileSystem.Name)) continue;

            //        var isSiteDirectory = false;

            //        if (_site.Root)
            //        {
            //            foreach (var siteDir in siteDirList)
            //            {
            //                if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
            //                {
            //                    isSiteDirectory = true;
            //                }
            //            }
            //        }
            //        if (!isSiteDirectory && !_pathManager.IsSystemDirectory(fileSystem.Name))
            //        {
            //            DirectoryUtils.CreateDirectoryIfNotExists(destPath);
            //            DirectoryUtils.MoveDirectory(srcPath, destPath, false);
            //        }
            //    }
            //    else
            //    {
            //        if (!isAllFiles && !StringUtils.ContainsIgnoreCase(files, fileSystem.Name)) continue;

            //        FileUtils.CopyFile(srcPath, destPath);
            //    }
            //}

            if (isCreateMetadataDirectory)
            {
                var siteTemplateMetadataPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplateMetadataPath);
            }
        }

        public async Task ExportFilesAsync(string filePath)
        {
            var filesDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
            FileUtils.DeleteFileIfExists(filePath);

            var sitePath = await _pathManager.GetSitePathAsync(_site);
            DirectoryUtils.Copy(sitePath, filesDirectoryPath);

            _pathManager.CreateZip(filePath, filesDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
        }

        public static async Task<string> ExportRootSingleTableStyleAsync(IPathManager pathManager, IDatabaseManager databaseManager, int siteId, string tableName, List<int> relatedIdentities)
        {
            var filePath = pathManager.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = pathManager.GetTemporaryFilesPath("TableStyle");
            await TableStyleIe.SingleExportTableStylesAsync(databaseManager, siteId, tableName, relatedIdentities, styleDirectoryPath);
            pathManager.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task ExportConfigurationAsync(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_databaseManager, _caching, _site, configurationFilePath);
            await configIe.ExportAsync();
        }

        public async Task ExportTemplatesAsync(string filePath)
        {
            var templateIe = new TemplateIe(_pathManager, _databaseManager, _caching, _site, filePath);
            await templateIe.ExportTemplatesAsync();
        }

        public async Task ExportRelatedFieldAsync(string relatedFieldDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(relatedFieldDirectoryPath);

            var relatedFieldIe = new RelatedFieldIe(_databaseManager, _site, relatedFieldDirectoryPath);
            var relatedFieldInfoList = await _databaseManager.RelatedFieldRepository.GetRelatedFieldsAsync(_site.Id);
            foreach (var relatedFieldInfo in relatedFieldInfoList)
            {
                await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);
            }
        }

        public static async Task<string> ExportRelatedFieldListAsync(IPathManager pathManager, IDatabaseManager databaseManager, int siteId)
        {
            var directoryPath = pathManager.GetTemporaryFilesPath("relatedField");
            var filePath = pathManager.GetTemporaryFilesPath("relatedField.zip");

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            var site = await databaseManager.SiteRepository.GetAsync(siteId);
            var relatedFieldInfoList = await databaseManager.RelatedFieldRepository.GetRelatedFieldsAsync(siteId);
            var relatedFieldIe = new RelatedFieldIe(databaseManager,  site, directoryPath);
            foreach (var relatedFieldInfo in relatedFieldInfoList)
            {
                await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);
            }

            pathManager.CreateZip(filePath, directoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task ExportTablesAndStylesAsync(string tableDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(tableDirectoryPath);
            var styleIe = new TableStyleIe(_databaseManager, _caching, tableDirectoryPath);

            var tableNameList = await _databaseManager.SiteRepository.GetTableNamesAsync(_site);

            foreach (var tableName in tableNameList)
            {
                await styleIe.ExportTableStylesAsync(_site.Id, true, tableName);
            }

            await styleIe.ExportTableStylesAsync(_site.Id, false, _databaseManager.ChannelRepository.TableName);
            await styleIe.ExportTableStylesAsync(_site.Id, false, _databaseManager.SiteRepository.TableName);
        }

        public async Task ExportSiteContentAsync(string siteContentDirectoryPath, bool isSaveContents, bool isSaveAllChannels, IList<int> channelIdArrayList)
        {
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var allChannelIdList = await _databaseManager.ChannelRepository.GetChannelIdsAsync(_site.Id);

            var includeChannelIdArrayList = new ArrayList();
            foreach (int channelId in channelIdArrayList)
            {
                var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                var parentIdArrayList = ListUtils.GetIntList(nodeInfo.ParentsPath);
                foreach (int parentId in parentIdArrayList)
                {
                    if (!includeChannelIdArrayList.Contains(parentId))
                    {
                        includeChannelIdArrayList.Add(parentId);
                    }
                }
                if (!includeChannelIdArrayList.Contains(channelId))
                {
                    includeChannelIdArrayList.Add(channelId);
                }
            }

            var siteIe = new SiteIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                if (!isSaveAllChannels)
                {
                    if (!includeChannelIdArrayList.Contains(channelId)) continue;
                }
                await siteIe.ExportAsync(_site, channelId, isSaveContents);
            }
        }

        public void ExportMetadata(string siteTemplateName, string webSiteUrl, string description, string samplePicPath, string metadataPath)
        {
            var siteTemplateInfo = new SiteTemplateInfo
            {
                SiteTemplateName = siteTemplateName,
                PicFileName = samplePicPath,
                WebSiteUrl = webSiteUrl,
                Description = description
            };

            var xmlPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteFiles.SiteTemplates.FileMetadata);
            XmlUtils.SaveAsXml(siteTemplateInfo, xmlPath);
        }

        public async Task<string> ExportChannelsAsync(List<int> channelIdList)
        {
            var filePath = _pathManager.GetTemporaryFilesPath(BackupType.ChannelsAndContents.GetValue() + ".zip");
            return await ExportChannelsAsync(channelIdList, filePath);
        }

        public async Task<string> ExportChannelsAsync(List<int> channelIdList, string filePath)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var allChannelIdList = new List<int>();
            foreach (var channelId in channelIdList)
            {
                if (!allChannelIdList.Contains(channelId))
                {
                    allChannelIdList.Add(channelId);
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    var childChannelIdList = await _databaseManager.ChannelRepository.GetChannelIdsAsync(nodeInfo.SiteId, nodeInfo.Id, ScopeType.Descendant);
                    allChannelIdList.AddRange(childChannelIdList);
                }
            }

            var sitePath = await _pathManager.GetSitePathAsync(_site);
            var siteIe = new SiteIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                await siteIe.ExportAsync(_site, channelId, true);
            }

            var imageUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _site.ImageUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(imageUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(sitePath, _site.ImageUploadDirectoryName), imageUploadDirectoryPath);

            var videoUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _site.VideoUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(videoUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(sitePath, _site.VideoUploadDirectoryName), videoUploadDirectoryPath);

            var fileUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _site.FileUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(fileUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(sitePath, _site.FileUploadDirectoryName), fileUploadDirectoryPath);

            AtomFeed feed = AtomUtility.GetEmptyFeed();  
            var entry = AtomUtility.GetEmptyEntry();  
            AtomUtility.AddDcElement(entry.AdditionalElements, "ImageUploadDirectoryName", _site.ImageUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "VideoUploadDirectoryName", _site.VideoUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "FileUploadDirectoryName", _site.FileUploadDirectoryName);

            feed.Entries.Add(entry);
            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName); 
            DirectoryUtils.CreateDirectoryIfNotExists(uploadFolderPath);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName); 
            feed.Save(uploadFilePath);

            _pathManager.CreateZip(filePath, siteContentDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task<bool> ExportContentsAsync(string filePath, List<Content> contentInfoList)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var contentIe = new ContentIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);
            var isExport = await contentIe.ExportContentsAsync(_site, contentInfoList);
            if (isExport)
            {
                _pathManager.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }
    }
}
