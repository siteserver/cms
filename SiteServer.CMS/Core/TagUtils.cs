using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System;
using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class TagUtils
    {
        private static void AddTags(List<string> tags, int siteId, int contentId)
        {
            if (tags == null || tags.Count == 0) return;

            foreach (var tagName in tags)
            {
                var tagInfo = DataProvider.TagDao.GetTagInfo(siteId, tagName);
                if (tagInfo != null)
                {
                    var contentIdList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                    if (!contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                        tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(contentIdList);
                        tagInfo.UseNum = contentIdList.Count;
                        DataProvider.TagDao.Update(tagInfo);
                    }
                }
                else
                {
                    tagInfo = new TagInfo(0, siteId, contentId.ToString(), tagName, contentId > 0 ? 1 : 0);
                    DataProvider.TagDao.Insert(tagInfo);
                }
            }
        }

        private static void RemoveTags(List<string> tags, int siteId, int contentId)
        {
            if (tags == null || tags.Count == 0) return;

            foreach (var tagName in tags)
            {
                var tagInfo = DataProvider.TagDao.GetTagInfo(siteId, tagName);
                if (tagInfo == null) continue;

                var contentIdList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                contentIdList.Remove(contentId);
                tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(contentIdList);
                tagInfo.UseNum = contentIdList.Count;

                if (tagInfo.UseNum == 0)
                {
                    DataProvider.TagDao.DeleteTag(tagName, siteId);
                }
                else
                {
                    DataProvider.TagDao.Update(tagInfo);
                }
            }
        }

        public static void UpdateTags(string tagsPrevious, string tagsNow, int siteId, int contentId)
        {
            if (tagsPrevious == tagsNow) return;

            var previousTags = ParseTagsString(tagsPrevious);
            var nowTags = ParseTagsString(tagsNow);

            var tagsToRemove = new List<string>();
            var tagsToAdd = new List<string>();

            foreach (var tag in previousTags)
            {
                if (!nowTags.Contains(tag))
                {
                    tagsToRemove.Add(tag);
                }
            }
            foreach (var tag in nowTags)
            {
                if (!previousTags.Contains(tag))
                {
                    tagsToAdd.Add(tag);
                }
            }

            RemoveTags(tagsToRemove, siteId, contentId);
            AddTags(tagsToAdd, siteId, contentId);
        }

        public static void RemoveTags(int siteId, List<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                RemoveTags(siteId, contentId);
            }
        }

        public static void RemoveTags(int siteId, int contentId)
        {
            var tagInfoList = DataProvider.TagDao.GetTagInfoList(siteId, contentId);
            if (tagInfoList == null || tagInfoList.Count == 0) return;

            foreach (var tagInfo in tagInfoList)
            {
                var contentIdList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                contentIdList.Remove(contentId);
                tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(contentIdList);
                tagInfo.UseNum = contentIdList.Count;
                DataProvider.TagDao.Update(tagInfo);
            }
        }

        public static string GetTagsString(StringCollection tags)
        {
            if (tags == null || tags.Count == 0) return string.Empty;

            var tagsBuilder = new StringBuilder();
            foreach (var tag in tags)
            {
                tagsBuilder.Append(tag.Trim().IndexOf(",", StringComparison.Ordinal) != -1 ? $"\"{tag}\"" : tag);
                tagsBuilder.Append(" ");
            }
            --tagsBuilder.Length;
            return tagsBuilder.ToString();
        }

        public static List<string> ParseTagsString(string tagsString)
        {
            var stringCollection = new List<string>();

            if (string.IsNullOrEmpty(tagsString)) return stringCollection;

            var regex = new Regex("\"([^\"]*)\"", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var mc = regex.Matches(tagsString);
            for (var i = 0; i < mc.Count; i++)
            {
                if (!string.IsNullOrEmpty(mc[i].Value))
                {
                    var tag = mc[i].Value.Replace("\"", string.Empty);
                    if (!stringCollection.Contains(tag))
                    {
                        stringCollection.Add(tag);
                    }

                    var startIndex = tagsString.IndexOf(mc[i].Value, StringComparison.Ordinal);
                    if (startIndex != -1)
                    {
                        tagsString = tagsString.Substring(0, startIndex) + tagsString.Substring(startIndex + mc[i].Value.Length);
                    }
                }
            }

            regex = new Regex("([^,;\\s]+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            mc = regex.Matches(tagsString);
            for (var i = 0; i < mc.Count; i++)
            {
                if (!string.IsNullOrEmpty(mc[i].Value))
                {
                    var tag = mc[i].Value.Replace("\"", string.Empty);
                    if (!stringCollection.Contains(tag))
                    {
                        stringCollection.Add(tag);
                    }
                }
            }

            return stringCollection;
        }

        public static List<TagInfo> GetTagInfoList(List<TagInfo> tagInfoList)
        {
            return GetTagInfoList(tagInfoList, 0, 0);
        }

        public static List<TagInfo> GetTagInfoList(List<TagInfo> tagInfoList, int totalNum, int tagLevel)
        {
            var list = new List<TagInfo>();
            var sortedlist = new SortedList();
            foreach (var tagInfo in tagInfoList)
            {
                list.Add(tagInfo);

                var tagNames = (List<string>)sortedlist[tagInfo.UseNum];
                if (tagNames == null || tagNames.Count == 0)
                {
                    tagNames = new List<string>();
                }
                tagNames.Add(tagInfo.Tag);
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
                    if (tagNames.Contains(tagInfo.Tag))
                    {
                        tagInfo.Level = level;
                    }
                }
            }

            if (tagLevel > 1)
            {
                var levelList = new List<TagInfo>();
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
