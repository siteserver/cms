using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.CMS.Context.Atom.Atom.Core.Collections;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class ContentIe
    {
        private readonly Site _site;
        private readonly string _siteContentDirectoryPath;

        public ContentIe(Site site, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _site = site;
        }

        public async Task ImportContentsAsync(string filePath, bool isOverride, Channel channel, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel, string adminName)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            await ImportContentsAsync(feed.Entries, channel, taxis, importStart, importCount, false, isChecked, checkedLevel, isOverride, adminName);
        }

        public async Task<List<int>> ImportContentsAsync(string filePath, bool isOverride, Channel channel, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            if (!FileUtils.IsFileExists(filePath)) return null;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            return await ImportContentsAsync(feed.Entries, channel, taxis, false, isChecked, checkedLevel, isOverride, adminId, userId, sourceId);
        }

        public async Task<List<int>> ImportContentsAsync(AtomEntryCollection entries, Channel channel, int taxis, bool isOverride, string adminName)
        {
            return await ImportContentsAsync(entries, channel, taxis, 0, 0, true, true, 0, isOverride, adminName);
        }

        // 内部消化掉错误
        private async Task<List<int>> ImportContentsAsync(AtomEntryCollection entries, Channel channel, int taxis, int importStart, int importCount, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, string adminName)
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

            var tableName = await ChannelManager.GetTableNameAsync(_site, channel);
            var contentIdList = new List<int>();

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string>{ ContentAttribute.GroupNameCollection , "ContentGroupNameCollection" });
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Checked));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.CheckedLevel));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Hits));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByDay));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByWeek));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByMonth));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastHitsDate);
                    var downloads = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Downloads));
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
                        taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, channel.Id, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var dict = new Dictionary<string, object>
                    {
                        {ContentAttribute.SiteId, _site.Id},
                        {ContentAttribute.ChannelId, channel.Id},
                        {ContentAttribute.AddUserName, adminName},
                        {ContentAttribute.AddDate, TranslateUtils.ToDateTime(addDate)}
                    };
                    var contentInfo = new Content(dict);

                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = TranslateUtils.ToDateTime(lastEditDate);
                    contentInfo.GroupNameCollection = groupNameCollection;
                    contentInfo.Tags = tags;
                    contentInfo.Checked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;
                    contentInfo.Hits = hits;
                    contentInfo.HitsByDay = hitsByDay;
                    contentInfo.HitsByWeek = hitsByWeek;
                    contentInfo.HitsByMonth = hitsByMonth;
                    contentInfo.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    contentInfo.Downloads = downloads;
                    contentInfo.Title = AtomUtility.Decrypt(title);
                    contentInfo.Top = isTop;
                    contentInfo.Recommend = isRecommend;
                    contentInfo.Hot = isHot;
                    contentInfo.Color = isColor;
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
                            foreach (var id in existsIDs)
                            {
                                contentInfo.Id = id;
                                await DataProvider.ContentDao.UpdateAsync(_site, channel, contentInfo);
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
                        var contentId = await DataProvider.ContentDao.InsertWithTaxisAsync(tableName, _site, channel, contentInfo, taxis);
                        contentIdList.Add(contentId);

                        await ContentTagUtils.UpdateTagsAsync(string.Empty, tags, _site.Id, contentId);
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

            return contentIdList;
        }

        private async Task<List<int>> ImportContentsAsync(AtomEntryCollection entries, Channel channel, int taxis, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, int adminId, int userId, int sourceId)
        {
            var tableName = await ChannelManager.GetTableNameAsync(_site, channel);
            var contentIdList = new List<int>();

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { ContentAttribute.GroupNameCollection, "ContentGroupNameCollection" });
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Checked));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.CheckedLevel));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Hits));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByDay));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByWeek));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByMonth));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastHitsDate);
                    var downloads = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Downloads));
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
                        taxis = DataProvider.ContentDao.GetMaxTaxis(tableName, channel.Id, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var dict = new Dictionary<string, object>
                    {
                        {ContentAttribute.SiteId, _site.Id},
                        {ContentAttribute.ChannelId, channel.Id},
                        {ContentAttribute.AdminId, adminId},
                        {ContentAttribute.UserId, userId},
                        {ContentAttribute.SourceId, sourceId},
                        {ContentAttribute.AddDate, TranslateUtils.ToDateTime(addDate)}
                    };
                    var contentInfo = new Content(dict);

                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.LastEditDate = TranslateUtils.ToDateTime(lastEditDate);
                    contentInfo.GroupNameCollection = groupNameCollection;
                    contentInfo.Tags = tags;
                    contentInfo.Checked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;
                    contentInfo.Hits = hits;
                    contentInfo.HitsByDay = hitsByDay;
                    contentInfo.HitsByWeek = hitsByWeek;
                    contentInfo.HitsByMonth = hitsByMonth;
                    contentInfo.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    contentInfo.Downloads = downloads;
                    contentInfo.Title = AtomUtility.Decrypt(title);
                    contentInfo.Top = isTop;
                    contentInfo.Recommend = isRecommend;
                    contentInfo.Hot = isHot;
                    contentInfo.Color = isColor;
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
                                await DataProvider.ContentDao.UpdateAsync(_site, channel, contentInfo);
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
                        var contentId = await DataProvider.ContentDao.InsertWithTaxisAsync(tableName, _site, channel, contentInfo, taxis);

                        contentIdList.Add(contentId);

                        await ContentTagUtils.UpdateTagsAsync(string.Empty, tags, _site.Id, contentId);
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

            return contentIdList;
        }

        public async Task<bool> ExportContentsAsync(Site site, int channelId, List<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var channelInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
            var feed = AtomUtility.GetEmptyFeed();

            if (contentIdList == null || contentIdList.Count == 0)
            {
                var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);
                contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId, isPeriods, dateFrom, dateTo, checkedState);
            }
            if (contentIdList.Count == 0) return false;

            var collection = new NameValueCollection();

            foreach (var contentId in contentIdList)
            {
                var contentInfo = await ContentManager.GetContentInfoAsync(site, channelInfo, contentId);
                try
                {
                    ContentUtility.PutImagePaths(site, contentInfo, collection);
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

        public bool ExportContents(Site site, List<Content> contentInfoList)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var feed = AtomUtility.GetEmptyFeed();

            var collection = new NameValueCollection();

            foreach (var contentInfo in contentInfoList)
            {
                try
                {
                    ContentUtility.PutImagePaths(site, contentInfo, collection);
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

        public AtomEntry ExportContentInfo(Content content)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Id, content.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ ContentAttribute.ChannelId, "NodeId" }, content.ChannelId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.SiteId, "PublishmentSystemId" }, content.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddUserName, content.AddUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditUserName, content.LastEditUserName);
            if (content.LastEditDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditDate, content.LastEditDate.Value.ToString(CultureInfo.InvariantCulture));
            }
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Taxis, content.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ ContentAttribute.GroupNameCollection, "ContentGroupNameCollection" }, content.GroupNameCollection);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Tags, AtomUtility.Encrypt(content.Tags));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.SourceId, content.SourceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.ReferenceId, content.ReferenceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Checked, content.Checked.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.CheckedLevel, content.CheckedLevel.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Hits, content.Hits.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByDay, content.HitsByDay.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByWeek, content.HitsByWeek.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByMonth, content.HitsByMonth.ToString());
            if (content.LastHitsDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastHitsDate,
                    content.LastHitsDate.Value.ToString(CultureInfo.InvariantCulture));
            }

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Downloads, content.Downloads.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Title, AtomUtility.Encrypt(content.Title));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsTop, content.IsTop.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsRecommend, content.Recommend.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsHot, content.Hot.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsColor, content.Color.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LinkUrl, AtomUtility.Encrypt(content.LinkUrl));
            if (content.AddDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddDate,
                    content.AddDate.Value.ToString(CultureInfo.InvariantCulture));
            }

            foreach (var attributeName in content.ToDictionary().Keys)
            {
                if (!StringUtils.ContainsIgnoreCase(ContentAttribute.AllAttributes.Value, attributeName))
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, AtomUtility.Encrypt(content.Get<string>(attributeName)));
                }
            }

            return entry;
        }
    }
}
