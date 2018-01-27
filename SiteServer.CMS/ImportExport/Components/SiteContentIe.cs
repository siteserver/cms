using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Atom.AdditionalElements;
using Atom.Core;
using Atom.Core.Collections;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class SiteContentIe
    {
        private readonly SiteInfo _siteInfo;

        //保存除内容表本身字段外的属性
        private const string ChannelTemplateName = "ChannelTemplateName";
        private const string ContentTemplateName = "ContentTemplateName";

        private readonly string _siteContentDirectoryPath;

        public SiteContentIe(SiteInfo siteInfo, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _siteInfo = siteInfo;
        }

        public int ImportChannelsAndContents(string filePath, bool isImportContents, bool isOverride, int theParentId)
        {
            var psChildCount = DataProvider.ChannelDao.GetCount(_siteInfo.Id);
            var indexNameList = DataProvider.ChannelDao.GetIndexNameList(_siteInfo.Id);

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

            var parentId = DataProvider.ChannelDao.GetId(_siteInfo.Id, orderString);
            if (theParentId != 0)
            {
                parentId = theParentId;
            }

            var parentIdOriginal = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, ChannelAttribute.ParentId));
            int channelId;
            if (parentIdOriginal == 0)
            {
                channelId = _siteInfo.Id;
                var nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, _siteInfo.Id);
                ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, indexNameList);

                DataProvider.ChannelDao.Update(nodeInfo);

                if (isImportContents)
                {
                    ImportContents(feed.Entries, nodeInfo, 0, isOverride);
                }
            }
            else
            {
                var nodeInfo = new ChannelInfo();
                ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, indexNameList);
                if (string.IsNullOrEmpty(nodeInfo.ChannelName)) return 0;

                var isUpdate = false;
                var theSameNameChannelId = 0;
                if (isOverride)
                {
                    theSameNameChannelId = DataProvider.ChannelDao.GetIdByParentIdAndChannelName(_siteInfo.Id, parentId, nodeInfo.ChannelName, false);
                    if (theSameNameChannelId != 0)
                    {
                        isUpdate = true;
                    }
                }
                if (!isUpdate)
                {
                    channelId = DataProvider.ChannelDao.Insert(nodeInfo);
                }
                else
                {
                    channelId = theSameNameChannelId;
                    nodeInfo = ChannelManager.GetChannelInfo(_siteInfo.Id, theSameNameChannelId);
                    var tableName = ChannelManager.GetTableName(_siteInfo, nodeInfo);
                    ImportNodeInfo(nodeInfo, feed.AdditionalElements, parentId, indexNameList);

                    DataProvider.ChannelDao.Update(nodeInfo);

                    DataProvider.ContentDao.DeleteContentsByChannelId(_siteInfo.Id, tableName, theSameNameChannelId);
                }

                if (isImportContents)
                {
                    ImportContents(feed.Entries, nodeInfo, 0, isOverride);
                }
            }

            return channelId;
        }

        public void ImportContents(string filePath, bool isOverride, ChannelInfo nodeInfo, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            ImportContents(feed.Entries, nodeInfo, taxis, importStart, importCount, false, isChecked, checkedLevel, isOverride);
        }

        private void ImportContents(AtomEntryCollection entries, ChannelInfo nodeInfo, int taxis, bool isOverride)
        {
            ImportContents(entries, nodeInfo, taxis, 0, 0, true, true, 0, isOverride);
        }

        // 内部消化掉错误
        private void ImportContents(AtomEntryCollection entries, ChannelInfo nodeInfo, int taxis, int importStart, int importCount, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride)
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

            var tableName = ChannelManager.GetTableName(_siteInfo, nodeInfo);

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string>{ ContentAttribute.GroupNameCollection , "ContentGroupNameCollection" });
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsChecked));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.CheckedLevel));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Hits));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByDay));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByWeek));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByMonth));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastHitsDate);
                    var title = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Title);
                    var isTop = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsTop));
                    var isRecommend = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsRecommend));
                    var isHot = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsHot));
                    var isColor = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.IsColor));
                    var linkUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LinkUrl);
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.AddDate);

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, nodeInfo.Id, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var contentInfo = new ContentInfo
                    {
                        ChannelId= nodeInfo.Id,
                        SiteId = _siteInfo.Id,
                        AddUserName = RequestContext.CurrentAdministratorName,
                        AddDate = TranslateUtils.ToDateTime(addDate)
                    };
                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = TranslateUtils.ToDateTime(lastEditDate);
                    contentInfo.GroupNameCollection = groupNameCollection;
                    contentInfo.Tags = tags;
                    contentInfo.IsChecked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;
                    contentInfo.Hits = hits;
                    contentInfo.HitsByDay = hitsByDay;
                    contentInfo.HitsByWeek = hitsByWeek;
                    contentInfo.HitsByMonth = hitsByMonth;
                    contentInfo.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    contentInfo.Title = AtomUtility.Decrypt(title);
                    contentInfo.IsTop = isTop;
                    contentInfo.IsRecommend = isRecommend;
                    contentInfo.IsHot = isHot;
                    contentInfo.IsColor = isColor;
                    contentInfo.LinkUrl = linkUrl;

                    var attributes = AtomUtility.GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string attributeName in attributes.Keys)
                    {
                        if (!contentInfo.ContainsKey(attributeName.ToLower()))
                        {
                            contentInfo.Set(attributeName, AtomUtility.Decrypt(attributes[attributeName]));
                        }
                    }

                    var isInsert = false;
                    if (isOverride)
                    {
                        var existsIDs = DataProvider.ContentDao.GetIdListBySameTitle(tableName, contentInfo.ChannelId, contentInfo.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (int id in existsIDs)
                            {
                                contentInfo.Id = id;
                                DataProvider.ContentDao.Update(tableName, _siteInfo, contentInfo);
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
                        var contentId = DataProvider.ContentDao.Insert(tableName, _siteInfo, contentInfo, false, taxis);

                        if (!string.IsNullOrEmpty(tags))
                        {
                            var tagCollection = TagUtils.ParseTagsString(tags);
                            TagUtils.AddTags(tagCollection, _siteInfo.Id, contentId);
                        }
                    }

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

        private void ImportNodeInfo(ChannelInfo nodeInfo, ScopedElementCollection additionalElements, int parentId, IList indexNameList)
        {
            nodeInfo.ChannelName = AtomUtility.GetDcElementContent(additionalElements, new List<string>{ ChannelAttribute.ChannelName, "NodeName" });
            nodeInfo.SiteId = _siteInfo.Id;
            var contentModelPluginId = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ContentModelPluginId);
            if (!string.IsNullOrEmpty(contentModelPluginId))
            {
                nodeInfo.ContentModelPluginId = contentModelPluginId;
            }
            var contentRelatedPluginIds = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(contentRelatedPluginIds))
            {
                nodeInfo.ContentRelatedPluginIds = contentRelatedPluginIds;
            }
            nodeInfo.ParentId = parentId;
            var indexName = AtomUtility.GetDcElementContent(additionalElements, new List<string> { ChannelAttribute.IndexName, "NodeIndexName" });
            if (!string.IsNullOrEmpty(indexName) && indexNameList.IndexOf(indexName) == -1)
            {
                nodeInfo.IndexName = indexName;
                indexNameList.Add(indexName);
            }
            nodeInfo.GroupNameCollection = AtomUtility.GetDcElementContent(additionalElements, new List<string> { ChannelAttribute.GroupNameCollection, "NodeGroupNameCollection" });
            nodeInfo.AddDate = DateTime.Now;
            nodeInfo.ImageUrl = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ImageUrl);
            nodeInfo.Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.Content));
            nodeInfo.FilePath = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.FilePath);
            nodeInfo.ChannelFilePathRule = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ChannelFilePathRule);
            nodeInfo.ContentFilePathRule = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ContentFilePathRule);

            nodeInfo.LinkUrl = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.LinkUrl);
            nodeInfo.LinkType = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.LinkType);

            var channelTemplateName = AtomUtility.GetDcElementContent(additionalElements, ChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                nodeInfo.ChannelTemplateId = TemplateManager.GetTemplateIdByTemplateName(_siteInfo.Id, TemplateType.ChannelTemplate, channelTemplateName);
            }
            var contentTemplateName = AtomUtility.GetDcElementContent(additionalElements, ContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                nodeInfo.ContentTemplateId = TemplateManager.GetTemplateIdByTemplateName(_siteInfo.Id, TemplateType.ContentTemplate, contentTemplateName);
            }

            nodeInfo.Keywords = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.Keywords);
            nodeInfo.Description = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.Description);

            nodeInfo.SetExtendValues(AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ExtendValues));
        }


        /// <summary>
        /// 导出栏目及栏目下内容至XML文件
        /// </summary>
        /// <returns></returns>
        public void Export(int siteId, int channelId, bool isSaveContents)
        {
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return;

            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var tableName = ChannelManager.GetTableName(siteInfo, nodeInfo);

            var fileName = DataProvider.ChannelDao.GetOrderStringInSite(channelId);

            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + fileName + ".xml";

            var feed = ExportNodeInfo(nodeInfo);

            if (isSaveContents)
            {
                var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxis);
                var contentIdList = DataProvider.ContentDao.GetContentIdListChecked(tableName, channelId, orderByString);
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                    //ContentUtility.PutImagePaths(siteInfo, contentInfo as BackgroundContentInfo, collection);
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

        public bool ExportContents(SiteInfo siteInfo, int channelId, List<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var tableName = ChannelManager.GetTableName(siteInfo, channelId);
            var feed = AtomUtility.GetEmptyFeed();

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId, isPeriods, dateFrom, dateTo, checkedState);
            }
            if (contentIdList.Count == 0) return false;

            var collection = new NameValueCollection();

            foreach (var contentId in contentIdList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                try
                {
                    ContentUtility.PutImagePaths(siteInfo, contentInfo, collection);
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

        private AtomFeed ExportNodeInfo(ChannelInfo nodeInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ ChannelAttribute.Id, "NodeId" }, nodeInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.ChannelName, "NodeName" }, nodeInfo.ChannelName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.SiteId, "PublishmentSystemId" }, nodeInfo.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentModelPluginId, nodeInfo.ContentModelPluginId);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentRelatedPluginIds, nodeInfo.ContentRelatedPluginIds);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentId, nodeInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentsPath, nodeInfo.ParentsPath);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentsCount, nodeInfo.ParentsCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChildrenCount, nodeInfo.ChildrenCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.IsLastNode, nodeInfo.IsLastNode.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.IndexName, "NodeIndexName" }, nodeInfo.IndexName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.GroupNameCollection, "NodeGroupNameCollection" }, nodeInfo.GroupNameCollection);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Taxis, nodeInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.AddDate, nodeInfo.AddDate.ToLongDateString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ImageUrl, nodeInfo.ImageUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Content, AtomUtility.Encrypt(nodeInfo.Content));
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentNum, nodeInfo.ContentNum.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.FilePath, nodeInfo.FilePath);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChannelFilePathRule, nodeInfo.ChannelFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentFilePathRule, nodeInfo.ContentFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.LinkUrl, nodeInfo.LinkUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.LinkType, nodeInfo.LinkType);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChannelTemplateId, nodeInfo.ChannelTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentTemplateId, nodeInfo.ContentTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Keywords, nodeInfo.Keywords);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Description, nodeInfo.Description);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ExtendValues, nodeInfo.Additional.ToString());

            if (nodeInfo.ChannelTemplateId != 0)
            {
                var channelTemplateName = TemplateManager.GetTemplateName(nodeInfo.SiteId, nodeInfo.ChannelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ChannelTemplateName, channelTemplateName);
            }

            if (nodeInfo.ContentTemplateId != 0)
            {
                var contentTemplateName = TemplateManager.GetTemplateName(nodeInfo.SiteId, nodeInfo.ContentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ContentTemplateName, contentTemplateName);
            }

            return feed;
        }

        private AtomEntry ExportContentInfo(ContentInfo contentInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Id, contentInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ ContentAttribute.ChannelId, "NodeId" }, contentInfo.ChannelId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.SiteId, "PublishmentSystemId" }, contentInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddUserName, contentInfo.AddUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditUserName, contentInfo.LastEditUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.WritingUserName, contentInfo.WritingUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditDate, contentInfo.LastEditDate.ToString(CultureInfo.InvariantCulture));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Taxis, contentInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ ContentAttribute.GroupNameCollection, "ContentGroupNameCollection" }, contentInfo.GroupNameCollection);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Tags, AtomUtility.Encrypt(contentInfo.Tags));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.SourceId, contentInfo.SourceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.ReferenceId, contentInfo.ReferenceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsChecked, contentInfo.IsChecked.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.CheckedLevel, contentInfo.CheckedLevel.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Hits, contentInfo.Hits.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByDay, contentInfo.HitsByDay.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByWeek, contentInfo.HitsByWeek.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByMonth, contentInfo.HitsByMonth.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastHitsDate, contentInfo.LastHitsDate.ToString(CultureInfo.InvariantCulture));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Title, AtomUtility.Encrypt(contentInfo.Title));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsTop, contentInfo.IsTop.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsRecommend, contentInfo.IsRecommend.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsHot, contentInfo.IsHot.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsColor, contentInfo.IsColor.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LinkUrl, contentInfo.LinkUrl);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddDate, contentInfo.AddDate.ToString(CultureInfo.InvariantCulture));

            foreach (string attributeName in contentInfo.ToNameValueCollection().Keys)
            {
                if (!ContentAttribute.AllAttributesLowercase.Contains(attributeName.ToLower()))
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, AtomUtility.Encrypt(contentInfo.GetString(attributeName)));
                }
            }

            return entry;
        }

    }
}
