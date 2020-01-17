using System.Collections;
using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport.Components;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.ImportExport
{
    public class ExportObject
    {
        private readonly int _siteId;
        private readonly string _adminName;

        public ExportObject(int siteId, string adminName)
        {
            _siteId = siteId;
            _adminName = adminName;
        }

        public async Task ExportFilesToSiteAsync(string siteTemplatePath, bool isAllFiles, IList<string> directories, IList<string> files, bool isCreateMetadataDirectory)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

            var siteDirList = await DataProvider.SiteRepository.GetSiteDirListAsync(0);
            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir);
            var fileSystems = FileManager.GetFileSystemInfoExtendCollection(PathUtility.GetSitePath(site), true);
            foreach (FileSystemInfoExtend fileSystem in fileSystems)
            {
                var srcPath = PathUtils.Combine(sitePath, fileSystem.Name);
                var destPath = PathUtils.Combine(siteTemplatePath, fileSystem.Name);

                if (fileSystem.IsDirectory)
                {
                    if (!isAllFiles && !StringUtils.ContainsIgnoreCase(directories, fileSystem.Name)) continue;

                    var isSiteDirectory = false;

                    if (site.Root)
                    {
                        foreach (var siteDir in siteDirList)
                        {
                            if (StringUtils.EqualsIgnoreCase(siteDir, fileSystem.Name))
                            {
                                isSiteDirectory = true;
                            }
                        }
                    }
                    if (!isSiteDirectory && !WebUtils.IsSystemDirectory(fileSystem.Name))
                    {
                        DirectoryUtils.CreateDirectoryIfNotExists(destPath);
                        DirectoryUtils.MoveDirectory(srcPath, destPath, false);
                    }
                }
                else
                {
                    if (!isAllFiles && !StringUtils.ContainsIgnoreCase(files, fileSystem.Name)) continue;

                    if (!PathUtility.IsSystemFile(fileSystem.Name))
                    {
                        FileUtils.CopyFile(srcPath, destPath);
                    }
                }
            }

            if (isCreateMetadataDirectory)
            {
                var siteTemplateMetadataPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);
                DirectoryUtils.CreateDirectoryIfNotExists(siteTemplateMetadataPath);
            }
        }

        public async Task ExportFilesAsync(string filePath)
        {
            var filesDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
            FileUtils.DeleteFileIfExists(filePath);

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir);
            DirectoryUtils.Copy(sitePath, filesDirectoryPath);

            ZipUtils.CreateZip(filePath, filesDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(filesDirectoryPath);
        }

        public async Task<string> ExportSingleTableStyleAsync(string tableName, int relatedIdentity)
        {
            var filePath = PathUtils.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            await TableStyleIe.SingleExportTableStylesAsync(tableName, site.Id, relatedIdentity, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public static async Task<string> ExportRootSingleTableStyleAsync(string tableName, List<int> relatedIdentities)
        {
            var filePath = PathUtils.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");
            await TableStyleIe.SingleExportTableStylesAsync(tableName, relatedIdentities, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task ExportConfigurationAsync(string configurationFilePath)
        {
            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var configIe = new ConfigurationIe(site.Id, configurationFilePath);
            await configIe.ExportAsync();
        }

        /// <summary>
        /// 导出网站模板至指定的文件地址
        /// </summary>
        /// <param name="filePath"></param>
        public async Task ExportTemplatesAsync(string filePath)
        {
            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var templateIe = new TemplateIe(site.Id, filePath);
            await templateIe.ExportTemplatesAsync();
        }

        public async Task ExportRelatedFieldAsync(string relatedFieldDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(relatedFieldDirectoryPath);

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var relatedFieldIe = new RelatedFieldIe(site.Id, relatedFieldDirectoryPath);
            var relatedFieldInfoList = await DataProvider.RelatedFieldRepository.GetRelatedFieldListAsync(site.Id);
            foreach (var relatedFieldInfo in relatedFieldInfoList)
            {
                await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);
            }
        }

        public async Task<string> ExportRelatedFieldAsync(int relatedFieldId)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("relatedField");
            var filePath = PathUtils.GetTemporaryFilesPath("relatedField.zip");

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            var relatedFieldInfo = await DataProvider.RelatedFieldRepository.GetRelatedFieldAsync(relatedFieldId);

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);

            var relatedFieldIe = new RelatedFieldIe(site.Id, directoryPath);
            await relatedFieldIe.ExportRelatedFieldAsync(relatedFieldInfo);

            ZipUtils.CreateZip(filePath, directoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            return PathUtils.GetFileName(filePath);
        }


        // 导出网站所有相关辅助表以及除提交表单外的所有表样式
        public async Task ExportTablesAndStylesAsync(string tableDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(tableDirectoryPath);
            var styleIe = new TableStyleIe(tableDirectoryPath, _adminName);

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var tableNameList = await DataProvider.SiteRepository.GetTableNameListAsync(site);

            foreach (var tableName in tableNameList)
            {
                await styleIe.ExportTableStylesAsync(site.Id, tableName);
            }

            await styleIe.ExportTableStylesAsync(site.Id, DataProvider.ChannelRepository.TableName);
            await styleIe.ExportTableStylesAsync(site.Id, DataProvider.SiteRepository.TableName);
        }


        /// <summary>
        /// 导出网站内容至默认的临时文件地址
        /// </summary>
        public async Task ExportSiteContentAsync(string siteContentDirectoryPath, bool isSaveContents, bool isSaveAllChannels, IList<int> channelIdArrayList)
        {
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var allChannelIdList = await ChannelManager.GetChannelIdListAsync(_siteId);

            var includeChannelIdArrayList = new ArrayList();
            foreach (int channelId in channelIdArrayList)
            {
                var nodeInfo = await ChannelManager.GetChannelAsync(_siteId, channelId);
                var parentIdArrayList = StringUtils.GetIntList(nodeInfo.ParentsPath);
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

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);

            var siteIe = new SiteIe(site, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                if (!isSaveAllChannels)
                {
                    if (!includeChannelIdArrayList.Contains(channelId)) continue;
                }
                await siteIe.ExportAsync(_siteId, channelId, isSaveContents);
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
            var filePath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.ChannelsAndContents) + ".zip");
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
                    var nodeInfo = await ChannelManager.GetChannelAsync(_siteId, channelId);
                    var childChannelIdList = await ChannelManager.GetChannelIdListAsync(nodeInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
                    allChannelIdList.AddRange(childChannelIdList);
                }
            }

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);
            var sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir);
            var siteIe = new SiteIe(site, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                await siteIe.ExportAsync(_siteId, channelId, true);
            }

            var imageUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, site.ImageUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(imageUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(sitePath, site.ImageUploadDirectoryName), imageUploadDirectoryPath);

            var videoUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, site.VideoUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(videoUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(sitePath, site.VideoUploadDirectoryName), videoUploadDirectoryPath);

            var fileUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, site.FileUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(fileUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(sitePath, site.FileUploadDirectoryName), fileUploadDirectoryPath);

            AtomFeed feed = AtomUtility.GetEmptyFeed();  
            var entry = AtomUtility.GetEmptyEntry();  
            AtomUtility.AddDcElement(entry.AdditionalElements, "ImageUploadDirectoryName", site.ImageUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "VideoUploadDirectoryName", site.VideoUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "FileUploadDirectoryName", site.FileUploadDirectoryName);

            feed.Entries.Add(entry);
            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName); 
            DirectoryUtils.CreateDirectoryIfNotExists(uploadFolderPath);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName); 
            feed.Save(uploadFilePath);

            ZipUtils.CreateZip(filePath, siteContentDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public async Task<bool> ExportContentsAsync(string filePath, int channelId, List<int> contentIdArrayList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);

            var contentIe = new ContentIe(site, siteContentDirectoryPath);
            var isExport = await contentIe.ExportContentsAsync(site, channelId, contentIdArrayList, isPeriods, dateFrom, dateTo, checkedState);
            if (isExport)
            {
                ZipUtils.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }

        public async Task<bool> ExportContentsAsync(string filePath, List<Content> contentInfoList)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var site = await DataProvider.SiteRepository.GetAsync(_siteId);

            var contentIe = new ContentIe(site, siteContentDirectoryPath);
            var isExport = contentIe.ExportContents(site, contentInfoList);
            if (isExport)
            {
                ZipUtils.CreateZip(filePath, siteContentDirectoryPath);
                DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            }
            return isExport;
        }
    }
}
