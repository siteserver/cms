using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Atom.Core;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.ImportExport.Components;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.ImportExport
{
    public class ImportObject
    {
        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly string _publishmentSystemPath;

        public ImportObject(int publishmentSystemId)
        {
            _publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            _publishmentSystemPath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, _publishmentSystemInfo.PublishmentSystemDir);
        }

        //获取保存辅助表名称对应集合数据库缓存键
        private string GetTableNameNameValueCollectionDbCacheKey()
        {
            return "SiteServer.CMS.Core.ImportObject.TableNameNameValueCollection_" + _publishmentSystemInfo.PublishmentSystemId;
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
            //if (this.FSO.IsHeadquarters)
            //{
            //    string[] filePaths = DirectoryUtils.GetFilePaths(siteTemplatePath);
            //    foreach (string filePath in filePaths)
            //    {
            //        string fileName = PathUtils.GetFileName(filePath);
            //        if (!PathUtility.IsSystemFile(fileName))
            //        {
            //            string destFilePath = PathUtils.Combine(FSO.PublishmentSystemPath, fileName);
            //            FileUtils.MoveFile(filePath, destFilePath, isOverride);
            //        }
            //    }

            //    ArrayList publishmentSystemDirArrayList = DataProvider.PublishmentSystemDAO.GetLowerPublishmentSystemDirArrayListThatNotIsHeadquarters();

            //    string[] directoryPaths = DirectoryUtils.GetDirectoryPaths(siteTemplatePath);
            //    foreach (string subDirectoryPath in directoryPaths)
            //    {
            //        string directoryName = PathUtils.GetDirectoryName(subDirectoryPath);
            //        if (!PathUtility.IsSystemDirectory(directoryName) && !publishmentSystemDirArrayList.Contains(directoryName.ToLower()))
            //        {
            //            string destDirectoryPath = PathUtils.Combine(FSO.PublishmentSystemPath, directoryName);
            //            DirectoryUtils.MoveDirectory(subDirectoryPath, destDirectoryPath, isOverride);
            //        }
            //    }
            //}
            //else
            //{
            //    DirectoryUtils.MoveDirectory(siteTemplatePath, FSO.PublishmentSystemPath, isOverride);
            //}
            //string siteTemplateMetadataPath = PathUtils.Combine(FSO.PublishmentSystemPath, DirectoryUtility.SiteTemplates.SiteTemplateMetadata);
            //DirectoryUtils.DeleteDirectoryIfExists(siteTemplateMetadataPath);
            DirectoryUtility.ImportPublishmentSystemFiles(_publishmentSystemInfo, siteTemplatePath, isOverride);
        }

        public void ImportSiteContent(string siteContentDirectoryPath, string filePath, bool isImportContents)
        {
            var siteContentIe = new SiteContentIe(_publishmentSystemInfo, siteContentDirectoryPath);
            siteContentIe.ImportChannelsAndContents(filePath, isImportContents, false, 0);
        }


        /// <summary>
        /// 从指定的地址导入网站模板至站点中
        /// </summary>
        public void ImportTemplates(string filePath, bool overwrite, string administratorName)
        {
            var templateIe = new TemplateIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            templateIe.ImportTemplates(overwrite, administratorName);
        }

        /// <summary>
        /// 从指定的地址导入网站菜单显示方式至站点中
        /// </summary>
        /// <param name="filePath">指定的导入地址</param>
        /// <param name="overwrite">是否覆盖原有菜单显示方式</param>
        public void ImportMenuDisplay(string filePath, bool overwrite)
        {
            var menuDisplayIe = new MenuDisplayIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            menuDisplayIe.ImportMenuDisplay(overwrite);
        }

        public void ImportTagStyle(string filePath, bool overwrite)
        {
            var tagStyleIe = new TagStyleIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            tagStyleIe.ImportTagStyle(overwrite);
        }


        /// <summary>
        /// 从指定的地址导入固定广告至站点中
        /// </summary>
        /// <param name="filePath">指定的导入地址</param>
        /// <param name="overwrite">是否覆盖原有固定广告</param>
        public void ImportAd(string filePath, bool overwrite)
        {
            var adIe = new AdvIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            adIe.ImportAd(overwrite);
        }

        /// <summary>
        /// 从指定的地址导入搜索引擎站点中
        /// </summary>
        /// <param name="filePath">指定的导入地址</param>
        /// <param name="overwrite">是否覆盖原有搜索引擎</param>
        public void ImportSeo(string filePath, bool overwrite)
        {
            var seoIe = new SeoIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            seoIe.ImportSeo(overwrite);
        }

        /// <summary>
        /// 从指定的地址导入自定义模板语言站点中
        /// </summary>
        /// <param name="filePath">指定的导入地址</param>
        /// <param name="overwrite">是否覆盖原有自定义模板语言</param>
        public void ImportStlTag(string filePath, bool overwrite)
        {
            var stlTagIe = new StlTagIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            stlTagIe.ImportStlTag(overwrite);
        }

        /// <summary>
        /// 从指定的地址导入采集规则至站点中
        /// </summary>
        /// <param name="filePath">指定的导入地址</param>
        /// <param name="overwrite">是否覆盖原有菜单显示方式</param>
        public void ImportGatherRule(string filePath, bool overwrite)
        {
            var gatherRuleIe = new GatherRuleIe(_publishmentSystemInfo.PublishmentSystemId, filePath);
            gatherRuleIe.ImportGatherRule(overwrite);
        }


        public void ImportInput(string inputDirectoryPath, bool overwrite)
        {
            if (DirectoryUtils.IsDirectoryExists(inputDirectoryPath))
            {
                var inputIe = new InputIe(_publishmentSystemInfo.PublishmentSystemId, inputDirectoryPath);
                inputIe.ImportInput(overwrite);
            }
        }

        public void ImportInputByZipFile(string zipFilePath, bool overwrite)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("Input");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.UnpackFiles(zipFilePath, directoryPath);

            var inputIe = new InputIe(_publishmentSystemInfo.PublishmentSystemId, directoryPath);
            inputIe.ImportInput(overwrite);
        }

        public void ImportRelatedFieldByZipFile(string zipFilePath, bool overwrite)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("RelatedField");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.UnpackFiles(zipFilePath, directoryPath);

            var relatedFieldIe = new RelatedFieldIe(_publishmentSystemInfo.PublishmentSystemId, directoryPath);
            relatedFieldIe.ImportRelatedField(overwrite);
        }

        public void ImportAuxiliaryTables(string tableDirectoryPath, bool isUseTables)
        {
            NameValueCollection nameValueCollection = null;
            if (DirectoryUtils.IsDirectoryExists(tableDirectoryPath))
            {
                var tableIe = new AuxiliaryTableIe(tableDirectoryPath);
                nameValueCollection = tableIe.ImportAuxiliaryTables(_publishmentSystemInfo.PublishmentSystemId, isUseTables);
            }
            SaveTableNameCache(nameValueCollection);
        }


        public void ImportTableStyles(string tableDirectoryPath)
        {
            if (DirectoryUtils.IsDirectoryExists(tableDirectoryPath))
            {
                var tableStyleIe = new TableStyleIe(tableDirectoryPath);
                tableStyleIe.ImportTableStyles(_publishmentSystemInfo.PublishmentSystemId);
            }
        }

        public static void ImportTableStyleByZipFile(ETableStyle tableStyle, string tableName, int nodeId, string zipFilePath)
        {
            var styleDirectoryPath = PathUtils.GetTemporaryFilesPath("TableStyle");
            DirectoryUtils.DeleteDirectoryIfExists(styleDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(styleDirectoryPath);

            ZipUtils.UnpackFiles(zipFilePath, styleDirectoryPath);

            TableStyleIe.SingleImportTableStyle(tableStyle, tableName, styleDirectoryPath, nodeId);
        }

        public void ImportConfiguration(string configurationFilePath)
        {
            var configIe = new ConfigurationIe(_publishmentSystemInfo.PublishmentSystemId, configurationFilePath);
            configIe.Import();
        }


        public void ImportChannelsAndContentsByZipFile(int parentId, string zipFilePath, bool isOverride)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath(EBackupTypeUtils.GetValue(EBackupType.ChannelsAndContents));
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.UnpackFiles(zipFilePath, siteContentDirectoryPath);

            ImportChannelsAndContentsFromZip(parentId, siteContentDirectoryPath, isOverride);

            DataProvider.NodeDao.UpdateContentNum(_publishmentSystemInfo);

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
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, imageUploadDirectoryPath), PathUtils.Combine(_publishmentSystemPath, _publishmentSystemInfo.Additional.ImageUploadDirectoryName), isOverride); 
                }
                string videoUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "VideoUploadDirectoryName");
                if (videoUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, videoUploadDirectoryPath), PathUtils.Combine(_publishmentSystemPath, _publishmentSystemInfo.Additional.VideoUploadDirectoryName), isOverride);
                }
                string fileUploadDirectoryPath = AtomUtility.GetDcElementContent(entry.AdditionalElements, "FileUploadDirectoryName");
                if (fileUploadDirectoryPath != null)
                {
                    DirectoryUtils.MoveDirectory(PathUtils.Combine(siteContentDirectoryPath, fileUploadDirectoryPath), PathUtils.Combine(_publishmentSystemPath, _publishmentSystemInfo.Additional.FileUploadDirectoryName), isOverride);
                }
            }
        }

        public void ImportChannelsAndContentsFromZip(int parentId, string siteContentDirectoryPath, bool isOverride)
        {
            var filePathArrayList = GetSiteContentFilePathArrayList(siteContentDirectoryPath);

            var siteContentIe = new SiteContentIe(_publishmentSystemInfo, siteContentDirectoryPath);

            Hashtable levelHashtable = null;
            foreach (string filePath in filePathArrayList)
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

                var insertNodeId = siteContentIe.ImportChannelsAndContents(filePath, true, isOverride, (int)levelHashtable[level]);
                levelHashtable[level + 1] = insertNodeId;
            }
        }

        public void ImportChannelsAndContents(int parentId, string siteContentDirectoryPath, bool isOverride)
        {
            var filePathArrayList = GetSiteContentFilePathArrayList(siteContentDirectoryPath);

            var siteContentIe = new SiteContentIe(_publishmentSystemInfo, siteContentDirectoryPath);

            var parentOrderString = "none";
            //int parentID = 0;
            foreach (string filePath in filePathArrayList)
            {
                var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
                var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
                var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

                if (StringUtils.StartsWithIgnoreCase(orderString, parentOrderString))
                {
                    parentId = siteContentIe.ImportChannelsAndContents(filePath, true, isOverride, parentId);
                    parentOrderString = orderString;
                }
                else
                {
                    siteContentIe.ImportChannelsAndContents(filePath, true, isOverride, parentId);
                }
            }
        }

        public void ImportContentsByZipFile(NodeInfo nodeInfo, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var siteContentDirectoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(siteContentDirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(siteContentDirectoryPath);

            ZipUtils.UnpackFiles(zipFilePath, siteContentDirectoryPath);

            var tableName = NodeManager.GetTableName(_publishmentSystemInfo, nodeInfo);

            var taxis = BaiRongDataProvider.ContentDao.GetMaxTaxis(tableName, nodeInfo.NodeId, false);

            ImportContents(nodeInfo, siteContentDirectoryPath, isOverride, taxis, importStart, importCount, isChecked, checkedLevel);

            DataProvider.NodeDao.UpdateContentNum(_publishmentSystemInfo);
        }

        public void ImportContentsByAccessFile(int nodeId, string excelFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var nodeInfo = NodeManager.GetNodeInfo(_publishmentSystemInfo.PublishmentSystemId, nodeId);
            var contentInfoArrayList = AccessObject.GetContentsByAccessFile(excelFilePath, _publishmentSystemInfo, nodeInfo);

            if (importStart > 1 || importCount > 0)
            {
                var theArrayList = new ArrayList();

                if (importStart == 0)
                {
                    importStart = 1;
                }
                if (importCount == 0)
                {
                    importCount = contentInfoArrayList.Count;
                }

                var firstIndex = contentInfoArrayList.Count - importStart - importCount + 1;
                if (firstIndex <= 0)
                {
                    firstIndex = 0;
                }

                var addCount = 0;
                for (var i = 0; i < contentInfoArrayList.Count; i++)
                {
                    if (addCount >= importCount) break;
                    if (i >= firstIndex)
                    {
                        theArrayList.Add(contentInfoArrayList[i]);
                        addCount++;
                    }
                }

                contentInfoArrayList = theArrayList;
            }

            var tableName = NodeManager.GetTableName(_publishmentSystemInfo, nodeInfo);

            foreach (BackgroundContentInfo contentInfo in contentInfoArrayList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;

                //contentInfo.ID = DataProvider.ContentDAO.Insert(tableName, this.FSO.PublishmentSystemInfo, contentInfo);
                if (isOverride)
                {
                    var existsIDs = DataProvider.ContentDao.GetIdListBySameTitleInOneNode(tableName, contentInfo.NodeId, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            DataProvider.ContentDao.Update(tableName, _publishmentSystemInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo);
                }
            }
        }

        public void ImportContentsByCsvFile(int nodeId, string csvFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var nodeInfo = NodeManager.GetNodeInfo(_publishmentSystemInfo.PublishmentSystemId, nodeId);
            var contentInfoList = ExcelObject.GetContentsByCsvFile(csvFilePath, _publishmentSystemInfo, nodeInfo);
            contentInfoList.Reverse();

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<BackgroundContentInfo>();

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

            var tableName = NodeManager.GetTableName(_publishmentSystemInfo, nodeInfo);

            foreach (var contentInfo in contentInfoList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;
                if (isOverride)
                {
                    var existsIDs = DataProvider.ContentDao.GetIdListBySameTitleInOneNode(tableName, contentInfo.NodeId, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            DataProvider.ContentDao.Update(tableName, _publishmentSystemInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo);
                }
                //this.FSO.AddContentToWaitingCreate(contentInfo.NodeID, contentID);
            }
        }

        public void ImportContentsByTxtZipFile(int nodeId, string zipFilePath, bool isOverride, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var directoryPath = PathUtils.GetTemporaryFilesPath("contents");
            DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);

            ZipUtils.UnpackFiles(zipFilePath, directoryPath);

            var nodeInfo = NodeManager.GetNodeInfo(_publishmentSystemInfo.PublishmentSystemId, nodeId);

            var contentInfoArrayList = TxtObject.GetContentsByTxtFile(directoryPath, _publishmentSystemInfo, nodeInfo);

            if (importStart > 1 || importCount > 0)
            {
                var theArrayList = new ArrayList();

                if (importStart == 0)
                {
                    importStart = 1;
                }
                if (importCount == 0)
                {
                    importCount = contentInfoArrayList.Count;
                }

                var firstIndex = contentInfoArrayList.Count - importStart - importCount + 1;
                if (firstIndex <= 0)
                {
                    firstIndex = 0;
                }

                var addCount = 0;
                for (var i = 0; i < contentInfoArrayList.Count; i++)
                {
                    if (addCount >= importCount) break;
                    if (i >= firstIndex)
                    {
                        theArrayList.Add(contentInfoArrayList[i]);
                        addCount++;
                    }
                }

                contentInfoArrayList = theArrayList;
            }

            var tableName = NodeManager.GetTableName(_publishmentSystemInfo, nodeInfo);

            foreach (BackgroundContentInfo contentInfo in contentInfoArrayList)
            {
                contentInfo.IsChecked = isChecked;
                contentInfo.CheckedLevel = checkedLevel;

                //int contentID = DataProvider.ContentDAO.Insert(tableName, this.FSO.PublishmentSystemInfo, contentInfo);

                if (isOverride)
                {
                    var existsIDs = DataProvider.ContentDao.GetIdListBySameTitleInOneNode(tableName, contentInfo.NodeId, contentInfo.Title);
                    if (existsIDs.Count > 0)
                    {
                        foreach (int id in existsIDs)
                        {
                            contentInfo.Id = id;
                            DataProvider.ContentDao.Update(tableName, _publishmentSystemInfo, contentInfo);
                        }
                    }
                    else
                    {
                        contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo);
                    }
                }
                else
                {
                    contentInfo.Id = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo);
                }

                //this.FSO.AddContentToWaitingCreate(contentInfo.NodeID, contentID);
            }
        }

        public void ImportContents(NodeInfo nodeInfo, string siteContentDirectoryPath, bool isOverride, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            var filePath = PathUtils.Combine(siteContentDirectoryPath, "contents.xml");

            var siteContentIe = new SiteContentIe(_publishmentSystemInfo, siteContentDirectoryPath);

            siteContentIe.ImportContents(filePath, isOverride, nodeInfo, taxis, importStart, importCount, isChecked, checkedLevel);

            FileUtils.DeleteFileIfExists(filePath);

            DirectoryUtils.MoveDirectory(siteContentDirectoryPath, _publishmentSystemPath, isOverride);
        }

        public void ImportInputContentsByCsvFile(InputInfo inputInfo, string excelFilePath, int importStart, int importCount, bool isChecked)
        {
            var contentInfoList = ExcelObject.GetInputContentsByCsvFile(excelFilePath, _publishmentSystemInfo, inputInfo);
            contentInfoList.Reverse();

            if (importStart > 1 || importCount > 0)
            {
                var theList = new List<InputContentInfo>();

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
                contentInfo.IsChecked = isChecked;
                DataProvider.InputContentDao.Insert(contentInfo);
            }
        }

        public static ArrayList GetSiteContentFilePathArrayList(string siteContentDirectoryPath)
        {
            var filePaths = DirectoryUtils.GetFilePaths(siteContentDirectoryPath);
            var filePathSortedList = new SortedList();
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
            var filePathArrayList = new ArrayList(filePathSortedList.Values);
            return filePathArrayList;
        }

    }
}
