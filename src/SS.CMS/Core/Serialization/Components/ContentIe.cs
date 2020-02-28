using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using SS.CMS.Abstractions;
using SS.CMS.Core.Serialization.Atom.Atom.Core;
using SS.CMS.Core.Serialization.Atom.Atom.Core.Collections;

namespace SS.CMS.Core.Serialization.Components
{
    internal class ContentIe
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly Site _site;
        private readonly string _siteContentDirectoryPath;

        public ContentIe(IPathManager pathManager, IDatabaseManager databaseManager, Site site, string siteContentDirectoryPath)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _site = site;
        }

        public async Task ImportContentsAsync(string filePath, bool isOverride, Channel channel, int taxis, int importStart, int importCount, bool isChecked, int checkedLevel, int adminId, string guid)
        {
            if (!FileUtils.IsFileExists(filePath)) return;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            await ImportContentsAsync(feed.Entries, channel, taxis, importStart, importCount, false, isChecked, checkedLevel, isOverride, adminId, guid);
        }

        public async Task<List<int>> ImportContentsAsync(string filePath, bool isOverride, Channel channel, int taxis, bool isChecked, int checkedLevel, int adminId, int userId, int sourceId)
        {
            if (!FileUtils.IsFileExists(filePath)) return null;
            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            return await ImportContentsAsync(feed.Entries, channel, taxis, false, isChecked, checkedLevel, isOverride, adminId, userId, sourceId);
        }

        public async Task ImportContentsAsync(AtomEntryCollection entries, Channel channel, int taxis, bool isOverride, int adminId, string guid)
        {
            await ImportContentsAsync(entries, channel, taxis, 0, 0, true, true, 0, isOverride, adminId, guid);
        }

        // 内部消化掉错误
        private async Task ImportContentsAsync(AtomEntryCollection entries, Channel channel, int taxis, int importStart, int importCount, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, int adminId, string guid)
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

            var contents = new List<Content>();

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string>{ nameof(Content.GroupNames), "GroupNameCollection", "ContentGroupNameCollection" });
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                            new List<string> {nameof(ContentAttribute.Checked), "IsChecked"}));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.CheckedLevel));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Hits));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByDay));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByWeek));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByMonth));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastHitsDate);
                    var downloads = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Downloads));
                    var title = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Title);
                    var isTop = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Top), "IsTop"}));
                    var isRecommend = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Recommend), "IsRecommend"}));
                    var isHot = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Hot), "IsHot"}));
                    var isColor = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Color), "IsColor"}));
                    var linkUrl = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LinkUrl));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.AddDate);

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = await _databaseManager.ContentRepository.GetMaxTaxisAsync(_site, channel, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.TagNames), "Tags" }));

                    var contentInfo = new Content
                    {
                        SiteId = _site.Id,
                        ChannelId = channel.Id,
                        AddDate = TranslateUtils.ToDateTime(addDate),
                        LastEditDate = TranslateUtils.ToDateTime(lastEditDate),
                        AdminId = adminId,
                        LastEditAdminId = adminId,
                        GroupNames = Utilities.GetStringList(groupNameCollection),
                        TagNames = Utilities.GetStringList(tags),
                        Checked = isChecked,
                        CheckedLevel = checkedLevel,
                        Hits = hits,
                        HitsByDay = hitsByDay,
                        HitsByWeek = hitsByWeek,
                        HitsByMonth = hitsByMonth,
                        LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate),
                        Downloads = downloads,
                        Title = AtomUtility.Decrypt(title),
                        Top = isTop,
                        Recommend = isRecommend,
                        Hot = isHot,
                        Color = isColor,
                        LinkUrl = linkUrl
                    };

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
                        var existsIDs = await _databaseManager.ContentRepository.GetIdListBySameTitleAsync(_site, channel, contentInfo.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (var id in existsIDs)
                            {
                                contentInfo.Id = id;
                                contents.Add(contentInfo);
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
                        contentInfo.Taxis = taxis;
                        contents.Add(contentInfo);

                        if (!string.IsNullOrEmpty(tags))
                        {
                            foreach (var tagName in Utilities.GetStringList(tags))
                            {
                                await _databaseManager.ContentTagRepository.InsertAsync(_site.Id, tagName);
                            }
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

            foreach (var content in contents)
            {
                Caching.SetProcess(guid, $"导入内容: {content.Title}");
                if (content.Id > 0)
                {
                    await _databaseManager.ContentRepository.UpdateAsync(_site, channel, content);
                }
                else
                {
                    await _databaseManager.ContentRepository.InsertWithTaxisAsync(_site, channel, content, content.Taxis);
                }
            }
        }

        private async Task<List<int>> ImportContentsAsync(AtomEntryCollection entries, Channel channel, int taxis, bool isCheckedBySettings, bool isChecked, int checkedLevel, bool isOverride, int adminId, int userId, int sourceId)
        {
            var contents = new List<Content>();

            foreach (AtomEntry entry in entries)
            {
                try
                {
                    taxis++;
                    var lastEditDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastEditDate);
                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.GroupNames), "GroupNameCollection", "ContentGroupNameCollection" });
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string>{nameof(Content.Checked), "IsChecked" }));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.CheckedLevel));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Hits));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByDay));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByWeek));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.HitsByMonth));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LastHitsDate);
                    var downloads = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Downloads));
                    var title = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.Title);
                    var isTop = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Top), "IsTop" }));
                    var isRecommend = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Recommend), "IsRecommend" }));
                    var isHot = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Hot), "IsHot" }));
                    var isColor = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Color), "IsColor" }));
                    var linkUrl = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.LinkUrl));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, ContentAttribute.AddDate);

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = await _databaseManager.ContentRepository.GetMaxTaxisAsync(_site, channel, true) + 1;
                    }
                    var tags = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.TagNames), "Tags" }));

                    var contentInfo = new Content
                    {
                        SiteId = _site.Id,
                        ChannelId = channel.Id,
                        AdminId = adminId,
                        LastEditAdminId = adminId,
                        UserId = userId,
                        SourceId = sourceId,
                        AddDate = TranslateUtils.ToDateTime(addDate),
                        LastEditDate = TranslateUtils.ToDateTime(lastEditDate),
                        GroupNames = Utilities.GetStringList(groupNameCollection),
                        TagNames = Utilities.GetStringList(tags),
                        Checked = isChecked,
                        CheckedLevel = checkedLevel,
                        Hits = hits,
                        HitsByDay = hitsByDay,
                        HitsByWeek = hitsByWeek,
                        HitsByMonth = hitsByMonth,
                        LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate),
                        Downloads = downloads,
                        Title = AtomUtility.Decrypt(title),
                        Top = isTop,
                        Recommend = isRecommend,
                        Hot = isHot,
                        Color = isColor,
                        LinkUrl = linkUrl
                    };

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
                        var existsIDs = await _databaseManager.ContentRepository.GetIdListBySameTitleAsync(_site, channel, contentInfo.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (var id in existsIDs)
                            {
                                contentInfo.Id = id;
                                contents.Add(contentInfo);
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
                        contentInfo.Taxis = taxis;
                        contents.Add(contentInfo);

                        if (!string.IsNullOrEmpty(tags))
                        {
                            foreach (var tagName in Utilities.GetStringList(tags))
                            {
                                await _databaseManager.ContentTagRepository.InsertAsync(_site.Id, tagName);
                            }
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

            var contentIdList = new List<int>();
            foreach (var content in contents)
            {
                if (content.Id > 0)
                {
                    await _databaseManager.ContentRepository.UpdateAsync(_site, channel, content);
                }
                else
                {
                    contentIdList.Add(await _databaseManager.ContentRepository.InsertWithTaxisAsync(_site, channel, content, content.Taxis));
                }
                
            }

            return contentIdList;
        }

        public async Task<bool> ExportContentsAsync(Site site, int channelId, List<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, bool? checkedState)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var channelInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
            var feed = AtomUtility.GetEmptyFeed();

            if (contentIdList == null)
            {
                contentIdList = await _databaseManager.ContentRepository.GetContentIdsAsync(site, channelInfo, isPeriods, dateFrom, dateTo, checkedState);
            }
            if (!contentIdList.Any()) return false;

            var collection = new NameValueCollection();

            foreach (var contentId in contentIdList)
            {
                var contentInfo = await _databaseManager.ContentRepository.GetAsync(site, channelInfo, contentId);
                try
                {
                    ContentUtility.PutImagePaths(_pathManager, site, contentInfo, collection);
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

        public bool ExportContents(Site site, List<Content> contentInfoList)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var feed = AtomUtility.GetEmptyFeed();

            var collection = new NameValueCollection();

            foreach (var contentInfo in contentInfoList)
            {
                try
                {
                    ContentUtility.PutImagePaths(_pathManager, site, contentInfo, collection);
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

        public AtomEntry ExportContentInfo(Content content)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Id, content.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ ContentAttribute.ChannelId, "NodeId" }, content.ChannelId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { ContentAttribute.SiteId, "PublishmentSystemId" }, content.SiteId.ToString());
            if (content.LastEditDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.LastEditDate, content.LastEditDate.Value.ToString(CultureInfo.InvariantCulture));
            }
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.Taxis, content.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(Content.GroupNames), "GroupNameCollection", "ContentGroupNameCollection" }, Utilities.ToString(content.GroupNames));
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.TagNames), "Tags" }, AtomUtility.Encrypt(Utilities.ToString(content.TagNames)));
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.SourceId, content.SourceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, ContentAttribute.ReferenceId, content.ReferenceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(ContentAttribute.Checked), "IsChecked" }, content.Checked.ToString());
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
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Top), "IsTop" }, content.Top.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Recommend), "IsRecommend" }, content.Recommend.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Hot), "IsHot" }, content.Hot.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Color), "IsColor" }, content.Color.ToString());
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
