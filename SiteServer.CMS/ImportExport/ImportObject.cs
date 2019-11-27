using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.ImportExport.Components;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport
{
    public class ImportObject
    {
        private readonly int _siteId;
        private readonly string _adminName;

        public ImportObject(int siteId, string adminName)
        {
            _siteId = siteId;
            _adminName = adminName;
        }

        //获取保存辅助表名称对应集合数据库缓存键
        private string GetTableNameNameValueCollectionDbCacheKey()
        {
            return "SiteServer.CMS.Core.ImportObject.TableNameNameValueCollection_" + _siteId;
        }

        public async Task<NameValueCollection> GetTableNameCacheAsync()
        {
            NameValueCollection nameValueCollection = null;
            var cacheValue = await DataProvider.DbCacheDao.GetValueAndRemoveAsync(GetTableNameNameValueCollectionDbCacheKey());
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
                await DataProvider.DbCacheDao.RemoveAndInsertAsync(cacheKey, cacheValue);
            }
        }

        public async Task RemoveDbCacheAsync()
        {
            var cacheKey = GetTableNameNameValueCollectionDbCacheKey();
            await DataProvider.DbCacheDao.GetValueAndRemoveAsync(cacheKey);
        }

        public async Task ImportFilesAsync(string siteTemplatePath, bool isOverride)
        {
            //if (this.FSO.IsRoot)
            //{
            //    string[] filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
            //    foreach (string filePath in filePaths)
            //    {
            //        string fileName = PathUtils.GetFileName(filePath);
            //        if (!PathUtility.IsSystemFile(fileName))
            //        {
            //            string destFilePath = PathUtils.Combine(FSO.SitePath, fileName);
            //            FileUtils.MoveFile(filePath, destFilePath, isOverride);
            //        }
            //    }

            //    ArrayList siteDirArrayList = DataProvider.SiteDAO.GetLowerSiteDirArrayListThatNotIsRoot();

            //    string[] directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
            //    foreach (string subDirectoryPath in directoryPaths)
            //    {
            //        string directoryName = PathUtils.GetDirectoryName(subDirectoryPath);
            //        if (!PathUtility.IsSystemDirectory(directoryName) && !siteDirArrayList.Contains(directoryName.ToLower()))
            //        {
            //            string destDirectoryPath = PathUtils.Combine(FSO.SitePath, directoryName);
            //            DirectoryUtils.MoveDirectory(subDirectoryPath, destDirectoryPath, isOverride);
            //        }
            //    }
            //}
            //else
            //{
            //    DirectoryUtils.MoveDirectory(siteTemplatePath, FSO.SitePath, isOverride);
            //}
            //string siteTemplateMetadataPath = PathUtils.Combine(FSO.SitePath, DirectoryUtility.SiteTemplates.SiteTemplateMetadata);
            //DirectoryUtils.DeleteDirectoryIfExists(siteTemplateMetadataPath);
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            await DirectoryUtility.ImportSiteFilesAsync(site, siteTemplatePath, isOverride);
        }

        public async Task ImportSiteContentAsync(string siteContentDirectoryPath, string filePath, bool isImportContents)
        {
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var siteIe = new SiteIe(site, siteContentDirectoryPath);
            await siteIe.ImportChannelsAndContentsAsync(filePath, isImportContents, false, 0, _adminName);
        }


        /// <summary>
        /// 从指定的地址导入网站模板至站点中
        /// </summary>
        public async Task ImportTemplatesAsync(string filePath, bool overwrite, string administratorName)
        {
            var templateIe = new TemplateIe(_siteId, filePath);
            await templateIe.ImportTemplatesAsync(overwrite, administratorName);
        }

        public async Task ImportRelatedFieldByZipFileAsync(string zipFilePath, bool overwrite)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("RelatedField");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.ExtractZip(zipFilePath, directoryPath);

            var relatedFieldIe = new RelatedFieldIe(_siteId, directoryPath);
            await relatedFieldIe.ImportRelatedFieldAsync(overwrite);
        }

        public async Task ImportTableStylesAsync(string tableDirectoryPath)
        {
            if (DirectoryUtils.IsDirectoryExists(tableDirectoryPath))
            {
                var tableStyleIe = new TableStyleIe(tableDirectoryPath, _adminName);
                await tableStyleIe.ImportTableStylesAsync(_siteId);
            }
        }

        public static async Task ImportTableStyleByZipFileAsync(string tableName, int channelId, string zipFilePath)
        {
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, styleDirectoryPath);

            await TableStyleIe.SingleImportTableStyleAsync(tableName, styleDirectoryPath, channelId);
        }

        public async Task ImportConfigurationAsync(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_siteId, configurationFilePath);
            await configIe.ImportAsync();
        }

        public async Task ImportChannelsAndContentsByZipFileAsync(int parentId, string zipFilePath, bool isOverride)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.ChannelsAndContents));
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            await ImportChannelsAndContentsFromZipAsync(parentId, siteContentDirectoryPath, isOverride);

            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName);
            if (!FileUtils.IsFileExists(uploadFilePath))
            {
                return;
            }

            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir);

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(uploadFilePath));
            if (feed != null)
            {
                AtomEntry entry = feed.Entries[0];
                string imageUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageUploadDirectoryName");
                if(imageUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, imageUploadDirectoryPath), PathUtils.Combine(sitePath, site.ImageUploadDirectoryName), isOverride); 
                }
                string videoUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "VideoUploadDirectoryName");
                if (videoUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, videoUploadDirectoryPath), PathUtils.Combine(sitePath, site.VideoUploadDirectoryName), isOverride);
                }
                string fileUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FileUploadDirectoryName");
                if (fileUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, fileUploadDirectoryPath), PathUtils.Combine(sitePath, site.FileUploadDirectoryName), isOverride);
                }
            }
        }

        public async Task ImportChannelsAndContentsFromZipAsync(int parentId, string siteContentDirectoryPath, bool isOverride)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var siteIe = new SiteIe(site, siteContentDirectoryPath);

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

                var insertChannelId = await siteIe.ImportChannelsAndContentsAsync(filePath, true, isOverride, (int)levelHashtable[level], _adminName);
                levelHashtable[level + 1] = insertChannelId;
            }
        }

        public async Task ImportChannelsAndContentsAsync(int parentId, string siteContentDirectoryPath, bool isOverride)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var siteIe = new SiteIe(site, siteContentDirectoryPath);

            var parentOrderString = "none";
            //int parentID = 0;
            foreach (var filePath in filePathList)
            {
                var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
                var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
                var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

                if (StringUtils.StartsWithIgnoreCase(orderString, parentOrderString))
                {
                    parentId = await siteIe.ImportChannelsAndContentsAsync(filePath, true, isOverride, parentId, _adminName);
                    parentOrderString = orderString;
                }
                else
                {
                    await siteIe.ImportChannelsAndContentsAsync(filePath, true, isOverride, parentId, _adminName);
                }
            }
        }

        public async Task ImportContentsByZipFileAsync(Channel channel, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            var taxis = await DataProvider.ContentDao.GetMaxTaxisAsync(tableName, channel.Id, false);

            await ImportContentsAsync(channel, siteContentDirectoryPath, isOverride, taxis, importStart, importCount, isChecked, checkedLevel);
        }

        public async Task<List<int>> ImportContentsByZipFileAsync(Channel channel, string zipFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

            var taxis = await DataProvider.ContentDao.GetMaxTaxisAsync(tableName, channel.Id, false);

            return await ImportContentsAsync(channel, siteContentDirectoryPath, isOverride, taxis, isChecked, checkedLevel, adminId, userId, sourceId);
        }

        public async Task ImportContentsByCsvFileAsync(int channelId, string csvFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var channelInfo = await ChannelManager.GetChannelAsync(_siteId, channelId);
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var contentInfoList = await ExcelObject.GetContentsByCsvFileAsync(csvFilePath, site, channelInfo);
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

            var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                if (isOverride)
                {
                    var existsIds = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                    if (existsIds.Count > 0)
                    {
                        foreach (var id in existsIds)
                        {
                            contentInfo.Id = id;
                            await DataProvider.ContentDao.UpdateAsync(site, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channelInfo, contentInfo);
                }
                //this.FSO.AddContentToWaitingCreate(contentInfo.ChannelId, contentID);
            }
        }

        public async Task<List<int>> ImportContentsByCsvFileAsync(Channel channel, string csvFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var contentInfoList = await ExcelObject.GetContentsByCsvFileAsync(csvFilePath, site, channel);
            contentInfoList.Reverse();

            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

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
                    var existsIds = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                    if (existsIds.Count > 0)
                    {
                        foreach (var id in existsIds)
                        {
                            contentInfo.Id = id;
                            await DataProvider.ContentDao.UpdateAsync(site, channel, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channel, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channel, contentInfo);
                }
            }

            return contentInfoList.Select(x => x.Id).ToList();
        }

        public async Task ImportContentsByTxtZipFileAsync(int channelId, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.ExtractZip(zipFilePath, directoryPath);

            var channelInfo = await ChannelManager.GetChannelAsync(_siteId, channelId);

            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var contentInfoList = TxtObject.GetContentListByTxtFile(directoryPath, site, channelInfo);

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

            var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;

                //int contentID = DataProvider.ContentDAO.Insert(tableName, this.FSO.Site, contentInfo);

                if (isOverride)
                {
                    var existsIDs = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            await DataProvider.ContentDao.UpdateAsync(site, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channelInfo, contentInfo);
                }

                //this.FSO.AddContentToWaitingCreate(contentInfo.ChannelId, contentID);
            }
        }

        public async Task<List<int>> ImportContentsByTxtFileAsync(Channel channel, string txtFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);

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
                var existsIDs = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                if (existsIDs.Count > 0)
                {
                    foreach (var id in existsIDs)
                    {
                        contentInfo.Id = id;
                        await DataProvider.ContentDao.UpdateAsync(site, channel, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channel, contentInfo);
                }
            }
            else
            {
                contentInfo.Id = await DataProvider.ContentDao.InsertAsync(site, channel, contentInfo);
            }

            return new List<int>
            {
                contentInfo.Id
            };
        }

        public async Task ImportContentsAsync(Channel channel, string siteContentDirectoryPath, bool isOverride, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir);
            var contentIe = new ContentIe(site, siteContentDirectoryPath);

            await contentIe.ImportContentsAsync(filePath, isOverride, channel, taxis, importStart, importCount, isChecked, checkedLevel, _adminName);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, sitePath, isOverride);
        }

        public async Task<List<int>> ImportContentsAsync(Channel channel, string siteContentDirectoryPath, bool isOverride, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");
            var site = await DataProvider.SiteDao.GetAsync(_siteId);
            var sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, site.SiteDir);
            var contentIe = new ContentIe(site, siteContentDirectoryPath);

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
