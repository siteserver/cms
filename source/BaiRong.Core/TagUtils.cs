using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System;
using System.Collections.Generic;
using BaiRong.Core.Model;

namespace BaiRong.Core
{
    public class TagUtils
    {
        private TagUtils()
        {
        }

        public static void AddTags(StringCollection tags, int publishmentSystemId, int contentId)
        {
            if (tags.Count > 0)
            {
                foreach (var tagName in tags)
                {
                    var tagInfo = BaiRongDataProvider.TagDao.GetTagInfo(publishmentSystemId,PageUtils.FilterXss(tagName));
                    if (tagInfo != null)
                    {
                        var contentIdList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                        if (!contentIdList.Contains(contentId))
                        {
                            contentIdList.Add(contentId);
                            tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(contentIdList);
                            tagInfo.UseNum = contentIdList.Count;
                            BaiRongDataProvider.TagDao.Update(tagInfo);
                        }
                    }
                    else
                    {
                        tagInfo = new TagInfo(0, publishmentSystemId, contentId.ToString(), tagName, contentId > 0 ? 1 : 0);
                        BaiRongDataProvider.TagDao.Insert(tagInfo);
                    }
                }
            }
        }

        public static void UpdateTags(string tagsLast, string tagsNow, StringCollection tagCollection, int publishmentSystemId, int contentId)
        {
            if (tagsLast != tagsNow)
            {
                var tagsList = TranslateUtils.StringCollectionToStringList(tagsLast);
                foreach (string tag in tagsList)
                {
                    if (!tagCollection.Contains(tag))//删除
                    {
                        var tagInfo = BaiRongDataProvider.TagDao.GetTagInfo(publishmentSystemId, tag);
                        if (tagInfo != null)
                        {
                            var contentIdList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                            contentIdList.Remove(contentId);
                            tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(contentIdList);
                            tagInfo.UseNum = contentIdList.Count;
                            BaiRongDataProvider.TagDao.Update(tagInfo);
                        }
                    }
                }

                var tagsToAdd = new StringCollection();
                foreach (var tag in tagCollection)
                {
                    if (!tagsList.Contains(tag))
                    {
                        tagsToAdd.Add(tag);
                    }
                }

                AddTags(tagsToAdd, publishmentSystemId, contentId);
            }
        }

        public static void RemoveTags(int publishmentSystemId, List<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                RemoveTags(publishmentSystemId, contentId);
            }
        }

        public static void RemoveTags(int publishmentSystemId, int contentId)
        {
            var tagInfoList = BaiRongDataProvider.TagDao.GetTagInfoList(publishmentSystemId, contentId);
            if (tagInfoList.Count > 0)
            {
                foreach (var tagInfo in tagInfoList)
                {
                    var contentIdList = TranslateUtils.StringCollectionToIntList(tagInfo.ContentIdCollection);
                    contentIdList.Remove(contentId);
                    tagInfo.ContentIdCollection = TranslateUtils.ObjectCollectionToString(contentIdList);
                    tagInfo.UseNum = contentIdList.Count;
                    BaiRongDataProvider.TagDao.Update(tagInfo);
                }
            }
        }

        public static string GetTagsString(StringCollection tags)
        {
            var tagsBuilder = new StringBuilder();
            if (tags != null && tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    if (tag.Trim().IndexOf(",", StringComparison.Ordinal) != -1)
                    {
                        tagsBuilder.Append($"\"{tag}\"");
                    }
                    else
                    {
                        tagsBuilder.Append(tag);
                    }
                    tagsBuilder.Append(" ");
                }
                --tagsBuilder.Length;
            }
            return tagsBuilder.ToString();
        }

        public static StringCollection ParseTagsString(string tagsString)
        {
            var stringCollection = new StringCollection();

            if (!string.IsNullOrEmpty(tagsString))
            {
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
            }

            return stringCollection;
        }

        public static List<TagInfo> GetTagInfoList(int publishmentSystemId)
        {
            return GetTagInfoList(publishmentSystemId, 0, 0, false, 0);
        }

        public static List<TagInfo> GetTagInfoList(int publishmentSystemId, int contentId, int totalNum, bool isOrderByCount, int tagLevel)
        {
            var tagInfoList = BaiRongDataProvider.TagDao.GetTagInfoList(publishmentSystemId, contentId, isOrderByCount, totalNum);

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
