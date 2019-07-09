using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.Serialization.Components;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;
using SS.CMS.Utils.IO;

namespace SS.CMS.Core.Serialization
{
    public class ExportObject
    {
        private SiteInfo _siteInfo;
        private string _sitePath;
        private int _userId;
        private ISettingsManager _settingsManager;
        private IPluginManager _pluginManager;
        private ICreateManager _createManager;
        private IPathManager _pathManager;
        private ITableManager _tableManager;
        private ISiteRepository _siteRepository;
        private IChannelRepository _channelRepository;
        private IRelatedFieldRepository _relatedFieldRepository;
        private IChannelGroupRepository _channelGroupRepository;
        private IContentGroupRepository _contentGroupRepository;
        private ISpecialRepository _specialRepository;
        private ITableStyleRepository _tableStyleRepository;
        private ITemplateRepository _templateRepository;

        public async Task LoadAsync(int siteId, int userId)
        {
            _siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            _sitePath = PathUtils.Combine(_settingsManager.WebRootPath, _siteInfo.SiteDir);
            _userId = userId;
        }

        /// <summary>
        /// 将发布系统文件保存到站点模板中
        /// </summary>
        public async Task ExportFilesToSiteAsync(string siteTemplatePath, bool isSaveAll, ArrayList lowerFileSystemArrayList, bool isCreateMetadataDirectory)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

            var siteDirList = await _siteRepository.GetLowerSiteDirListThatNotIsRootAsync();

            var fileSystems = FileSystemUtils.GetFileSystemInfoExtendCollection(_pathManager.GetSitePath(_siteInfo), true);
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                if (isSaveAll || lowerFileSystemArrayList.Contains(fileSystem.Name.ToLower()))
                {
                    var srcPath = PathUtils.Combine(_sitePath, fileSystem.Name);
                    var destPath = PathUtils.Combine(siteTemplatePath, fileSystem.Name);

                    if (fileSystem.IsDirectory)
                    {
                        var isSiteDirectory = false;

                        if (_siteInfo.IsRoot)
                        {
                            foreach (var siteDir in siteDirList)
                            {
                                if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
                                {
                                    isSiteDirectory = true;
                                }
                            }
                        }
                        if (!isSiteDirectory)
                        {
                            DirectoryUtils.CreateDirectoryIfNotExists(destPath);
                            DirectoryUtils.MoveDirectory(srcPath, destPath, false);
                        }
                    }
                    else
                    {
                        if (!_pathManager.IsSystemFile(fileSystem.Name))
                        {
                            FileUtils.CopyFile(srcPath, destPath);
                        }
                    }
                }
            }

            if (isCreateMetadataDirectory)
            {
                var siteTemplateMetadataPath = _pathManager.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplateMetadataPath);
            }
        }

        public void ExportFiles(string filePath)
        {
            var filesDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.Copy(_sitePath, filesDirectoryPath);

            ZipUtils.CreateZip(filePath, filesDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
        }

        public async Task<string> ExportSingleTableStyleAsync(string tableName, int relatedIdentity)
        {
            var filePath = _pathManager.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = _pathManager.GetTemporaryFilesPath("TableStyle");
            await TableStyleIe.SingleExportTableStylesAsync(_tableManager, _channelRepository, tableName, _siteInfo.Id, relatedIdentity, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public static async Task<string> ExportRootSingleTableStyleAsync(ITableManager tableManager, IPathManager pathManager, IChannelRepository channelRepository, string tableName)
        {
            var filePath = pathManager.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = pathManager.GetTemporaryFilesPath("TableStyle");
            await TableStyleIe.SingleExportTableStylesAsync(tableManager, channelRepository, tableName, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task ExportConfigurationAsync(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_siteInfo.Id, configurationFilePath);
            await configIe.ExportAsync();
        }

        /// <summary>
        /// 导出网站模板至指定的文件地址
        /// </summary>
        /// <param name="filePath"></param>
        public async Task ExportTemplatesAsync(string filePath)
        {
            var templateIe = new TemplateIe(_siteInfo.Id, filePath);
            await templateIe.ExportTemplatesAsync();
        }

        public async Task ExportTemplatesAsync(string filePath, List<int> templateIdList)
        {
            var templateIe = new TemplateIe(_siteInfo.Id, filePath);
            await templateIe.ExportTemplatesAsync(templateIdList);
        }

        public async Task ExportRelatedFieldAsync(string relatedFieldDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(relatedFieldDirectoryPath);

            var relatedFieldIe = new RelatedFieldIe(_siteInfo.Id, relatedFieldDirectoryPath);
            var relatedFieldInfoList = await _relatedFieldRepository.GetRelatedFieldInfoListAsync(_siteInfo.Id);
            foreach (var relatedFieldInfo in relatedFieldInfoList)
            {
                await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);
            }
        }

        public async Task<string> ExportRelatedFieldAsync(int relatedFieldId)
        {
            var directoryPath = _pathManager.GetTemporaryFilesPath("relatedField");
            var filePath = _pathManager.GetTemporaryFilesPath("relatedField.zip");

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            var relatedFieldInfo = await _relatedFieldRepository.GetRelatedFieldInfoAsync(relatedFieldId);

            var relatedFieldIe = new RelatedFieldIe(_siteInfo.Id, directoryPath);
            await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);

            ZipUtils.CreateZip(filePath, directoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            return PathUtils.GetFileName(filePath);
        }


        // 导出网站所有相关辅助表以及除提交表单外的所有表样式
        public async Task ExportTablesAndStylesAsync(string tableDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(tableDirectoryPath);
            var styleIe = new TableStyleIe(tableDirectoryPath, _userId);

            var siteInfo = await _siteRepository.GetSiteInfoAsync(_siteInfo.Id);
            var tableNameList = await _siteRepository.GetTableNameListAsync(_pluginManager, siteInfo);

            foreach (var tableName in tableNameList)
            {
                await styleIe.ExportTableStylesAsync(siteInfo.Id, tableName);
            }

            await styleIe.ExportTableStylesAsync(siteInfo.Id, _channelRepository.TableName);
            await styleIe.ExportTableStylesAsync(siteInfo.Id, _siteRepository.TableName);
        }


        /// <summary>
        /// 导出网站内容至默认的临时文件地址
        /// </summary>
        public async Task ExportSiteContentAsync(string siteContentDirectoryPath, bool isSaveContents, bool isSaveAllChannels, List<int> channelIdArrayList)
        {
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var allChannelIdList = await _channelRepository.GetChannelIdListAsync(_siteInfo.Id);

            var includeChannelIdArrayList = new ArrayList();
            foreach (int channelId in channelIdArrayList)
            {
                var nodeInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                var parentIdArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.ParentsPath);
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

            var siteIe = new SiteIe(_siteInfo, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                if (!isSaveAllChannels)
                {
                    if (!includeChannelIdArrayList.Contains(channelId)) continue;
                }
                await siteIe.ExportAsync(_siteInfo.Id, channelId, isSaveContents);
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
            Serializer.SaveAsXML(siteTemplateInfo, xmlPath);
        }


        public async Task<string> ExportChannelsAsync(List<int> channelIdList)
        {
            var filePath = _pathManager.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.ChannelsAndContents) + ".zip");
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
                    var nodeInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                    var childChannelIdList = await _channelRepository.GetChannelIdListAsync(nodeInfo, ScopeType.Descendant, string.Empty, string.Empty, string.Empty);
                    allChannelIdList.AddRange(childChannelIdList);
                }
            }

            var siteIe = new SiteIe(_siteInfo, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                await siteIe.ExportAsync(_siteInfo.Id, channelId, true);
            }

            var imageUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _siteInfo.ImageUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(imageUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(_sitePath, _siteInfo.ImageUploadDirectoryName), imageUploadDirectoryPath);

            var videoUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _siteInfo.VideoUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(videoUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(_sitePath, _siteInfo.VideoUploadDirectoryName), videoUploadDirectoryPath);

            var fileUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _siteInfo.FileUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(fileUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(_sitePath, _siteInfo.FileUploadDirectoryName), fileUploadDirectoryPath);

            AtomFeed feed = AtomUtility.GetEmptyFeed();
            var entry = AtomUtility.GetEmptyEntry();
            AtomUtility.AddDcElement(entry.AdditionalElements, "ImageUploadDirectoryName", _siteInfo.ImageUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "VideoUploadDirectoryName", _siteInfo.VideoUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "FileUploadDirectoryName", _siteInfo.FileUploadDirectoryName);

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

            var contentIe = new ContentIe(_siteInfo, siteContentDirectoryPath);
            var isExport = await contentIe.ExportContentsAsync(_siteInfo, channelId, contentIdArrayList, isPeriods, dateFrom, dateTo, checkedState);
            if (isExport)
            {
                ZipUtils.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }

        public bool ExportContents(string filePath, List<ContentInfo> contentInfoList)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var contentIe = new ContentIe(_siteInfo, siteContentDirectoryPath);
            var isExport = contentIe.ExportContents(_siteInfo, contentInfoList);
            if (isExport)
            {
                ZipUtils.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }
    }
}
