using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Office;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Core.Utils.Serialization.Components;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization
{
    public class ImportObject
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly Site _site;
        private readonly int _adminId;

        public ImportObject(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, int adminId)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _site = site;
            _adminId = adminId;
        }

        public async Task ImportFilesAsync(string siteTemplatePath, bool isOverride, string guid)
        {
            var sitePath = await _pathManager.GetSitePathAsync(_site);

            IList<string> siteDirList = new List<string>();

            var filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
            foreach (var filePath in filePaths)
            {
                var fileName = PathUtils.GetFileName(filePath);
                if (StringUtils.StartsWithIgnoreCase(fileName, "T_")) continue;

                var destFilePath = PathUtils.Combine(sitePath, fileName);
                _caching.SetProcess(guid, $"导入站点文件: {filePath}");
                FileUtils.MoveFile(filePath, destFilePath, isOverride);
            }

            if (_site.Root)
            {
                siteDirList = await _databaseManager.SiteRepository.GetSiteDirsAsync(0);
            }

            var directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
            foreach (var subDirectoryPath in directoryPaths)
            {
                var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                if (StringUtils.EqualsIgnoreCase(directoryName, "Template")) continue;

                if (_site.Root && (_pathManager.IsSystemDirectory(directoryName) || ListUtils.ContainsIgnoreCase(siteDirList, directoryName))) continue;

                _caching.SetProcess(guid, $"导入站点文件夹: {subDirectoryPath}");
                var destDirectoryPath = PathUtils.Combine(sitePath, directoryName);
                DirectoryUtils.MoveDirectory(subDirectoryPath, destDirectoryPath, isOverride);
            }

            var siteTemplateMetadataPath = PathUtils.Combine(sitePath, DirectoryUtils.SiteFiles.SiteTemplates.SiteTemplateMetadata);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplateMetadataPath);
        }

        public async Task ImportSiteContentAsync(string siteContentDirectoryPath, string filePath, bool isImportContents, string guid)
        {
            var siteIe = new SiteIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);
            await siteIe.ImportChannelsAndContentsAsync(filePath, isImportContents, false, 0, _adminId, guid);
        }
        public async Task ImportTemplatesAsync(string filePath, bool overwrite, int adminId, string guid)
        {
            var templateIe = new TemplateIe(_pathManager, _databaseManager, _caching, _site, filePath);
            await templateIe.ImportTemplatesAsync(overwrite, adminId, guid);
        }

        public static async Task<string> ImportRelatedFieldByZipFileAsync(IPathManager pathManager, IDatabaseManager databaseManager, Site site, string zipFilePath)
        {
            var directoryPath = pathManager.GetTemporaryFilesPath("RelatedField");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            pathManager.ExtractZip(zipFilePath, directoryPath);

            var relatedFieldIe = new RelatedFieldIe(databaseManager, site, directoryPath);
            await relatedFieldIe.ImportRelatedFieldAsync(true);

            return directoryPath;
        }

        public async Task ImportTableStylesAsync(string tableDirectoryPath, string guid)
        {
            if (DirectoryUtils.IsDirectoryExists(tableDirectoryPath))
            {
                var tableStyleIe = new TableStyleIe(_databaseManager, _caching, tableDirectoryPath);
                await tableStyleIe.ImportTableStylesAsync(_site, guid);
            }
        }

        public static async Task<string> ImportTableStyleByZipFileAsync(IPathManager pathManager, IDatabaseManager databaseManager, string tableName, List<int> relatedIdentities, string zipFilePath)
        {
            var styleDirectoryPath = pathManager.GetTemporaryFilesPath("TableStyle");
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            pathManager.ExtractZip(zipFilePath, styleDirectoryPath);

            await ImportTableStyleByDirectoryAsync(databaseManager, tableName, relatedIdentities, styleDirectoryPath);

            return styleDirectoryPath;
        }

        public static async Task ImportTableStyleByDirectoryAsync(IDatabaseManager databaseManager, string tableName, List<int> relatedIdentities, string styleDirectoryPath)
        {
            await TableStyleIe.SingleImportTableStyleAsync(databaseManager, tableName, styleDirectoryPath, relatedIdentities);
        }

        public async Task ImportConfigurationAsync(string configurationFilePath, string guid)
        {
            var configIe = new ConfigurationIe(_databaseManager, _caching, _site, configurationFilePath);
            await configIe.ImportAsync(guid);
        }

        public async Task ImportChannelsAndContentsByZipFileAsync(int parentId, string zipFilePath, bool isOverride, string guid)
        {
            var siteContentDirectoryPath = _pathManager.GetTemporaryFilesPath(BackupType.ChannelsAndContents.GetValue());
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            _pathManager.ExtractZip(zipFilePath, siteContentDirectoryPath);

            await ImportChannelsAndContentsFromZipAsync(parentId, siteContentDirectoryPath, isOverride, guid);

            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName);
            if (!FileUtils.IsFileExists(uploadFilePath))
            {
                return;
            }

            var sitePath = await _pathManager.GetSitePathAsync(_site);

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(uploadFilePath));
            if (feed != null)
            {
                AtomEntry entry = feed.Entries[0];
                string imageUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageUploadDirectoryName");
                if(imageUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, imageUploadDirectoryPath), PathUtils.Combine(sitePath, _site.ImageUploadDirectoryName), isOverride); 
                }
                string videoUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "VideoUploadDirectoryName");
                if (videoUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, videoUploadDirectoryPath), PathUtils.Combine(sitePath, _site.VideoUploadDirectoryName), isOverride);
                }
                string fileUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FileUploadDirectoryName");
                if (fileUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, fileUploadDirectoryPath), PathUtils.Combine(sitePath, _site.FileUploadDirectoryName), isOverride);
                }
            }
        }

        private async Task ImportChannelsAndContentsFromZipAsync(int parentId, string siteContentDirectoryPath, bool isOverride, string guid)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var siteIe = new SiteIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);

            Hashtable levelHashtable = null;
            foreach (var filePath in filePathList)
            {
                var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
                var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
                var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

                var level = StringUtils.GetCount("_", orderString);

                if (levelHashtable == null)
                {
                    levelHashtable = new Hashtable
                    {
                        [level] = parentId
                    };
                }

                var insertChannelId = await siteIe.ImportChannelsAndContentsAsync(filePath, true, isOverride, (int)levelHashtable[level], _adminId, guid);
                levelHashtable[level + 1] = insertChannelId;
            }
        }

        public async Task ImportChannelsAndContentsAsync(int parentId, string siteContentDirectoryPath, bool isOverride, string guid)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var siteIe = new SiteIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);

            var parentOrderString = "none";
            //int parentID = 0;
            foreach (var filePath in filePathList)
            {
                var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
                var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
                var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

                if (StringUtils.StartsWithIgnoreCase(orderString, parentOrderString))
                {
                    parentId = await siteIe.ImportChannelsAndContentsAsync(filePath, true, isOverride, parentId, _adminId, guid);
                    parentOrderString = orderString;
                }
                else
                {
                    await siteIe.ImportChannelsAndContentsAsync(filePath, true, isOverride, parentId, _adminId, guid);
                }
            }
        }

        public async Task<List<int>> ImportContentsByZipFileAsync(Channel channel, string zipFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var siteContentDirectoryPath = _pathManager.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            _pathManager.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var taxis = await _databaseManager.ContentRepository.GetMaxTaxisAsync(_site, channel, false);

            return await ImportContentsAsync(channel, siteContentDirectoryPath, isOverride, taxis, isChecked, checkedLevel, adminId, userId, sourceId);
        }

        public async Task<List<int>> ImportContentsByXlsxFileAsync(Channel channel, string filePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var excelObject = new ExcelObject(_databaseManager, _pathManager);
            var contentInfoList = await excelObject.GetContentsByFileAsync(filePath, _site, channel);
            contentInfoList.Reverse();

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                if (!contentInfo.AddDate.HasValue)
                {
                    contentInfo.AddDate = DateTime.Now;
                }
                contentInfo.AdminId = adminId;
                contentInfo.UserId = userId;
                contentInfo.SourceId = sourceId;

                if (isOverride)
                {
                    var existsIds = await _databaseManager.ContentRepository.GetContentIdsBySameTitleAsync(_site, channel, contentInfo.Title);
                    if (existsIds.Count > 0)
                    {
                        foreach (var id in existsIds)
                        {
                            contentInfo.Id = id;
                            await _databaseManager.ContentRepository.UpdateAsync(_site, channel, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await _databaseManager.ContentRepository.InsertAsync(_site, channel, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await _databaseManager.ContentRepository.InsertAsync(_site, channel, contentInfo);
                }
            }

            return contentInfoList.Select(x => x.Id).ToList();
        }

        public async Task<List<int>> ImportContentsByTxtFileAsync(Channel channel, string txtFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var contentInfo = new Content
            {
                SiteId = channel.SiteId,
                ChannelId = channel.Id,
                Title = PathUtils.GetFileNameWithoutExtension(txtFilePath),
                Checked = isChecked,
                CheckedLevel = checkedLevel,
                AddDate = DateTime.Now,
                AdminId = adminId,
                UserId = userId,
                SourceId = sourceId,
                Body = StringUtils.ReplaceNewlineToBr(FileUtils.ReadText(txtFilePath))
            };

            if (isOverride)
            {
                var existsIDs = await _databaseManager.ContentRepository.GetContentIdsBySameTitleAsync(_site, channel, contentInfo.Title);
                if (existsIDs.Count > 0)
                {
                    foreach (var id in existsIDs)
                    {
                        contentInfo.Id = id;
                        await _databaseManager.ContentRepository.UpdateAsync(_site, channel, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await _databaseManager.ContentRepository.InsertAsync(_site, channel, contentInfo);
                }
            }
            else
            {
                contentInfo.Id = await _databaseManager.ContentRepository.InsertAsync(_site, channel, contentInfo);
            }

            return new List<int>
            {
                contentInfo.Id
            };
        }

        private async Task<List<int>> ImportContentsAsync(Channel channel, string siteContentDirectoryPath, bool isOverride, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");
            var sitePath = await _pathManager.GetSitePathAsync(_site);
            var contentIe = new ContentIe(_pathManager, _databaseManager, _caching, _site, siteContentDirectoryPath);

            var contentIdList = await contentIe.ImportContentsAsync(filePath, isOverride, channel, taxis, isChecked, checkedLevel, adminId, userId, sourceId);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, sitePath, isOverride);

            return contentIdList;
        }

        public static IEnumerable<string> GetSiteContentFilePathList(string siteContentDirectoryPath)
        {
            var filePaths = DirectoryUtils.GetFilePaths(siteContentDirectoryPath);
            var filePathSortedList = new SortedList<string, string>();
            foreach (var filePath in filePaths)
            {
                var keyBuilder = new StringBuilder();
                var fileName = StringUtils.ToLower(PathUtils.GetFileName(filePath)).Replace(".xml", "");
                var intList = fileName.Split('_');
                foreach (var numStr in intList)
                {
                    var count = 7 - numStr.Length;
                    if (count > 0)
                    {
                        for (var i = 0; i < count; i++)
                        {
                            keyBuilder.Append("0");
                        }
                    }
                    keyBuilder.Append(numStr);
                    keyBuilder.Append("_");
                }
                if (keyBuilder.Length > 0) keyBuilder.Remove(keyBuilder.Length - 1, 1);
                filePathSortedList.Add(keyBuilder.ToString(), filePath);
            }
            return filePathSortedList.Values;
        }

    }
}
