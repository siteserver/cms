using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;
using SS.CMS.Utils.Atom.Atom.Core.Collections;

namespace SS.CMS.Core.Serialization.Components
{
    internal class ContentIe
    {
        private readonly SiteInfo _siteInfo;
        private readonly string _siteContentDirectoryPath;
        private readonly IPathManager _pathManager;
        private readonly IUrlManager _urlManager;
        private readonly IChannelRepository _channelRepository;
        private readonly ITagRepository _tagRepository;

        public ContentIe(SiteInfo siteInfo, string siteContentDirectoryPath)
        {
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _siteInfo = siteInfo;
        }

        public async Task ImportContentsAsync(string filePath, bool isOverride, ChannelInfo nodeInfo, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel, string adminName)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            await ImportContentsAsync(feed.Entries, nodeInfo, taxis, importStart, importCount, false, isChecked, checkedLevel, isOverride, adminName);
        }

        public async Task ImportContentsAsync(string filePath, bool isOverride, ChannelInfo nodeInfo, int taxis, bool isChecked, int checkedLevel, int userId, int sourceId)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            await ImportContentsAsync(feed.Entries, nodeInfo, taxis, false, isChecked, checkedLevel, isOverride, userId, sourceId);
        }

        public async Task ImportContentsAsync(AtomEntryCollection entries, ChannelInfo channelInfo, int taxis, bool isOverride, string adminName)
        {
            await ImportContentsAsync(entries, channelInfo, taxis, 0, 0, true, true, 0, isOverride, adminName);
        }

        // 内部消化掉错误
        private async Task ImportContentsAsync(AtomEntryCollection entries, ChannelInfo channelInfo, int taxis, int importStart, int importCount, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, string adminName)
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

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
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
                        taxis = channelInfo.ContentRepository.GetMaxTaxis(channelInfo.Id, true) + 1;
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
                    contentInfo.GroupNameCollection = groupNameCollection;
                    contentInfo.Tags = tags;
                    contentInfo.IsChecked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;
                    contentInfo.Hits = hits;
                    contentInfo.HitsByDay = hitsByDay;
                    contentInfo.HitsByWeek = hitsByWeek;
                    contentInfo.HitsByMonth = hitsByMonth;
                    contentInfo.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    contentInfo.Downloads = downloads;
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
                        var existsIDs = channelInfo.ContentRepository.GetIdListBySameTitle(contentInfo.ChannelId, contentInfo.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (int id in existsIDs)
                            {
                                contentInfo.Id = id;
                                await channelInfo.ContentRepository.UpdateAsync(_siteInfo, channelInfo, contentInfo);
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
                        var contentId = await channelInfo.ContentRepository.InsertWithTaxisAsync(_siteInfo, channelInfo, contentInfo, taxis);

                        await _tagRepository.UpdateTagsAsync(string.Empty, tags, _siteInfo.Id, contentId);
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

        private async Task ImportContentsAsync(AtomEntryCollection entries, ChannelInfo channelInfo, int taxis, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, int userId, int sourceId)
        {
            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
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
                        taxis = channelInfo.ContentRepository.GetMaxTaxis(channelInfo.Id, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Tags));

                    var dict = new Dictionary<string, object>
                    {
                        {ContentAttribute.SiteId, _siteInfo.Id},
                        {ContentAttribute.ChannelId, channelInfo.Id},
                        {ContentAttribute.UserId, userId},
                        {ContentAttribute.SourceId, sourceId},
                        {ContentAttribute.AddDate, TranslateUtils.ToDateTime(addDate)}
                    };
                    var contentInfo = new ContentInfo(dict);

                    contentInfo.LastEditUserName = contentInfo.AddUserName;
                    contentInfo.GroupNameCollection = groupNameCollection;
                    contentInfo.Tags = tags;
                    contentInfo.IsChecked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;
                    contentInfo.Hits = hits;
                    contentInfo.HitsByDay = hitsByDay;
                    contentInfo.HitsByWeek = hitsByWeek;
                    contentInfo.HitsByMonth = hitsByMonth;
                    contentInfo.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    contentInfo.Downloads = downloads;
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
                        var existsIDs = channelInfo.ContentRepository.GetIdListBySameTitle(contentInfo.ChannelId, contentInfo.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (int id in existsIDs)
                            {
                                contentInfo.Id = id;
                                await channelInfo.ContentRepository.UpdateAsync(_siteInfo, channelInfo, contentInfo);
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
                        var contentId = await channelInfo.ContentRepository.InsertWithTaxisAsync(_siteInfo, channelInfo, contentInfo, taxis);

                        await _tagRepository.UpdateTagsAsync(string.Empty, tags, _siteInfo.Id, contentId);
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

        public async Task<bool> ExportContentsAsync(SiteInfo siteInfo, int channelId, IList<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, bool? checkedState)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
            var feed = AtomUtility.GetEmptyFeed();

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = channelInfo.ContentRepository.GetContentIdList(channelId, isPeriods, dateFrom, dateTo, checkedState);
            }
            if (contentIdList.Count == 0) return false;

            var collection = new NameValueCollection();

            foreach (var contentId in contentIdList)
            {
                var contentInfo = channelInfo.ContentRepository.GetContentInfo(contentId);
                try
                {
                    _urlManager.PutImagePaths(siteInfo, contentInfo, collection);
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
                var destFilePath = _pathManager.MapPath(_siteContentDirectoryPath, imageUrl);
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
                    _urlManager.PutImagePaths(siteInfo, contentInfo, collection);
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
                var destFilePath = _pathManager.MapPath(_siteContentDirectoryPath, imageUrl);
                DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);
                FileUtils.MoveFile(sourceFilePath, destFilePath, true);
            }

            return true;
        }

        public AtomEntry ExportContentInfo(ContentInfo contentInfo)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Id, contentInfo.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.ChannelId, "NodeId" }, contentInfo.ChannelId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.SiteId, "PublishmentSystemId" }, contentInfo.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddUserName, contentInfo.AddUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditUserName, contentInfo.LastEditUserName);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Taxis, contentInfo.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.GroupNameCollection, "ContentGroupNameCollection" }, contentInfo.GroupNameCollection);
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Tags, AtomUtility.Encrypt(contentInfo.Tags));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.SourceId, contentInfo.SourceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.ReferenceId, contentInfo.ReferenceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsChecked, contentInfo.IsChecked.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.CheckedLevel, contentInfo.CheckedLevel.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Hits, contentInfo.Hits.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByDay, contentInfo.HitsByDay.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByWeek, contentInfo.HitsByWeek.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.HitsByMonth, contentInfo.HitsByMonth.ToString());
            if (contentInfo.LastHitsDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastHitsDate,
                    contentInfo.LastHitsDate.Value.ToString(CultureInfo.InvariantCulture));
            }

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Downloads, contentInfo.Downloads.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Title, AtomUtility.Encrypt(contentInfo.Title));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsTop, contentInfo.IsTop.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsRecommend, contentInfo.IsRecommend.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsHot, contentInfo.IsHot.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.IsColor, contentInfo.IsColor.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LinkUrl, AtomUtility.Encrypt(contentInfo.LinkUrl));
            if (contentInfo.AddDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.AddDate,
                    contentInfo.AddDate.Value.ToString(CultureInfo.InvariantCulture));
            }

            foreach (var attributeName in contentInfo.ToDictionary().Keys)
            {
                if (!StringUtils.ContainsIgnoreCase(ContentAttribute.AllAttributes.Value, attributeName))
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, AtomUtility.Encrypt(contentInfo.Get<string>(attributeName)));
                }
            }

            return entry;
        }
    }
}
