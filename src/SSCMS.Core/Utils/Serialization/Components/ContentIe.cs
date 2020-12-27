using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core.Collections;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
    internal class ContentIe
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
        private readonly Site _site;
        private readonly string _siteContentDirectoryPath;

        public ContentIe(IPathManager pathManager, IDatabaseManager databaseManager, CacheUtils caching, Site site, string siteContentDirectoryPath)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _caching = caching;
            _siteContentDirectoryPath = siteContentDirectoryPath;
            _site = site;
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

                    var groupNames = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string>{ nameof(Content.GroupNames), "GroupNameCollection", "ContentGroupNameCollection" });
                    var tagNames = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.TagNames), "Tags" }));
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                            new List<string> { nameof(Content.Checked), "IsChecked"}));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.CheckedLevel)));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Hits)));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.HitsByDay)));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.HitsByWeek)));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.HitsByMonth)));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.LastHitsDate));
                    var downloads = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Downloads)));
                    var title = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Title));
                    var isTop = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Top), "IsTop"}));
                    var isRecommend = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Recommend), "IsRecommend"}));
                    var isHot = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Hot), "IsHot"}));
                    var isColor = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> {nameof(Content.Color), "IsColor"}));
                    var linkUrl = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.LinkUrl)));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.AddDate));
                    var body = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements,
                        new List<string> { nameof(Content.Body), "Content" }));

                    var topTaxis = 0;
                    if (isTop)
                    {
                        topTaxis = taxis - 1;
                        taxis = await _databaseManager.ContentRepository.GetMaxTaxisAsync(_site, channel, true) + 1;
                    }

                    var dict = new Dictionary<string, object>();
                    var attributes = AtomUtility.GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string attributeName in attributes.Keys)
                    {
                        dict[attributeName] = AtomUtility.Decrypt(attributes[attributeName]);
                    }
                    var content = new Content();
                    content.LoadDict(dict);

                    content.SiteId = _site.Id;
                    content.ChannelId = channel.Id;
                    content.AddDate = TranslateUtils.ToDateTime(addDate);
                    content.AdminId = adminId;
                    content.LastEditAdminId = adminId;
                    content.GroupNames = ListUtils.GetStringList(groupNames);
                    content.TagNames = ListUtils.GetStringList(tagNames);
                    content.Checked = isChecked;
                    content.CheckedLevel = checkedLevel;
                    content.Hits = hits;
                    content.HitsByDay = hitsByDay;
                    content.HitsByWeek = hitsByWeek;
                    content.HitsByMonth = hitsByMonth;
                    content.LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate);
                    content.Downloads = downloads;
                    content.Title = AtomUtility.Decrypt(title);
                    content.Top = isTop;
                    content.Recommend = isRecommend;
                    content.Hot = isHot;
                    content.Color = isColor;
                    content.LinkUrl = linkUrl;
                    content.Body = body;

                    var isInsert = false;
                    if (isOverride)
                    {
                        var existsIDs = await _databaseManager.ContentRepository.GetContentIdsBySameTitleAsync(_site, channel, content.Title);
                        if (existsIDs.Count > 0)
                        {
                            foreach (var id in existsIDs)
                            {
                                content.Id = id;
                                contents.Add(content);
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
                        content.Taxis = taxis;
                        contents.Add(content);

                        if (!string.IsNullOrEmpty(tagNames))
                        {
                            foreach (var tagName in ListUtils.GetStringList(tagNames))
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
                catch(Exception ex)
                {
                    await _databaseManager.ErrorLogRepository.AddErrorLogAsync(ex, "导入内容");
                }
            }

            foreach (var content in contents)
            {
                _caching.SetProcess(guid, $"导入内容: {content.Title}");
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

                    var groupNameCollection = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.GroupNames), "GroupNameCollection", "ContentGroupNameCollection" });
                    if (isCheckedBySettings)
                    {
                        isChecked = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string>{nameof(Content.Checked), "IsChecked" }));
                        checkedLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.CheckedLevel)));
                    }
                    var hits = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Hits)));
                    var hitsByDay = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.HitsByDay)));
                    var hitsByWeek = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.HitsByWeek)));
                    var hitsByMonth = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.HitsByMonth)));
                    var lastHitsDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.LastHitsDate));
                    var downloads = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Downloads)));
                    var title = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Title));

                    var subTitle = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.SubTitle));
                    var imageUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.ImageUrl));
                    var videoUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.VideoUrl));
                    var fileUrl = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.FileUrl));
                    var body = AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Body), "Content" });
                    var summary = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Summary));
                    var author = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Author));
                    var source = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.Source));

                    var isTop = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Top), "IsTop" }));
                    var isRecommend = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Recommend), "IsRecommend" }));
                    var isHot = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Hot), "IsHot" }));
                    var isColor = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, new List<string> { nameof(Content.Color), "IsColor" }));
                    var linkUrl = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.LinkUrl)));
                    var addDate = AtomUtility.GetDcElementContent(entry.AdditionalElements, nameof(Content.AddDate));

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
                        GroupNames = ListUtils.GetStringList(groupNameCollection),
                        TagNames = ListUtils.GetStringList(tags),
                        Checked = isChecked,
                        CheckedLevel = checkedLevel,
                        Hits = hits,
                        HitsByDay = hitsByDay,
                        HitsByWeek = hitsByWeek,
                        HitsByMonth = hitsByMonth,
                        LastHitsDate = TranslateUtils.ToDateTime(lastHitsDate),
                        Downloads = downloads,
                        Title = AtomUtility.Decrypt(title),
                        SubTitle = AtomUtility.Decrypt(subTitle),
                        ImageUrl = AtomUtility.Decrypt(imageUrl),
                        VideoUrl = AtomUtility.Decrypt(videoUrl),
                        FileUrl = AtomUtility.Decrypt(fileUrl),
                        Body = AtomUtility.Decrypt(body),
                        Summary = AtomUtility.Decrypt(summary),
                        Author = AtomUtility.Decrypt(author),
                        Source = AtomUtility.Decrypt(source),
                        Top = isTop,
                        Recommend = isRecommend,
                        Hot = isHot,
                        Color = isColor,
                        LinkUrl = linkUrl
                    };

                    var attributes = AtomUtility.GetDcElementNameValueCollection(entry.AdditionalElements);
                    foreach (string attributeName in attributes.Keys)
                    {
                        if (!contentInfo.ContainsKey(StringUtils.ToLower(attributeName)))
                        {
                            contentInfo.Set(attributeName, AtomUtility.Decrypt(attributes[attributeName]));
                        }
                    }

                    var isInsert = false;
                    if (isOverride)
                    {
                        var existsIDs = await _databaseManager.ContentRepository.GetContentIdsBySameTitleAsync(_site, channel, contentInfo.Title);
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
                            foreach (var tagName in ListUtils.GetStringList(tags))
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

        public async Task<bool> ExportContentsAsync(Site site, List<Content> contents)
        {
            var filePath = _siteContentDirectoryPath + PathUtils.SeparatorChar + "contents.xml";
            var feed = AtomUtility.GetEmptyFeed();

            var collection = new NameValueCollection();

            foreach (var content in contents)
            {
                try
                {
                    await _pathManager.PutImagePathsAsync(site, content, collection);
                }
                catch
                {
                    // ignored
                }
                var entry = ExportContent(content);
                feed.Entries.Add(entry);
            }

            feed.Save(filePath);

            foreach (string imageUrl in collection.Keys)
            {
                var sourceFilePath = collection[imageUrl];
                var destFilePath = _pathManager.ParsePath(_siteContentDirectoryPath, imageUrl);
                DirectoryUtils.CreateDirectoryIfNotExists(destFilePath);
                FileUtils.MoveFile(sourceFilePath, destFilePath, true);
            }

            return true;
        }

        public AtomEntry ExportContent(Content content)
        {
            var entry = AtomUtility.GetEmptyEntry();

            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Id), content.Id.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(Content.ChannelId), "NodeId" }, content.ChannelId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.SiteId), "PublishmentSystemId" }, content.SiteId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Taxis), content.Taxis.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string>{ nameof(Content.GroupNames), "GroupNameCollection", "ContentGroupNameCollection" }, ListUtils.ToString(content.GroupNames));
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.TagNames), "Tags" }, AtomUtility.Encrypt(ListUtils.ToString(content.TagNames)));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.SourceId), content.SourceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.ReferenceId), content.ReferenceId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.TemplateId), content.TemplateId.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Checked), "IsChecked" }, content.Checked.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.CheckedLevel), content.CheckedLevel.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Hits), content.Hits.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.HitsByDay), content.HitsByDay.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.HitsByWeek), content.HitsByWeek.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.HitsByMonth), content.HitsByMonth.ToString());
            if (content.LastHitsDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.LastHitsDate), content.LastHitsDate.Value.ToString(CultureInfo.InvariantCulture));
            }

            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Downloads), content.Downloads.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Title), AtomUtility.Encrypt(content.Title));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.SubTitle), AtomUtility.Encrypt(content.SubTitle));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.ImageUrl), AtomUtility.Encrypt(content.ImageUrl));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.VideoUrl), AtomUtility.Encrypt(content.VideoUrl));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.FileUrl), AtomUtility.Encrypt(content.FileUrl));
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Body), "Content" }, AtomUtility.Encrypt(content.Body));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Summary), AtomUtility.Encrypt(content.Summary));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Author), AtomUtility.Encrypt(content.Author));
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.Source), AtomUtility.Encrypt(content.Source));

            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Top), "IsTop" }, content.Top.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Recommend), "IsRecommend" }, content.Recommend.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Hot), "IsHot" }, content.Hot.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, new List<string> { nameof(Content.Color), "IsColor" }, content.Color.ToString());
            AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.LinkUrl), AtomUtility.Encrypt(content.LinkUrl));
            if (content.AddDate.HasValue)
            {
                AtomUtility.AddDcElement(entry.AdditionalElements, nameof(Content.AddDate), content.AddDate.Value.ToString(CultureInfo.InvariantCulture));
            }

            foreach (var attributeName in content.ToDictionary().Keys)
            {
                if (!ListUtils.ContainsIgnoreCase(ColumnsManager.MetadataAttributes.Value, attributeName))
                {
                    AtomUtility.AddDcElement(entry.AdditionalElements, attributeName, AtomUtility.Encrypt(content.Get<string>(attributeName)));
                }
            }

            return entry;
        }
    }
}
