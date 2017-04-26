using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Atom.AdditionalElements;
using Atom.Core;
using Atom.Core.Collections;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class SiteContentIe
    {
        readonly PublishmentSystemInfo _publishmentSystemInfo;
        readonly FileSystemObject _fso;

        //保存除内容表本身字段外的属性
        private const string ChannelTemplateName = "ChannelTemplateName";
        private const string ContentTemplateName = "ContentTemplateName";

        private readonly string _siteContentDirectoryPath;
        private readonly PhotoIe _photoIe;

        public SiteContentIe(PublishmentSystemInfo publishmentSystemInfo, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _publishmentSystemInfo = publishmentSystemInfo;
            _fso = new FileSystemObject(_publishmentSystemInfo.PublishmentSystemId);

            var photoDirectoryPath = PathUtils.Combine(siteContentDirectoryPath, DirectoryUtils.SiteTemplates.Photo);
            DirectoryUtils.CreateDirectoryIfNotExists(photoDirectoryPath);
            _photoIe = new PhotoIe(_publishmentSystemInfo, photoDirectoryPath);
        }

        public int ImportChannelsAndContents(string filePath, bool isImportContents, bool isOverride, int theParentId)
        {
            var psChildCount = DataProvider.NodeDao.GetNodeCount(_publishmentSystemInfo.PublishmentSystemId);
            var nodeIndexNameList = DataProvider.NodeDao.GetNodeIndexNameList(_publishmentSystemInfo.PublishmentSystemId);

            if (!FileUtils.IsFileExists(filePath)) return 0;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            var firstIndex = filePath.LastIndexOf(PathUtils.SeparatorChar) + 1;
            var lastIndex = filePath.LastIndexOf(".", StringComparison.Ordinal);
            var orderString = filePath.Substring(firstIndex, lastIndex - firstIndex);

            var idx = orderString.IndexOf("_", StringComparison.Ordinal);
            if (idx != -1)
            {
                var secondOrder = TranslateUtils.ToInt(orderString.Split('_')[1]);
                secondOrder = secondOrder + psChildCount;
                orderString = orderString.Substring(idx + 1);
                idx = orderString.IndexOf("_", StringComparison.Ordinal);
                if (idx != -1)
                {
                    orderString = orderString.Substring(idx);
                    orderString = "1_" + secondOrder + orderString;
                }
                else
                {
                    orderString = "1_" + secondOrder;
                }

                orderString = orderString.Substring(0, orderString.LastIndexOf("_", StringComparison.Ordinal));
            }

            var parentId = DataProvider.NodeDao.GetNodeId(_publishmentSystemInfo.PublishmentSystemId, orderString);
            if (theParentId != 0)
            {
                parentId = theParentId;
            }

            var nodeType = ENodeTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, NodeAttribute.NodeType));
            int nodeId;
            if (nodeType == ENodeType.BackgroundPublishNode)
            {
                nodeId = _publishmentSystemInfo.PublishmentSystemId;
                var nodeInfo = NodeManager.GetNodeInfo(_publishmentSystemInfo.PublishmentSystemId, _publishmentSystemInfo.PublishmentSystemId);
                ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, nodeIndexNameList);

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                if (isImportContents)
                {
                    ImportContents(feed.Entries, nodeInfo, 0, isOverride);
                }
            }
            else
            {
                var nodeInfo = new NodeInfo();
                ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, nodeIndexNameList);
                if (string.IsNullOrEmpty(nodeInfo.NodeName)) return 0;

                var isUpdate = false;
                var theSameNameNodeId = 0;
                if (isOverride)
                {
                    theSameNameNodeId = DataProvider.NodeDao.GetNodeIdByParentIdAndNodeName(_publishmentSystemInfo.PublishmentSystemId, parentId, nodeInfo.NodeName, false);
                    if (theSameNameNodeId != 0)
                    {
                        isUpdate = true;
                    }
                }
                if (!isUpdate)
                {
                    //BackgroundNodeInfo backgroundNodeInfo = new BackgroundNodeInfo();
                    //this.ImportBackgroundNodeInfo(backgroundNodeInfo, feed.AdditionalElements);

                    nodeId = DataProvider.NodeDao.InsertNodeInfo(nodeInfo);
                }
                else
                {
                    nodeId = theSameNameNodeId;
                    nodeInfo = NodeManager.GetNodeInfo(_publishmentSystemInfo.PublishmentSystemId, theSameNameNodeId);
                    var tableName = NodeManager.GetTableName(_publishmentSystemInfo, nodeInfo);
                    ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, nodeIndexNameList);

                    DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                    DataProvider.ContentDao.DeleteContentsByNodeId(_publishmentSystemInfo.PublishmentSystemId, tableName, theSameNameNodeId);
                }

                if (isImportContents)
                {
                    ImportContents(feed.Entries, nodeInfo, 0, isOverride);
                }
            }

            //this.FSO.CreateRedirectChannel(nodeID);
            //this.FSO.AddChannelToWaitingCreate(nodeID);
            return nodeId;
        }

        public void ImportContents(string filePath, bool isOverride, NodeInfo nodeInfo, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            ImportContents(feed.Entries, nodeInfo, taxis, importStart, importCount, false, isChecked, checkedLevel, isOverride);

            //this.FSO.CreateRedirectChannel(nodeID);
            //this.FSO.AddChannelToWaitingCreate(nodeInfo.NodeID);
        }

        private void ImportContents(AtomEntryCollection entries, NodeInfo nodeInfo, int taxis, bool isOverride)
        {
            ImportContents(entries, nodeInfo, taxis, 0, 0, true, true, 0, isOverride);
        }

        // 内部消化掉错误
        private void ImportContents(AtomEntryCollection entries, NodeInfo nodeInfo, int taxis, int importStart, int importCount, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride)
        {
            if (importStart > 1 || importCount > 0)
            {
                var theEntries = new AtomEntryCollection();

                if (importStart == 0)
                {
                    importStart = 1;
                }
                if (importCount == 0)
                {
                    importCount = entries.Count;
                }

                var firstIndex = entries.Count - importStart - importCount + 1;
                if (firstIndex <= 0)
                {
                    firstIndex = 0;
                }

                var addCount = 0;
                for (var i = 0; i < entries.Count; i++)
                {
                    if (addCount >= importCount) break;
                    if (i >= firstIndex)
                    {
                        theEntries.Add(entries[i]);
                        addCount++;
                    }
                }

                entries = theEntries;
            }

            var tableName = NodeManager.GetTableName(_publishmentSystemInfo, nodeInfo);

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var contentIdFromFile = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Id));
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var contentGroupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.ContentGroupNameCollection);
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsChecked));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.CheckedLevel));
                    }
                    var comments = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Comments));
                    var photos = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Photos));
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Hits));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByDay));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByWeek));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByMonth));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastHitsDate);
                    var title = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Title);
                    var isTop = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsTop));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.AddDate);

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = BaiRongDataProvider.ContentDao.GetMaxTaxis(tableName, nodeInfo.NodeId, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var starSetting = AtomUtility.GetDcElementContent(entry.AdditionalElements, BackgroundContentAttribute.StarSetting);

                    var contentInfo = new ContentInfo
                    {
                        NodeId = nodeInfo.NodeId,
                        PublishmentSystemId = _publishmentSystemInfo.PublishmentSystemId,
                        AddUserName = RequestBody.CurrentAdministratorName,
                        AddDate = TranslateUtils.ToDateTime(addDate)
                    };
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = TranslateUtils.ToDateTime(lastEditDate);
                    contentInfo.ContentGroupNameCollection = contentGroupNameCollection;
                    contentInfo.Tags = tags;
                    contentInfo.IsChecked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;
                    contentInfo.Comments = comments;
                    contentInfo.Photos = photos;
                    contentInfo.Hits = hits;
                    contentInfo.HitsByDay = hitsByDay;
                    contentInfo.HitsByWeek = hitsByWeek;
                    contentInfo.HitsByMonth = hitsByMonth;
                    contentInfo.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    contentInfo.Title = AtomUtility.Decrypt(title);
                    contentInfo.IsTop = isTop;

                    var attributes = AtomUtility.GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string attributeName in attributes.Keys)
                    {
                        if (!contentInfo.ContainsKey(attributeName.ToLower()))
                        {
                            contentInfo.Attributes[attributeName] = AtomUtility.Decrypt(attributes[attributeName]);
                        }
                    }

                    var isInsert = false;
                    if (isOverride)
                    {
                        var existsIDs = DataProvider.ContentDao.GetIdListBySameTitleInOneNode(tableName, contentInfo.NodeId, contentInfo.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (int id in existsIDs)
                            {
                                contentInfo.Id = id;
                                DataProvider.ContentDao.Update(tableName, _fso.PublishmentSystemInfo, contentInfo);
                            }
                        }
                        else
                        {
                            isInsert = true;
                        }
                    }
                    else
                    {
                        isInsert = true;
                    }

                    if (isInsert)
                    {
                        var contentId = DataProvider.ContentDao.Insert(tableName, _publishmentSystemInfo, contentInfo, false, taxis);
                        if (photos > 0)
                        {
                            _photoIe.ImportPhoto(contentIdFromFile, contentId);
                        }

                        if (_publishmentSystemInfo.Additional.IsRelatedByTags && !string.IsNullOrEmpty(tags))
                        {
                            var tagCollection = TagUtils.ParseTagsString(tags);
                            TagUtils.AddTags(tagCollection, _publishmentSystemInfo.PublishmentSystemId, contentId);
                        }

                        if (!string.IsNullOrEmpty(starSetting))
                        {
                            var settings = starSetting.Split('_');
                            if (settings != null && settings.Length == 2)
                            {
                                var totalCount = TranslateUtils.ToInt(settings[0]);
                                var pointAverage = TranslateUtils.ToDecimal(settings[1]);
                                StarsManager.SetStarSetting(_publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentId, totalCount, pointAverage);
                            }
                        }
                    }
                    //this.FSO.AddContentToWaitingCreate(contentInfo.NodeID, contentID);

                    if (isTop)
                    {
                        taxis = topTaxis;
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }


        private void ImportNodeInfo(NodeInfo nodeInfo, ScopedElementCollection additionalElements, int parentId, IList nodeIndexNameArrayList)
        {
            nodeInfo.NodeName = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.NodeName);
            nodeInfo.NodeType = ENodeTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.NodeType));
            nodeInfo.PublishmentSystemId = _publishmentSystemInfo.PublishmentSystemId;
            var contentModelId = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.ContentModelId);
            if (!string.IsNullOrEmpty(contentModelId))
            {
                nodeInfo.ContentModelId = contentModelId;
            }
            nodeInfo.ParentId = parentId;
            var nodeIndexName = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.NodeIndexName);
            if (!string.IsNullOrEmpty(nodeIndexName) && nodeIndexNameArrayList.IndexOf(nodeIndexName) == -1)
            {
                nodeInfo.NodeIndexName = nodeIndexName;
                nodeIndexNameArrayList.Add(nodeIndexName);
            }
            nodeInfo.NodeGroupNameCollection = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.NodeGroupNameCollection);
            nodeInfo.AddDate = DateTime.Now;
            nodeInfo.ImageUrl = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.ImageUrl);
            nodeInfo.Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.Content));
            nodeInfo.FilePath = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.FilePath);
            nodeInfo.ChannelFilePathRule = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.ChannelFilePathRule);
            nodeInfo.ContentFilePathRule = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.ContentFilePathRule);

            nodeInfo.LinkUrl = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.LinkUrl);
            nodeInfo.LinkType = ELinkTypeUtils.GetEnumType(AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.LinkType));

            var channelTemplateName = AtomUtility.GetDcElementContent(additionalElements, ChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                nodeInfo.ChannelTemplateId = TemplateManager.GetTemplateIDByTemplateName(_publishmentSystemInfo.PublishmentSystemId, ETemplateType.ChannelTemplate, channelTemplateName);
            }
            var contentTemplateName = AtomUtility.GetDcElementContent(additionalElements, ContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                nodeInfo.ContentTemplateId = TemplateManager.GetTemplateIDByTemplateName(_publishmentSystemInfo.PublishmentSystemId, ETemplateType.ContentTemplate, contentTemplateName);
            }

            nodeInfo.Keywords = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.Keywords);
            nodeInfo.Description = AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.Description);

            nodeInfo.SetExtendValues(AtomUtility.GetDcElementContent(additionalElements, NodeAttribute.ExtendValues));
        }


        /// <summary>
        /// 导出栏目及栏目下内容至XML文件
        /// </summary>
        /// <returns></returns>
        public void Export(int publishmentSystemId, int nodeId, bool isSaveContents)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo == null) return;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var fileName = DataProvider.NodeDao.GetOrderStringInPublishmentSystem(nodeId);

            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + fileName + ".xml";

            var feed = ExportNodeInfo(nodeInfo);

            if (isSaveContents)
            {
                var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxis);
                var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, nodeId, orderByString);
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                    //ContentUtility.PutImagePaths(publishmentSystemInfo, contentInfo as BackgroundContentInfo, collection);
                    var entry = ExportContentInfo(contentInfo);
                    feed.Entries.Add(entry);

                }
            }
            feed.Save(filePath);

            //  foreach (string imageUrl in collection.Keys)
            //  {
            //     string sourceFilePath = collection[imageUrl];
            //     string destFilePath = PathUtility.MapPath(this.siteContentDirectoryPath, imageUrl);
            //     DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);
            //     FileUtils.MoveFile(sourceFilePath, destFilePath, true);
            //  }
        }

        public bool ExportContents(PublishmentSystemInfo publishmentSystemInfo, int nodeId, List<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            var feed = AtomUtility.GetEmptyFeed();

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(tableName, nodeId, isPeriods, dateFrom, dateTo, checkedState);
            }
            if (contentIdList.Count == 0) return false;

            var collection = new NameValueCollection();

            foreach (var contentId in contentIdList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                try
                {
                    ContentUtility.PutImagePaths(publishmentSystemInfo, contentInfo as BackgroundContentInfo, collection);
                }
                catch
                {
                    // ignored
                }
                var entry = ExportContentInfo(contentInfo);
                feed.Entries.Add(entry);
            }
            feed.Save(filePath);

            foreach (string imageUrl in collection.Keys)
            {
                var sourceFilePath = collection[imageUrl];
                var destFilePath = PathUtility.MapPath(_siteContentDirectoryPath, imageUrl);
                DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);
                FileUtils.MoveFile(sourceFilePath, destFilePath, true);
            }

            return true;
        }

        private AtomFeed ExportNodeInfo(NodeInfo nodeInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.NodeId, nodeInfo.NodeId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.NodeName, nodeInfo.NodeName);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.NodeType, ENodeTypeUtils.GetValue(nodeInfo.NodeType));
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.PublishmentSystemId, nodeInfo.PublishmentSystemId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ContentModelId, nodeInfo.ContentModelId);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ParentId, nodeInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ParentsPath, nodeInfo.ParentsPath);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ParentsCount, nodeInfo.ParentsCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ChildrenCount, nodeInfo.ChildrenCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.IsLastNode, nodeInfo.IsLastNode.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.NodeIndexName, nodeInfo.NodeIndexName);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.NodeGroupNameCollection, nodeInfo.NodeGroupNameCollection);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.Taxis, nodeInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.AddDate, nodeInfo.AddDate.ToLongDateString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ImageUrl, nodeInfo.ImageUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.Content, AtomUtility.Encrypt(nodeInfo.Content));
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ContentNum, nodeInfo.ContentNum.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.FilePath, nodeInfo.FilePath);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ChannelFilePathRule, nodeInfo.ChannelFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ContentFilePathRule, nodeInfo.ContentFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.LinkUrl, nodeInfo.LinkUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.LinkType, ELinkTypeUtils.GetValue(nodeInfo.LinkType));
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ChannelTemplateId, nodeInfo.ChannelTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ContentTemplateId, nodeInfo.ContentTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.Keywords, nodeInfo.Keywords);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.Description, nodeInfo.Description);
            AtomUtility.AddDcElement(feed.AdditionalElements, NodeAttribute.ExtendValues, nodeInfo.Additional.ToString());

            if (nodeInfo.ChannelTemplateId != 0)
            {
                var channelTemplateName = TemplateManager.GetTemplateName(nodeInfo.PublishmentSystemId, nodeInfo.ChannelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ChannelTemplateName, channelTemplateName);
            }

            if (nodeInfo.ContentTemplateId != 0)
            {
                var contentTemplateName = TemplateManager.GetTemplateName(nodeInfo.PublishmentSystemId, nodeInfo.ContentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ContentTemplateName, contentTemplateName);
            }

            return feed;
        }

        private AtomEntry ExportContentInfo(ContentInfo contentInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Id, contentInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.NodeId, contentInfo.NodeId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.PublishmentSystemId, contentInfo.PublishmentSystemId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddUserName, contentInfo.AddUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditUserName, contentInfo.LastEditUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditDate, contentInfo.LastEditDate.ToString(CultureInfo.InvariantCulture));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Taxis, contentInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.ContentGroupNameCollection, contentInfo.ContentGroupNameCollection);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Tags, AtomUtility.Encrypt(contentInfo.Tags));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsChecked, contentInfo.IsChecked.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.CheckedLevel, contentInfo.CheckedLevel.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Comments, contentInfo.Comments.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Photos, contentInfo.Photos.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Hits, contentInfo.Hits.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByDay, contentInfo.HitsByDay.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByWeek, contentInfo.HitsByWeek.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByMonth, contentInfo.HitsByMonth.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastHitsDate, contentInfo.LastHitsDate.ToString(CultureInfo.InvariantCulture));

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Title, AtomUtility.Encrypt(contentInfo.Title));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsTop, contentInfo.IsTop.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddDate, contentInfo.AddDate.ToString(CultureInfo.InvariantCulture));

            var starSetting = StarsManager.GetStarSettingToExport(_publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id);
            AtomUtility.AddDcElement(entry.AdditionalElements, BackgroundContentAttribute.StarSetting, starSetting);

            foreach (string attributeName in contentInfo.Attributes.Keys)
            {
                if (!ContentAttribute.AllAttributes.Contains(attributeName.ToLower()))
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, AtomUtility.Encrypt(contentInfo.Attributes[attributeName]));
                }
            }

            if (contentInfo.Photos > 0)
            {
                _photoIe.ExportPhoto(contentInfo.Id);
            }

            return entry;
        }

    }
}
