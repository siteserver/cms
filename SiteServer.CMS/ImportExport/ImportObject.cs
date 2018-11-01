using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport.Components;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.CMS.ImportExport
{
    public class ImportObject
    {
        private readonly SiteInfo _siteInfo;
        private readonly string _sitePath;
        private readonly string _adminName;

        public ImportObject(int siteId, string adminName)
        {
            _siteInfo = SiteManager.GetSiteInfo(siteId);
            _sitePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, _siteInfo.SiteDir);
            _adminName = adminName;
        }

        //获取保存辅助表名称对应集合数据库缓存键
        private string GetTableNameNameValueCollectionDbCacheKey()
        {
            return "SiteServer.CMS.Core.ImportObject.TableNameNameValueCollection_" + _siteInfo.Id;
        }

        public NameValueCollection GetTableNameCache()
        {
            NameValueCollection nameValueCollection = null;
            var cacheValue = CacheDbUtils.GetValue(GetTableNameNameValueCollectionDbCacheKey());
            if (!string.IsNullOrEmpty(cacheValue))
            {
                nameValueCollection = TranslateUtils.ToNameValueCollection(cacheValue);
            }
            return nameValueCollection;
        }

        public void SaveTableNameCache(NameValueCollection nameValueCollection)
        {
            if (nameValueCollection != null && nameValueCollection.Count > 0)
            {
                var cacheKey = GetTableNameNameValueCollectionDbCacheKey();
                var cacheValue = TranslateUtils.NameValueCollectionToString(nameValueCollection);
                CacheDbUtils.RemoveAndInsert(cacheKey, cacheValue);
            }
        }

        public void RemoveDbCache()
        {
            var cacheKey = GetTableNameNameValueCollectionDbCacheKey();
            CacheDbUtils.GetValueAndRemove(cacheKey);
        }

        public void ImportFiles(string siteTemplatePath, bool isOverride)
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
            DirectoryUtility.ImportSiteFiles(_siteInfo, siteTemplatePath, isOverride);
        }

        public void ImportSiteContent(string siteContentDirectoryPath, string filePath, bool isImportContents)
        {
            var siteIe = new SiteIe(_siteInfo, siteContentDirectoryPath);
            siteIe.ImportChannelsAndContents(filePath, isImportContents, false, 0, _adminName);
        }


        /// <summary>
        /// 从指定的地址导入网站模板至站点中
        /// </summary>
        public void ImportTemplates(string filePath, bool overwrite, string administratorName)
        {
            var templateIe = new TemplateIe(_siteInfo.Id, filePath);
            templateIe.ImportTemplates(overwrite, administratorName);
        }

        public void ImportRelatedFieldByZipFile(string zipFilePath, bool overwrite)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("RelatedField");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.ExtractZip(zipFilePath, directoryPath);

            var relatedFieldIe = new RelatedFieldIe(_siteInfo.Id, directoryPath);
            relatedFieldIe.ImportRelatedField(overwrite);
        }

        public void ImportTableStyles(string tableDirectoryPath)
        {
            if (DirectoryUtils.IsDirectoryExists(tableDirectoryPath))
            {
                var tableStyleIe = new TableStyleIe(tableDirectoryPath, _adminName);
                tableStyleIe.ImportTableStyles(_siteInfo.Id);
            }
        }

        public static void ImportTableStyleByZipFile(string tableName, int channelId, string zipFilePath)
        {
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, styleDirectoryPath);

            TableStyleIe.SingleImportTableStyle(tableName, styleDirectoryPath, channelId);
        }

        public void ImportConfiguration(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_siteInfo.Id, configurationFilePath);
            configIe.Import();
        }


        public void ImportChannelsAndContentsByZipFile(int parentId, string zipFilePath, bool isOverride)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.ChannelsAndContents));
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            ImportChannelsAndContentsFromZip(parentId, siteContentDirectoryPath, isOverride);

            var uploadFolderPath = PathUtils.Combine(siteContentDirectoryPath, BackupUtility.UploadFolderName);
            var uploadFilePath = PathUtils.Combine(uploadFolderPath, BackupUtility.UploadFileName);
            if (!FileUtils.IsFileExists(uploadFilePath))
            {
                return;
            }
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(uploadFilePath));
            if (feed != null)
            {
                AtomEntry entry = feed.Entries[0];
                string imageUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ImageUploadDirectoryName");
                if(imageUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, imageUploadDirectoryPath), PathUtils.Combine(_sitePath, _siteInfo.Additional.ImageUploadDirectoryName), isOverride); 
                }
                string videoUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "VideoUploadDirectoryName");
                if (videoUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, videoUploadDirectoryPath), PathUtils.Combine(_sitePath, _siteInfo.Additional.VideoUploadDirectoryName), isOverride);
                }
                string fileUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FileUploadDirectoryName");
                if (fileUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, fileUploadDirectoryPath), PathUtils.Combine(_sitePath, _siteInfo.Additional.FileUploadDirectoryName), isOverride);
                }
            }
        }

        public void ImportChannelsAndContentsFromZip(int parentId, string siteContentDirectoryPath, bool isOverride)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var siteIe = new SiteIe(_siteInfo, siteContentDirectoryPath);

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

                var insertChannelId = siteIe.ImportChannelsAndContents(filePath, true, isOverride, (int)levelHashtable[level], _adminName);
                levelHashtable[level + 1] = insertChannelId;
            }
        }

        public void ImportChannelsAndContents(int parentId, string siteContentDirectoryPath, bool isOverride)
        {
            var filePathList = GetSiteContentFilePathList(siteContentDirectoryPath);

            var siteIe = new SiteIe(_siteInfo, siteContentDirectoryPath);

            var parentOrderString = "none";
            //int parentID = 0;
            foreach (var filePath in filePathList)
            {
                var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
                var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
                var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

                if (StringUtils.StartsWithIgnoreCase(orderString, parentOrderString))
                {
                    parentId = siteIe.ImportChannelsAndContents(filePath, true, isOverride, parentId, _adminName);
                    parentOrderString = orderString;
                }
                else
                {
                    siteIe.ImportChannelsAndContents(filePath, true, isOverride, parentId, _adminName);
                }
            }
        }

        public void ImportContentsByZipFile(ChannelInfo nodeInfo, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var tableName = ChannelManager.GetTableName(_siteInfo, nodeInfo);

            var taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, nodeInfo.Id, false);

            ImportContents(nodeInfo, siteContentDirectoryPath, isOverride, taxis, importStart, importCount, isChecked, checkedLevel);
        }

        public void ImportContentsByZipFile(ChannelInfo nodeInfo, string zipFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.ExtractZip(zipFilePath, siteContentDirectoryPath);

            var tableName = ChannelManager.GetTableName(_siteInfo, nodeInfo);

            var taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, nodeInfo.Id, false);

            ImportContents(nodeInfo, siteContentDirectoryPath, isOverride, taxis, isChecked, checkedLevel, adminId, userId, sourceId);
        }

        public void ImportContentsByAccessFile(int channelId, string excelFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var channelInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, channelId);
            var contentInfoList = AccessObject.GetContentsByAccessFile(excelFilePath, _siteInfo, channelInfo);

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<ContentInfo>();

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

            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;

                //contentInfo.ID = DataProvider.ContentDAO.Insert(tableName, this.FSO.SiteInfo, contentInfo);
                if (isOverride)
                {
                    var existsIDs = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                }
            }
        }

        public void ImportContentsByCsvFile(int channelId, string csvFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var channelInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, channelId);
            var contentInfoList = ExcelObject.GetContentsByCsvFile(csvFilePath, _siteInfo, channelInfo);
            contentInfoList.Reverse();

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<ContentInfo>();

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

            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                if (isOverride)
                {
                    var existsIds = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                    if (existsIds.Count > 0)
                    {
                        foreach (var id in existsIds)
                        {
                            contentInfo.Id = id;
                            DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                }
                //this.FSO.AddContentToWaitingCreate(contentInfo.ChannelId, contentID);
            }
        }

        public void ImportContentsByCsvFile(ChannelInfo channelInfo, string csvFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var contentInfoList = ExcelObject.GetContentsByCsvFile(csvFilePath, _siteInfo, channelInfo);
            contentInfoList.Reverse();

            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                contentInfo.AddDate = DateTime.Now;
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
                            DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                }
            }
        }

        public void ImportContentsByTxtZipFile(int channelId, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.ExtractZip(zipFilePath, directoryPath);

            var channelInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, channelId);

            var contentInfoList = TxtObject.GetContentListByTxtFile(directoryPath, _siteInfo, channelInfo);

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<ContentInfo>();

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

            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;

                //int contentID = DataProvider.ContentDAO.Insert(tableName, this.FSO.SiteInfo, contentInfo);

                if (isOverride)
                {
                    var existsIDs = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                }

                //this.FSO.AddContentToWaitingCreate(contentInfo.ChannelId, contentID);
            }
        }

        public void ImportContentsByTxtFile(ChannelInfo channelInfo, string txtFilePath, bool isOverride, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

            var contentInfo = new ContentInfo
            {
                SiteId = channelInfo.SiteId,
                ChannelId = channelInfo.Id,
                Title = PathUtils.GetFileNameWithoutExtension(txtFilePath),
                Content = StringUtils.ReplaceNewlineToBr(FileUtils.ReadText(txtFilePath, Encoding.UTF8)),
                IsChecked = isChecked,
                CheckedLevel = checkedLevel,
                AddDate = DateTime.Now,
                LastEditDate = DateTime.Now,
                AdminId = adminId,
                UserId = userId,
                SourceId = sourceId
            };

            if (isOverride)
            {
                var existsIDs = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                if (existsIDs.Count > 0)
                {
                    foreach (var id in existsIDs)
                    {
                        contentInfo.Id = id;
                        DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
                }
            }
            else
            {
                contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _siteInfo, channelInfo, contentInfo);
            }
        }

        public void ImportContents(ChannelInfo nodeInfo, string siteContentDirectoryPath, bool isOverride, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");

            var contentIe = new ContentIe(_siteInfo, siteContentDirectoryPath);

            contentIe.ImportContents(filePath, isOverride, nodeInfo, taxis, importStart, importCount, isChecked, checkedLevel, _adminName);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, _sitePath, isOverride);
        }

        public void ImportContents(ChannelInfo nodeInfo, string siteContentDirectoryPath, bool isOverride, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");

            var contentIe = new ContentIe(_siteInfo, siteContentDirectoryPath);

            contentIe.ImportContents(filePath, isOverride, nodeInfo, taxis, isChecked, checkedLevel, adminId, userId, sourceId);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, _sitePath, isOverride);
        }

        //public void ImportInputContentsByCsvFile(InputInfo inputInfo, string excelFilePath, int importStart, int importCount, bool isChecked)
        //{
        //    var contentInfoList = ExcelObject.GetInputContentsByCsvFile(excelFilePath, _siteInfo, inputInfo);
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
