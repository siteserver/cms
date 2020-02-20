using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Serialization.Atom.Atom.Core;
using SiteServer.CMS.Serialization.Components;

namespace SiteServer.CMS.Serialization
{
    public class ImportObject
    {
        private readonly Site _site;
        private readonly int _adminId;

        public ImportObject(Site site, int adminId)
        {
            _site = site;
            _adminId = adminId;
        }

        //获取保存辅助表名称对应集合数据库缓存键
        private string GetTableNameNameValueCollectionDbCacheKey()
        {
            return "SiteServer.CMS.Core.ImportObject.TableNameNameValueCollection_" + _site.Id;
        }

        public async Task<NameValueCollection> GetTableNameCacheAsync()
        {
            NameValueCollection nameValueCollection = null;
            var cacheValue = await DataProvider.DbCacheRepository.GetValueAndRemoveAsync(GetTableNameNameValueCollectionDbCacheKey());
            if (!string.IsNullOrEmpty(cacheValue))
            {
                nameValueCollection = TranslateUtils.ToNameValueCollection(cacheValue);
            }
            return nameValueCollection;
        }

        public async Task SaveTableNameCacheAsync(NameValueCollection nameValueCollection)
        {
            if (nameValueCollection != null && nameValueCollection.Count > 0)
            {
                var cacheKey = GetTableNameNameValueCollectionDbCacheKey();
                var cacheValue = TranslateUtils.NameValueCollectionToString(nameValueCollection);
                await DataProvider.DbCacheRepository.RemoveAndInsertAsync(cacheKey, cacheValue);
            }
        }

        public async Task RemoveDbCacheAsync()
        {
            var cacheKey = GetTableNameNameValueCollectionDbCacheKey();
            await DataProvider.DbCacheRepository.GetValueAndRemoveAsync(cacheKey);
        }

        public async Task ImportFilesAsync(string siteTemplatePath, bool isOverride, string guid)
        {
            var sitePath = await PathUtility.GetSitePathAsync(_site);

            if (_site.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!PathUtility.IsSystemFile(fileName))
                    {
                        var destFilePath = PathUtils.Combine(sitePath, fileName);
                        Caching.SetProcess(guid, $"导入站点文件: {filePath}");
                        FileUtils.MoveFile(filePath, destFilePath, isOverride);
                    }
                }

                var siteDirList = await DataProvider.SiteRepository.GetSiteDirListAsync(0);

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!WebUtils.IsSystemDirectory(directoryName) && !StringUtils.ContainsIgnoreCase(siteDirList, directoryName))
                    {
                        Caching.SetProcess(guid, $"导入站点文件夹: {subDirectoryPath}");
                        var destDirectoryPath = PathUtils.Combine(sitePath, directoryName);
                        DirectoryUtils.MoveDirectory(subDirectoryPath, destDirectoryPath, isOverride);
                    }
                }
            }
            else
            {
                Caching.SetProcess(guid, $"导入站点文件夹: {siteTemplatePath}");
                DirectoryUtils.MoveDirectory(siteTemplatePath, sitePath, isOverride);
            }
            var siteTemplateMetadataPath = PathUtils.Combine(sitePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata);
            DirectoryUtils.DeleteDirectoryIfExists(siteTemplateMetadataPath);
        }

        public async Task ImportSiteContentAsync(string siteContentDirectoryPath, string filePath, bool isImportContents, string guid)
        {
            var siteIe = new SiteIe(_site, siteContentDirectoryPath);
            await siteIe.ImportChannelsAndContentsAsync(filePath, isImportContents, false, 0, _adminId, guid);
        }


        /// <summary>
        /// 从指定的地址导入网站模板至站点中
        /// </summary>
        public async Task ImportTemplatesAsync(string filePath, bool overwrite, int adminId, string guid)
        {
            var templateIe = new TemplateIe(_site, filePath);
            await templateIe.ImportTemplatesAsync(overwrite, adminId, guid);
        }

        public static async Task<string> ImportRelatedFieldByZipFileAsync(Site site, string zipFilePath)
        {
            var directoryPath = PathUtility.GetTemporaryFilesPath("RelatedField");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.ExtractZip(zipFilePath, directoryPath);

            var relatedFieldIe = new RelatedFieldIe(site, directoryPath);
            await relatedFieldIe.ImportRelatedFieldAsync(true);

            return directoryPath;
        }

        public async Task ImportTableStylesAsync(string tableDirectoryPath, string guid)
        {
            if (DirectoryUtils.IsDirectoryExists(tableDirectoryPath))
            {
                var tableStyleIe = new TableStyleIe(tableDirectoryPath, _adminId);
                await tableStyleIe.ImportTableStylesAsync(_site, guid);
            }
        }

        public static async Task<string> ImportTableStyleByZipFileAsync(string tableName, List<int> relatedIdentities, string zipFilePath)
        {
            var styleDirectoryPath = PathUtility.GetTemporaryFilesPath("TableStyle");
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, styleDirectoryPath);

            await TableStyleIe.SingleImportTableStyleAsync(tableName, styleDirectoryPath, relatedIdentities);
            return styleDirectoryPath;
        }

        public async Task ImportConfigurationAsync(string configurationFilePath, string guid)
        {
            var configIe = new ConfigurationIe(_site, configurationFilePath);
            await configIe.ImportAsync(guid);
        }

        public async Task ImportChannelsAndContentsByZipFileAsync(int parentId, string zipFilePath, bool isOverride, string guid)
        {
            var siteContentDirectoryPath = PathUtility.GetTemporaryFilesPath(BackupType.ChannelsAndContents.GetValue());
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            await ImportChannelsAndContentsFromZipAsync(parentId, siteContentDirectoryPath, isOverride, guid);

            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName);
            if (!FileUtils.IsFileExists(uploadFilePath))
            {
                return;
            }

            var sitePath = await PathUtility.GetSitePathAsync(_site);

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

        public async Task ImportChannelsAndContentsFromZipAsync(int parentId, string siteContentDirectoryPath, bool isOverride, string guid)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var siteIe = new SiteIe(_site, siteContentDirectoryPath);

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

            var siteIe = new SiteIe(_site, siteContentDirectoryPath);

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

        public async Task ImportContentsByZipFileAsync(Channel channel, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel, string guid)
        {
            var siteContentDirectoryPath = PathUtility.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var taxis = await DataProvider.ContentRepository.GetMaxTaxisAsync(_site, channel, false);

            await ImportContentsAsync(channel, siteContentDirectoryPath, isOverride, taxis, importStart, importCount, isChecked, checkedLevel, guid);
        }

        public async Task<List<int>> ImportContentsByZipFileAsync(Channel channel, string zipFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var siteContentDirectoryPath = PathUtility.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var taxis = await DataProvider.ContentRepository.GetMaxTaxisAsync(_site, channel, false);

            return await ImportContentsAsync(channel, siteContentDirectoryPath, isOverride, taxis, isChecked, checkedLevel, adminId, userId, sourceId);
        }

        public async Task ImportContentsByCsvFileAsync(int channelId, string csvFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            var contentInfoList = await ExcelObject.GetContentsByCsvFileAsync(csvFilePath, _site, channelInfo);
            contentInfoList.Reverse();

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<Content>();

                if (importStart == 0)
                {
                    importStart = 1;
                }
                if (importCount == 0)
                {
                    importCount = contentInfoList.Count;
                }

                var firstIndex = contentInfoList.Count - importStart - importCount + 1;
                if (firstIndex <= 0)
                {
                    firstIndex = 0;
                }

                var addCount = 0;
                for (var i = 0; i < contentInfoList.Count; i++)
                {
                    if (addCount >= importCount) break;
                    if (i >= firstIndex)
                    {
                        theList.Add(contentInfoList[i]);
                        addCount++;
                    }
                }

                contentInfoList = theList;
            }

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                if (isOverride)
                {
                    var existsIds = await DataProvider.ContentRepository.GetIdListBySameTitleAsync(_site, channelInfo, contentInfo.Title);
                    if (existsIds.Count > 0)
                    {
                        foreach (var id in existsIds)
                        {
                            contentInfo.Id = id;
                            await DataProvider.ContentRepository.UpdateAsync(_site, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channelInfo, contentInfo);
                }
                //this.FSO.AddContentToWaitingCreate(contentInfo.ChannelId, contentID);
            }
        }

        public async Task<List<int>> ImportContentsByCsvFileAsync(Channel channel, string csvFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var contentInfoList = await ExcelObject.GetContentsByCsvFileAsync(csvFilePath, _site, channel);
            contentInfoList.Reverse();

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                if (!contentInfo.AddDate.HasValue)
                {
                    contentInfo.AddDate = DateTime.Now;
                }
                contentInfo.LastEditDate = DateTime.Now;
                contentInfo.AdminId = adminId;
                contentInfo.UserId = userId;
                contentInfo.SourceId = sourceId;

                if (isOverride)
                {
                    var existsIds = await DataProvider.ContentRepository.GetIdListBySameTitleAsync(_site, channel, contentInfo.Title);
                    if (existsIds.Count > 0)
                    {
                        foreach (var id in existsIds)
                        {
                            contentInfo.Id = id;
                            await DataProvider.ContentRepository.UpdateAsync(_site, channel, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channel, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channel, contentInfo);
                }
            }

            return contentInfoList.Select(x => x.Id).ToList();
        }

        public async Task ImportContentsByTxtZipFileAsync(int channelId, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var directoryPath = PathUtility.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.ExtractZip(zipFilePath, directoryPath);

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);

            var contentInfoList = TxtObject.GetContentListByTxtFile(directoryPath, _site, channelInfo);

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<Content>();

                if (importStart == 0)
                {
                    importStart = 1;
                }
                if (importCount == 0)
                {
                    importCount = contentInfoList.Count;
                }

                var firstIndex = contentInfoList.Count - importStart - importCount + 1;
                if (firstIndex <= 0)
                {
                    firstIndex = 0;
                }

                var addCount = 0;
                for (var i = 0; i < contentInfoList.Count; i++)
                {
                    if (addCount >= importCount) break;
                    if (i >= firstIndex)
                    {
                        theList.Add(contentInfoList[i]);
                        addCount++;
                    }
                }

                contentInfoList = theList;
            }

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;

                //int contentID = DataProvider.ContentDAO.Insert(tableName, this.FSO.Site, contentInfo);

                if (isOverride)
                {
                    var existsIDs = await DataProvider.ContentRepository.GetIdListBySameTitleAsync(_site, channelInfo, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            await DataProvider.ContentRepository.UpdateAsync(_site, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channelInfo, contentInfo);
                }

                //this.FSO.AddContentToWaitingCreate(contentInfo.ChannelId, contentID);
            }
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
                LastEditDate = DateTime.Now,
                AdminId = adminId,
                UserId = userId,
                SourceId = sourceId
            };

            contentInfo.Set(ContentAttribute.Content, StringUtils.ReplaceNewlineToBr(FileUtils.ReadText(txtFilePath, Encoding.UTF8)));

            if (isOverride)
            {
                var existsIDs = await DataProvider.ContentRepository.GetIdListBySameTitleAsync(_site, channel, contentInfo.Title);
                if (existsIDs.Count > 0)
                {
                    foreach (var id in existsIDs)
                    {
                        contentInfo.Id = id;
                        await DataProvider.ContentRepository.UpdateAsync(_site, channel, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channel, contentInfo);
                }
            }
            else
            {
                contentInfo.Id = await DataProvider.ContentRepository.InsertAsync(_site, channel, contentInfo);
            }

            return new List<int>
            {
                contentInfo.Id
            };
        }

        public async Task ImportContentsAsync(Channel channel, string siteContentDirectoryPath, bool isOverride, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel, string guid)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");
            var sitePath = await PathUtility.GetSitePathAsync(_site);
            var contentIe = new ContentIe(_site, siteContentDirectoryPath);

            await contentIe.ImportContentsAsync(filePath, isOverride, channel, taxis, importStart, importCount, isChecked, checkedLevel, _adminId, guid);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, sitePath, isOverride);
        }

        public async Task<List<int>> ImportContentsAsync(Channel channel, string siteContentDirectoryPath, bool isOverride, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");
            var sitePath = await PathUtility.GetSitePathAsync(_site);
            var contentIe = new ContentIe(_site, siteContentDirectoryPath);

            var contentIdList = await contentIe.ImportContentsAsync(filePath, isOverride, channel, taxis, isChecked, checkedLevel, adminId, userId, sourceId);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, sitePath, isOverride);

            return contentIdList;
        }

        //public void ImportInputContentsByCsvFile(InputInfo inputInfo, string excelFilePath, int importStart, int importCount, bool isChecked)
        //{
        //    var contentInfoList = ExcelObject.GetInputContentsByCsvFile(excelFilePath, _site, inputInfo);
        //    contentInfoList.Reverse();

        //    if (importStart > 1 || importCount > 0)
        //    {
        //        var theList = new List<InputContentInfo>();

        //        if (importStart == 0)
        //        {
        //            importStart = 1;
        //        }
        //        if (importCount == 0)
        //        {
        //            importCount = contentInfoList.Count;
        //        }

        //        var firstIndex = contentInfoList.Count - importStart - importCount + 1;
        //        if (firstIndex <= 0)
        //        {
        //            firstIndex = 0;
        //        }

        //        var addCount = 0;
        //        for (var i = 0; i < contentInfoList.Count; i++)
        //        {
        //            if (addCount >= importCount) break;
        //            if (i >= firstIndex)
        //            {
        //                theList.Add(contentInfoList[i]);
        //                addCount++;
        //            }
        //        }

        //        contentInfoList = theList;
        //    }

        //    foreach (var contentInfo in contentInfoList)
        //    {
        //        contentInfo.IsChecked = isChecked;
        //        DataProvider.InputContentDao.Insert(contentInfo);
        //    }
        //}

        public static IList<string> GetSiteContentFilePathList(string siteContentDirectoryPath)
        {
            var filePaths = DirectoryUtils.GetFilePaths(siteContentDirectoryPath);
            var filePathSortedList = new SortedList<string, string>();
            foreach (var filePath in filePaths)
            {
                var keyBuilder = new StringBuilder();
                var fileName = PathUtils.GetFileName(filePath).ToLower().Replace(".xml", "");
                var nums = fileName.Split('_');
                foreach (var numStr in nums)
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
