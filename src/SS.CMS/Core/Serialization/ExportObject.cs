using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SS.CMS.Abstractions;
using SS.CMS.Core.Serialization.Atom.Atom.Core;
using SS.CMS.Core.Serialization.Components;
using FileUtils = SS.CMS.Abstractions.FileUtils;

namespace SS.CMS.Core.Serialization
{
    public class ExportObject
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly Site _site;

        public ExportObject(IPathManager pathManager, IDatabaseManager databaseManager, IPluginManager pluginManager, Site site)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _site = site;
        }

        public async Task ExportFilesToSiteAsync(string siteTemplatePath, bool isAllFiles, IList<string> directories, IList<string> files, bool isCreateMetadataDirectory)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

            var siteDirList = await _databaseManager.SiteRepository.GetSiteDirListAsync(0);
            var sitePath = await _pathManager.GetSitePathAsync(_site);
            var fileSystems = FileUtility.GetFileSystemInfoExtendCollection(await _pathManager.GetSitePathAsync(_site));
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                var srcPath = PathUtils.Combine(sitePath, fileSystem.Name);
                var destPath = PathUtils.Combine(siteTemplatePath, fileSystem.Name);

                if (fileSystem.IsDirectory)
                {
                    if (!isAllFiles && !StringUtils.ContainsIgnoreCase(directories, fileSystem.Name)) continue;

                    var isSiteDirectory = false;

                    if (_site.Root)
                    {
                        foreach (var siteDir in siteDirList)
                        {
                            if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
                            {
                                isSiteDirectory = true;
                            }
                        }
                    }
                    if (!isSiteDirectory && !_pathManager.IsSystemDirectory(fileSystem.Name))
                    {
                        DirectoryUtils.CreateDirectoryIfNotExists(destPath);
                        DirectoryUtils.MoveDirectory(srcPath, destPath, false);
                    }
                }
                else
                {
                    if (!isAllFiles && !StringUtils.ContainsIgnoreCase(files, fileSystem.Name)) continue;

                    if (!_pathManager.IsSystemFile(fileSystem.Name))
                    {
                        FileUtils.CopyFile(srcPath, destPath);
                    }
                }
            }

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

            ZipUtils.CreateZip(filePath, filesDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
        }

        public async Task<string> ExportSingleTableStyleAsync(string tableName, int relatedIdentity)
        {
            var filePath = _pathManager.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = _pathManager.GetTemporaryFilesPath("TableStyle");

            await TableStyleIe.SingleExportTableStylesAsync(_databaseManager, tableName, _site.Id, relatedIdentity, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public static async Task<string> ExportRootSingleTableStyleAsync(IPathManager pathManager, IDatabaseManager databaseManager, int siteId, string tableName, List<int> relatedIdentities)
        {
            var filePath = pathManager.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = pathManager.GetTemporaryFilesPath("TableStyle");
            await TableStyleIe.SingleExportTableStylesAsync(databaseManager, siteId, tableName, relatedIdentities, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task ExportConfigurationAsync(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_databaseManager, _site, configurationFilePath);
            await configIe.ExportAsync();
        }

        /// <summary>
        /// 导出网站模板至指定的文件地址
        /// </summary>
        /// <param name="filePath"></param>
        public async Task ExportTemplatesAsync(string filePath)
        {
            var templateIe = new TemplateIe(_pathManager, _databaseManager, _site, filePath);
            await templateIe.ExportTemplatesAsync();
        }

        public async Task ExportRelatedFieldAsync(string relatedFieldDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(relatedFieldDirectoryPath);

            var relatedFieldIe = new RelatedFieldIe(_databaseManager, _site, relatedFieldDirectoryPath);
            var relatedFieldInfoList = await _databaseManager.RelatedFieldRepository.GetRelatedFieldListAsync(_site.Id);
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
            var relatedFieldInfoList = await databaseManager.RelatedFieldRepository.GetRelatedFieldListAsync(siteId);
            var relatedFieldIe = new RelatedFieldIe(databaseManager,  site, directoryPath);
            foreach (var relatedFieldInfo in relatedFieldInfoList)
            {
                await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);
            }

            ZipUtils.CreateZip(filePath, directoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            return PathUtils.GetFileName(filePath);
        }


        // 导出网站所有相关辅助表以及除提交表单外的所有表样式
        public async Task ExportTablesAndStylesAsync(string tableDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(tableDirectoryPath);
            var styleIe = new TableStyleIe(_databaseManager, tableDirectoryPath);

            var tableNameList = await _databaseManager.SiteRepository.GetTableNamesAsync(_pluginManager, _site);

            foreach (var tableName in tableNameList)
            {
                await styleIe.ExportTableStylesAsync(_site.Id, tableName);
            }

            await styleIe.ExportTableStylesAsync(_site.Id, _databaseManager.ChannelRepository.TableName);
            await styleIe.ExportTableStylesAsync(_site.Id, _databaseManager.SiteRepository.TableName);
        }


        /// <summary>
        /// 导出网站内容至默认的临时文件地址
        /// </summary>
        public async Task ExportSiteContentAsync(string siteContentDirectoryPath, bool isSaveContents, bool isSaveAllChannels, IList<int> channelIdArrayList)
        {
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var allChannelIdList = await _databaseManager.ChannelRepository.GetChannelIdListAsync(_site.Id);

            var includeChannelIdArrayList = new ArrayList();
            foreach (int channelId in channelIdArrayList)
            {
                var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                var parentIdArrayList = Utilities.GetIntList(nodeInfo.ParentsPath);
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

            var siteIe = new SiteIe(_pathManager, _databaseManager, _site, siteContentDirectoryPath);
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

            var xmlPath = PathUtils.Combine(metadataPath, DirectoryUtils.SiteTemplates.FileMetadata);
            Serializer.SaveAsXml(siteTemplateInfo, xmlPath);
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
            var siteIe = new SiteIe(_pathManager, _databaseManager, _site, siteContentDirectoryPath);
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

            ZipUtils.CreateZip(filePath, siteContentDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task<bool> ExportContentsAsync(string filePath, int channelId, List<int> contentIdArrayList, bool isPeriods, string dateFrom, string dateTo, bool? checkedState)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var contentIe = new ContentIe(_pathManager, _databaseManager, _site, siteContentDirectoryPath);
            var isExport = await contentIe.ExportContentsAsync(_site, channelId, contentIdArrayList, isPeriods, dateFrom, dateTo, checkedState);
            if (isExport)
            {
                ZipUtils.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }

        public bool ExportContents(string filePath, List<Content> contentInfoList)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var contentIe = new ContentIe(_pathManager, _databaseManager, _site, siteContentDirectoryPath);
            var isExport = contentIe.ExportContents(_site, contentInfoList);
            if (isExport)
            {
                ZipUtils.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }
    }
}
