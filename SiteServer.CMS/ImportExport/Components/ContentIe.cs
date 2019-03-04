using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using Atom.Core;
using Atom.Core.Collections;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class ContentIe
    {
        private readonly SiteInfo _siteInfo;
        private readonly string _siteContentDirectoryPath;

        public ContentIe(SiteInfo siteInfo, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _siteInfo = siteInfo;
        }

        public void ImportContents(string filePath, bool isOverride, ChannelInfo nodeInfo, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel, string adminName)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            ImportContents(feed.Entries, nodeInfo, taxis, importStart, importCount, false, isChecked, checkedLevel, isOverride, adminName);
        }

        public void ImportContents(string filePath, bool isOverride, ChannelInfo nodeInfo, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            ImportContents(feed.Entries, nodeInfo, taxis, false, isChecked, checkedLevel, isOverride, adminId, userId, sourceId);
        }

        public void ImportContents(AtomEntryCollection entries, ChannelInfo channelInfo, int taxis, bool isOverride, string adminName)
        {
            ImportContents(entries, channelInfo, taxis, 0, 0, true, true, 0, isOverride, adminName);
        }

        // 内部消化掉错误
        private void ImportContents(AtomEntryCollection entries, ChannelInfo channelInfo, int taxis, int importStart, int importCount, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, string adminName)
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

            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

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
                    var linkUrl = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LinkUrl));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.AddDate);

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, channelInfo.Id, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var dict = new Dictionary<string, object>
                    {
                        {ContentAttribute.SiteId, _siteInfo.Id},
                        {ContentAttribute.ChannelId, channelInfo.Id},
                        {ContentAttribute.AddUserName, adminName},
                        {ContentAttribute.AddDate, TranslateUtils.ToDateTime(addDate)}
                    };
                    var contentInfo = new ContentInfo(dict);

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
                                DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
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
                        var contentId = DataProvider.ContentDao.InsertWithTaxis(tableName, _siteInfo, channelInfo, contentInfo, taxis);

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

        private void ImportContents(AtomEntryCollection entries, ChannelInfo channelInfo, int taxis, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, int adminId, int userId, int sourceId)
        {
            var tableName = ChannelManager.GetTableName(_siteInfo, channelInfo);

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { ContentAttribute.GroupNameCollection, "ContentGroupNameCollection" });
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
                    var linkUrl = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LinkUrl));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.AddDate);

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, channelInfo.Id, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var dict = new Dictionary<string, object>
                    {
                        {ContentAttribute.SiteId, _siteInfo.Id},
                        {ContentAttribute.ChannelId, channelInfo.Id},
                        {ContentAttribute.AdminId, adminId},
                        {ContentAttribute.UserId, userId},
                        {ContentAttribute.SourceId, sourceId},
                        {ContentAttribute.AddDate, TranslateUtils.ToDateTime(addDate)}
                    };
                    var contentInfo = new ContentInfo(dict);

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
                                DataProvider.ContentDao.Update(_siteInfo, channelInfo, contentInfo);
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
                        var contentId = DataProvider.ContentDao.InsertWithTaxis(tableName, _siteInfo, channelInfo, contentInfo, taxis);

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

        public bool ExportContents(SiteInfo siteInfo, int channelId, List<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            var feed = AtomUtility.GetEmptyFeed();

            if (contentIdList == null || contentIdList.Count == 0)
            {
                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId, isPeriods, dateFrom, dateTo, checkedState);
            }
            if (contentIdList.Count == 0) return false;

            var collection = new NameValueCollection();

            foreach (var contentId in contentIdList)
            {
                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
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

        public bool ExportContents(SiteInfo siteInfo, List<ContentInfo> contentInfoList)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var feed = AtomUtility.GetEmptyFeed();

            var collection = new NameValueCollection();

            foreach (var contentInfo in contentInfoList)
            {
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

        public AtomEntry ExportContentInfo(ContentInfo contentInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Id, contentInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ ContentAttribute.ChannelId, "NodeId" }, contentInfo.ChannelId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.SiteId, "PublishmentSystemId" }, contentInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddUserName, contentInfo.AddUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditUserName, contentInfo.LastEditUserName);
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
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LinkUrl, AtomUtility.Encrypt(contentInfo.LinkUrl));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddDate, contentInfo.AddDate.ToString(CultureInfo.InvariantCulture));

            foreach (var attributeName in contentInfo.ToDictionary().Keys)
            {
                if (!StringUtils.ContainsIgnoreCase(ContentAttribute.AllAttributes.Value, attributeName))
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, AtomUtility.Encrypt(contentInfo.GetString(attributeName)));
                }
            }

            return entry;
        }
    }
}
