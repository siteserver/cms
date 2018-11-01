using System.Collections;
using System.Collections.Generic;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport.Components;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.IO;

namespace SiteServer.CMS.ImportExport
{
    public class ExportObject
    {
        private readonly SiteInfo _siteInfo;
        private readonly string _sitePath;
        private readonly string _adminName;

        public ExportObject(int siteId, string adminName)
        {
            _siteInfo = SiteManager.GetSiteInfo(siteId);
            _sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, _siteInfo.SiteDir);
            _adminName = adminName;
        }

        /// <summary>
        /// 将发布系统文件保存到站点模板中
        /// </summary>
        public void ExportFilesToSite(string siteTemplatePath, bool isSaveAll, ArrayList lowerFileSystemArrayList, bool isCreateMetadataDirectory)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(siteTemplatePath);

            var siteDirList = DataProvider.SiteDao.GetLowerSiteDirListThatNotIsRoot();

            var fileSystems = FileManager.GetFileSystemInfoExtendCollection(PathUtility.GetSitePath(_siteInfo), true);
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
                        if (!isSiteDirectory && !DirectoryUtils.IsSystemDirectory(fileSystem.Name))
                        {
                            DirectoryUtils.CreateDirectoryIfNotExists(destPath);
                            DirectoryUtils.MoveDirectory(srcPath, destPath, false);
                        }
                    }
                    else
                    {
                        if (!PathUtility.IsSystemFile(fileSystem.Name))
                        {
                            FileUtils.CopyFile(srcPath, destPath);
                        }
                    }
                }
            }

            if (isCreateMetadataDirectory)
            {
                var siteTemplateMetadataPath = PathUtility.GetSiteTemplateMetadataPath(siteTemplatePath, string.Empty);
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

        public string ExportSingleTableStyle(string tableName, int relatedIdentity)
        {
            var filePath = PathUtils.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");
            TableStyleIe.SingleExportTableStyles(tableName, _siteInfo.Id, relatedIdentity, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public static string ExportRootSingleTableStyle(string tableName)
        {
            var filePath = PathUtils.GetTemporaryFilesPath("tableStyle.zip");
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");
            TableStyleIe.SingleExportTableStyles(tableName, styleDirectoryPath);
            ZipUtils.CreateZip(filePath, styleDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public void ExportConfiguration(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_siteInfo.Id, configurationFilePath);
            configIe.Export();
        }

        /// <summary>
        /// 导出网站模板至指定的文件地址
        /// </summary>
        /// <param name="filePath"></param>
        public void ExportTemplates(string filePath)
        {
            var templateIe = new TemplateIe(_siteInfo.Id, filePath);
            templateIe.ExportTemplates();
        }

        public void ExportTemplates(string filePath, List<int> templateIdList)
        {
            var templateIe = new TemplateIe(_siteInfo.Id, filePath);
            templateIe.ExportTemplates(templateIdList);
        }

        public void ExportRelatedField(string relatedFieldDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(relatedFieldDirectoryPath);

            var relatedFieldIe = new RelatedFieldIe(_siteInfo.Id, relatedFieldDirectoryPath);
            var relatedFieldInfoList = DataProvider.RelatedFieldDao.GetRelatedFieldInfoList(_siteInfo.Id);
            foreach (var relatedFieldInfo in relatedFieldInfoList)
            {
                relatedFieldIe.ExportRelatedField(relatedFieldInfo);
            }
        }

        public string ExportRelatedField(int relatedFieldId)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("relatedField");
            var filePath = PathUtils.GetTemporaryFilesPath("relatedField.zip");

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            var relatedFieldInfo = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(relatedFieldId);

            var relatedFieldIe = new RelatedFieldIe(_siteInfo.Id, directoryPath);
            relatedFieldIe.ExportRelatedField(relatedFieldInfo);

            ZipUtils.CreateZip(filePath, directoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

            return PathUtils.GetFileName(filePath);
        }


        // 导出网站所有相关辅助表以及除提交表单外的所有表样式
        public void ExportTablesAndStyles(string tableDirectoryPath)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(tableDirectoryPath);
            var styleIe = new TableStyleIe(tableDirectoryPath, _adminName);

            var siteInfo = SiteManager.GetSiteInfo(_siteInfo.Id);
            var tableNameList = SiteManager.GetTableNameList(siteInfo);

            foreach (var tableName in tableNameList)
            {
                styleIe.ExportTableStyles(siteInfo.Id, tableName);
            }

            styleIe.ExportTableStyles(siteInfo.Id, DataProvider.ChannelDao.TableName);
            styleIe.ExportTableStyles(siteInfo.Id, DataProvider.SiteDao.TableName);
        }


        /// <summary>
        /// 导出网站内容至默认的临时文件地址
        /// </summary>
        public void ExportSiteContent(string siteContentDirectoryPath, bool isSaveContents, bool isSaveAllChannels, List<int> channelIdArrayList)
        {
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var allChannelIdList = ChannelManager.GetChannelIdList(_siteInfo.Id);

            var includeChannelIdArrayList = new ArrayList();
            foreach (int channelId in channelIdArrayList)
            {
                var nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, channelId);
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
                siteIe.Export(_siteInfo.Id, channelId, isSaveContents);
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


        public string ExportChannels(List<int> channelIdList)
        {
            var filePath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.ChannelsAndContents) + ".zip");
            return ExportChannels(channelIdList, filePath);
        }

        public string ExportChannels(List<int> channelIdList, string filePath)
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
                    var nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, channelId);
                    var childChannelIdList = ChannelManager.GetChannelIdList(nodeInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
                    allChannelIdList.AddRange(childChannelIdList);
                }
            }

            var siteIe = new SiteIe(_siteInfo, siteContentDirectoryPath);
            foreach (var channelId in allChannelIdList)
            {
                siteIe.Export(_siteInfo.Id, channelId, true);
            } 
             
            var imageUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _siteInfo.Additional.ImageUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(imageUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(_sitePath, _siteInfo.Additional.ImageUploadDirectoryName), imageUploadDirectoryPath);

            var videoUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _siteInfo.Additional.VideoUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(videoUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(_sitePath, _siteInfo.Additional.VideoUploadDirectoryName), videoUploadDirectoryPath);

            var fileUploadDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, _siteInfo.Additional.FileUploadDirectoryName);
            DirectoryUtils.DeleteDirectoryIfExists(fileUploadDirectoryPath);
            DirectoryUtils.Copy(PathUtils.Combine(_sitePath, _siteInfo.Additional.FileUploadDirectoryName), fileUploadDirectoryPath);

            Atom.Core.AtomFeed feed = AtomUtility.GetEmptyFeed();  
            var entry = AtomUtility.GetEmptyEntry();  
            AtomUtility.AddDcElement(entry.AdditionalElements, "ImageUploadDirectoryName", _siteInfo.Additional.ImageUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "VideoUploadDirectoryName", _siteInfo.Additional.VideoUploadDirectoryName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "FileUploadDirectoryName", _siteInfo.Additional.FileUploadDirectoryName);

            feed.Entries.Add(entry);
            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName); 
            DirectoryUtils.CreateDirectoryIfNotExists(uploadFolderPath);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName); 
            feed.Save(uploadFilePath);

            ZipUtils.CreateZip(filePath, siteContentDirectoryPath);

            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);

            return PathUtils.GetFileName(filePath);
        }

        public bool ExportContents(string filePath, int channelId, List<int> contentIdArrayList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var siteContentDirectoryPath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), PathUtils.GetFileNameWithoutExtension(filePath));

            FileUtils.DeleteFileIfExists(filePath);
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            var contentIe = new ContentIe(_siteInfo, siteContentDirectoryPath);
            var isExport = contentIe.ExportContents(_siteInfo, channelId, contentIdArrayList, isPeriods, dateFrom, dateTo, checkedState);
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
