using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class ContentTagRepository : IContentTagRepository
    {
        private readonly Repository<ContentTag> _repository;

        public ContentTagRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ContentTag>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(ContentTag tag)
        {
            await _repository.InsertAsync(tag, Q
                .CachingRemove(GetCacheKey(tag.SiteId))
            );
        }

        public async Task InsertAsync(int siteId, string tagName)
        {
            var tagNames = await GetTagNamesAsync(siteId);
            if (!tagNames.Contains(tagName))
            {
                await _repository.InsertAsync(new ContentTag
                    {
                        SiteId = siteId,
                        TagName = tagName
                    }, Q.CachingRemove(GetCacheKey(siteId))
                );
            }
        }

        public async Task DeleteAsync(int siteId, string tagName)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .Where(nameof(ContentTag.TagName), tagName)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task DeleteAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task UpdateAsync(ContentTag tag)
        {
            await _repository.UpdateAsync(tag, Q
                .CachingRemove(GetCacheKey(tag.SiteId))
            );
        }

        public async Task UpdateTagsAsync(List<string> previousTags, List<string> nowTags, int siteId, int contentId)
        {
            if (ListUtils.Equals(previousTags, nowTags)) return;

            var tagsToRemove = new List<string>();
            var tagsToAdd = new List<string>();

            if (previousTags != null)
            {
                foreach (var tag in previousTags)
                {
                    if (!nowTags.Contains(tag))
                    {
                        tagsToRemove.Add(tag);
                    }
                }
            }

            if (nowTags != null)
            {
                foreach (var tag in nowTags)
                {
                    if (previousTags == null || !previousTags.Contains(tag))
                    {
                        tagsToAdd.Add(tag);
                    }
                }
            }

            var tags = await GetTagsAsync(siteId);

            foreach (var tagName in tagsToRemove)
            {
                var tag = ListUtils.FirstOrDefault(tags, x => x.TagName == tagName);
                if (tag == null) continue;

                ListUtils.Remove(tag.ContentIds, contentId);
                tag.UseNum = ListUtils.Count(tag.ContentIds);

                if (tag.UseNum == 0)
                {
                    await DeleteAsync(siteId, tagName);
                }
                else
                {
                    await UpdateAsync(tag);
                }
            }

            foreach (var tagName in tagsToAdd)
            {
                var tag = ListUtils.FirstOrDefault(tags, x => x.TagName == tagName);
                if (tag != null)
                {
                    if (!ListUtils.Contains(tag.ContentIds, contentId))
                    {
                        tag.ContentIds = ListUtils.Add(tag.ContentIds, contentId);
                        tag.UseNum = tag.ContentIds.Count;
                        await UpdateAsync(tag);
                    }
                }
                else
                {
                    tag = new ContentTag
                    {
                        SiteId = siteId,
                        TagName = tagName,
                        ContentIds = new List<int>(contentId),
                        UseNum = 1
                    };
                    await InsertAsync(tag);
                }
            }
        }

        private string GetCacheKey(int siteId)
        {
            return CacheUtils.GetListKey(TableName, siteId);
        }

        public async Task<List<ContentTag>> GetTagsAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(ContentTag.SiteId), siteId)
                .WhereNotNull(nameof(ContentTag.TagName))
                .WhereNot(nameof(ContentTag.TagName), string.Empty)
                .Distinct()
                .OrderByDesc(nameof(ContentTag.UseNum))
                .CachingGet(GetCacheKey(siteId))
            );
        }

        public async Task<List<string>> GetTagNamesAsync(int siteId)
        {
            var tags = await GetTagsAsync(siteId);
            return tags.Select(x => x.TagName).ToList();
        }

        public List<ContentTag> GetTagsByLevel(List<ContentTag> tagInfoList, int totalNum, int tagLevel)
        {
            var list = new List<ContentTag>();
            var sortedlist = new SortedList();
            foreach (var tagInfo in tagInfoList)
            {
                list.Add(tagInfo);

                var tagNames = (List<string>)sortedlist[tagInfo.UseNum];
                if (tagNames == null || tagNames.Count == 0)
                {
                    tagNames = new List<string>();
                }
                tagNames.Add(tagInfo.TagName);
                sortedlist[tagInfo.UseNum] = tagNames;
            }

            var count1 = 1;
            var count2 = 2;
            var count3 = 3;
            if (sortedlist.Keys.Count > 3)
            {
                count1 = (int)Math.Ceiling(0.3 * sortedlist.Keys.Count);
                if (count1 < 1)
                {
                    count1 = 1;
                }
                count2 = (int)Math.Ceiling(0.7 * sortedlist.Keys.Count);
                if (count2 == sortedlist.Keys.Count)
                {
                    count2--;
                }
                if (count2 <= count1)
                {
                    count2++;
                }
                count3 = count2 + 1;
            }

            var currentCount = 0;
            foreach (int count in sortedlist.Keys)
            {
                currentCount++;

                var level = 1;

                if (currentCount <= count1)
                {
                    level = 1;
                }
                else if (currentCount > count1 && currentCount <= count2)
                {
                    level = 2;
                }
                else if (currentCount > count2 && currentCount <= count3)
                {
                    level = 3;
                }
                else if (currentCount > count3)
                {
                    level = 4;
                }

                var tagNames = (List<string>)sortedlist[count];
                foreach (var tagInfo in list)
                {
                    if (tagNames.Contains(tagInfo.TagName))
                    {
                        tagInfo.Level = level;
                    }
                }
            }

            if (tagLevel > 1)
            {
                var levelList = new List<ContentTag>();
                foreach (var tagInfo in list)
                {
                    if (tagInfo.Level >= tagLevel)
                    {
                        levelList.Add(tagInfo);
                    }
                    if (totalNum > 0 && levelList.Count > totalNum)
                    {
                        break;
                    }
                }
                list = levelList;
            }

            return list;
        }
    }
}