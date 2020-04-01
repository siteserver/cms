using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.DataCache
{
    public static class ContentManager
    {
        private static class ListCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ListCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                lock(LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    DataCacheManager.Remove(cacheKey);
                }
            }

            public static List<int> GetContentIdList(int channelId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    var list = DataCacheManager.Get<List<int>>(cacheKey);
                    if (list != null) return list;

                    list = new List<int>();
                    DataCacheManager.Insert(cacheKey, list);
                    return list;
                }
            }
        }

        private static class ContentCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CachePrefix = DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(ContentCache);

            private static string GetCacheKey(int channelId)
            {
                return $"{CachePrefix}.{channelId}";
            }

            public static void Remove(int channelId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    DataCacheManager.Remove(cacheKey);
                }
            }

            public static Dictionary<int, ContentInfo> GetContentDict(int channelId)
            {
                lock (LockObject)
                {
                    var cacheKey = GetCacheKey(channelId);
                    var dict = DataCacheManager.Get<Dictionary<int, ContentInfo>>(cacheKey);
                    if (dict == null)
                    {
                        dict = new Dictionary<int, ContentInfo>();
                        DataCacheManager.InsertHours(cacheKey, dict, 12);
                    }

                    return dict;
                }
            }

            public static ContentInfo GetContent(SiteInfo siteInfo, int channelId, int contentId)
            {
                lock (LockObject)
                {
                    var dict = GetContentDict(channelId);
                    dict.TryGetValue(contentId, out var contentInfo);
                    if (contentInfo != null && contentInfo.ChannelId == channelId && contentInfo.Id == contentId) return contentInfo;

                    contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelId), channelId, contentId);
                    dict[contentId] = contentInfo;

                    return new ContentInfo(contentInfo);
                }
            }

            public static ContentInfo GetContent(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
            {
                lock (LockObject)
                {
                    var dict = GetContentDict(channelInfo.Id);
                    dict.TryGetValue(contentId, out var contentInfo);
                    if (contentInfo != null && contentInfo.ChannelId == channelInfo.Id && contentInfo.Id == contentId) return contentInfo;

                    contentInfo = DataProvider.ContentDao.GetCacheContentInfo(ChannelManager.GetTableName(siteInfo, channelInfo), channelInfo.Id, contentId);
                    dict[contentId] = contentInfo;

                    return new ContentInfo(contentInfo);
                }
            }
        }

        private static class CountCache
        {
            private static readonly object LockObject = new object();
            private static readonly string CacheKey =
                DataCacheManager.GetCacheKey(nameof(ContentManager)) + "." + nameof(CountCache);

            public static void Clear(string tableName)
            {
                lock (LockObject)
                {
                    var dict = GetAllContentCounts();
                    dict.Remove(tableName);
                }
            }

            public static Dictionary<string, List<ContentCountInfo>> GetAllContentCounts()
            {
                lock (LockObject)
                {
                    var retval = DataCacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CacheKey);
                    if (retval != null) return retval;

                    retval = DataCacheManager.Get<Dictionary<string, List<ContentCountInfo>>>(CacheKey);
                    if (retval == null)
                    {
                        retval = new Dictionary<string, List<ContentCountInfo>>();
                        DataCacheManager.Insert(CacheKey, retval);
                    }

                    return retval;
                }
            }
        }

        public static void RemoveCache(string tableName, int channelId)
        {
            ListCache.Remove(channelId);
            ContentCache.Remove(channelId);
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void RemoveCountCache(string tableName)
        {
            CountCache.Clear(tableName);
            StlContentCache.ClearCache();
        }

        public static void InsertCache(SiteInfo siteInfo, ChannelInfo channelInfo, IContentInfo contentInfo)
        {
            if (contentInfo.SourceId == SourceManager.Preview) return;

            var contentIdList = ListCache.GetContentIdList(channelInfo.Id);

            if (ETaxisTypeUtils.Equals(ETaxisType.OrderByTaxisDesc, channelInfo.Additional.DefaultTaxisType))
            {
                contentIdList.Insert(0, contentInfo.Id);
            }
            else
            {
                ListCache.Remove(channelInfo.Id);
            }

            var dict = ContentCache.GetContentDict(contentInfo.ChannelId);
            dict[contentInfo.Id] = (ContentInfo)contentInfo;

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var countInfoList = GetContentCountInfoList(tableName);
            var countInfo = countInfoList.FirstOrDefault(x =>
                x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id &&
                x.IsChecked == contentInfo.IsChecked.ToString() && x.CheckedLevel == contentInfo.CheckedLevel);
            if (countInfo != null) countInfo.Count++;

            StlContentCache.ClearCache();
            CountCache.Clear(tableName);
        }

        public static void UpdateCache(SiteInfo siteInfo, ChannelInfo channelInfo, IContentInfo contentInfoToUpdate)
        {
            var dict = ContentCache.GetContentDict(channelInfo.Id);

            var contentInfo = GetContentInfo(siteInfo, channelInfo, contentInfoToUpdate.Id);
            if (contentInfo != null)
            {
                var isClearCache = contentInfo.IsTop != contentInfoToUpdate.IsTop;

                if (!isClearCache)
                {
                    var orderAttributeName =
                        ETaxisTypeUtils.GetContentOrderAttributeName(
                            ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType));
                    if (contentInfo.Get(orderAttributeName) != contentInfoToUpdate.Get(orderAttributeName))
                    {
                        isClearCache = true;
                    }
                }

                if (isClearCache)
                {
                    ListCache.Remove(channelInfo.Id);
                }
            }

            
            dict[contentInfoToUpdate.Id] = (ContentInfo)contentInfoToUpdate;

            StlContentCache.ClearCache();
        }

        public static List<int> GetContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int offset, int limit)
        {
            var list = ListCache.GetContentIdList(channelInfo.Id);
            if (list.Count >= offset + limit)
            {
                return list.Skip(offset).Take(limit).ToList();
            }

            if (list.Count == offset)
            {
                var dict = ContentCache.GetContentDict(channelInfo.Id);
                var pageContentInfoList = DataProvider.ContentDao.GetCacheContentInfoList(siteInfo, channelInfo, offset, limit);
                foreach (var contentInfo in pageContentInfoList)
                {
                    dict[contentInfo.Id] = contentInfo;
                }

                var pageContentIdList = pageContentInfoList.Select(x => x.Id).ToList();
                list.AddRange(pageContentIdList);
                return pageContentIdList;
            }

            return DataProvider.ContentDao.GetCacheContentIdList(siteInfo, channelInfo, offset, limit);
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, int channelId, int contentId)
        {
            return ContentCache.GetContent(siteInfo, channelId, contentId);
        }

        public static ContentInfo GetContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            return ContentCache.GetContent(siteInfo, channelInfo, contentId);
        }

        public static int GetCount(SiteInfo siteInfo, bool isChecked)
        {
            var tableNames = SiteManager.GetTableNameList(siteInfo);
            var count = 0;
            foreach (var tableName in tableNames)
            {
                var list = GetContentCountInfoList(tableName);
                count += list.Where(x => x.SiteId == siteInfo.Id && x.IsChecked == isChecked.ToString()).Sum(x => x.Count);
            }

            return count;
        }

        public static int GetCount(SiteInfo siteInfo)
        {
            var tableNames = SiteManager.GetTableNameList(siteInfo);
            var count = 0;
            foreach (var tableName in tableNames)
            {
                var list = GetContentCountInfoList(tableName);
                count += list.Where(x => x.SiteId == siteInfo.Id).Sum(x => x.Count);
            }

            return count;
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var list = GetContentCountInfoList(tableName);
            return list.Where(x => x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id).Sum(x => x.Count);
        }

        public static int GetCount(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            var list = GetContentCountInfoList(tableName);
            return list.Where(x => x.SiteId == siteInfo.Id && x.ChannelId == channelInfo.Id && x.IsChecked == isChecked.ToString()).Sum(x => x.Count);
        }

        private static List<ContentCountInfo> GetContentCountInfoList(string tableName)
        {
            var dict = CountCache.GetAllContentCounts();
            dict.TryGetValue(tableName, out var countList);
            if (countList != null) return countList;

            countList = DataProvider.ContentDao.GetTableContentCounts(tableName);
            dict[tableName] = countList;

            return countList;
        }

        public static List<ContentColumn> GetContentColumns(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll)
        {
            var columns = new List<ContentColumn>();

            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.Additional.ContentAttributesOfDisplay);
            var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
            var pluginColumns = PluginContentManager.GetContentColumns(pluginIds);

            var styleInfoList = ContentUtility.GetAllTableStyleInfoList(TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo));

            styleInfoList.Insert(0, new TableStyleInfo
            {
                AttributeName = ContentAttribute.Sequence,
                DisplayName = "序号"
            });

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.InputType == InputType.TextEditor) continue;

                var column = new ContentColumn
                {
                    AttributeName = styleInfo.AttributeName,
                    DisplayName = styleInfo.DisplayName,
                    InputType = styleInfo.InputType
                };
                if (styleInfo.AttributeName == ContentAttribute.Title)
                {
                    column.IsList = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                    {
                        column.IsList = true;
                    }
                }

                if (StringUtils.ContainsIgnoreCase(ContentAttribute.CalculateAttributes.Value, styleInfo.AttributeName))
                {
                    column.IsCalculate = true;
                }

                if (includeAll || column.IsList)
                {
                    columns.Add(column);
                }
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        var column = new ContentColumn
                        {
                            AttributeName = attributeName,
                            DisplayName = $"{columnName}({pluginId})",
                            InputType = InputType.Text,
                            IsCalculate = true
                        };

                        if (attributesOfDisplay.Contains(attributeName))
                        {
                            column.IsList = true;
                        }

                        if (includeAll || column.IsList)
                        {
                            columns.Add(column);
                        }
                    }
                }
            }

            return columns;
        }

        public static ContentInfo Calculate(int sequence, ContentInfo contentInfo, List<ContentColumn> columns, Dictionary<string, Dictionary<string, Func<IContentContext, string>>> pluginColumns)
        {
            if (contentInfo == null) return null;

            var retval = new ContentInfo(contentInfo.ToDictionary());

            foreach (var column in columns)
            {
                if (!column.IsCalculate) continue;

                if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.Sequence))
                {
                    retval.Set(ContentAttribute.Sequence, sequence);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AdminId))
                {
                    var value = string.Empty;
                    if (contentInfo.AdminId > 0)
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserId(contentInfo.AdminId);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) ? adminInfo.UserName : adminInfo.DisplayName;
                        }
                    }
                    retval.Set(ContentAttribute.AdminId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.UserId))
                {
                    var value = string.Empty;
                    if (contentInfo.UserId > 0)
                    {
                        var userInfo = UserManager.GetUserInfoByUserId(contentInfo.UserId);
                        if (userInfo != null)
                        {
                            value = string.IsNullOrEmpty(userInfo.DisplayName) ? userInfo.UserName : userInfo.DisplayName;
                        }
                    }
                    retval.Set(ContentAttribute.UserId, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.SourceId))
                {
                    retval.Set(ContentAttribute.SourceId, SourceManager.GetSourceName(contentInfo.SourceId));
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.AddUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.AddUserName))
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserName(contentInfo.AddUserName);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) ? adminInfo.UserName : adminInfo.DisplayName;
                        }
                    }
                    retval.Set(ContentAttribute.AddUserName, value);
                }
                else if (StringUtils.EqualsIgnoreCase(column.AttributeName, ContentAttribute.LastEditUserName))
                {
                    var value = string.Empty;
                    if (!string.IsNullOrEmpty(contentInfo.LastEditUserName))
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserName(contentInfo.LastEditUserName);
                        if (adminInfo != null)
                        {
                            value = string.IsNullOrEmpty(adminInfo.DisplayName) ? adminInfo.UserName : adminInfo.DisplayName;
                        }
                    }
                    retval.Set(ContentAttribute.LastEditUserName, value);
                }
            }

            if (pluginColumns != null)
            {
                foreach (var pluginId in pluginColumns.Keys)
                {
                    var contentColumns = pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        if (columns.All(x => x.AttributeName != attributeName)) continue;

                        try
                        {
                            var func = contentColumns[columnName];
                            var value = func(new ContentContextImpl
                            {
                                SiteId = contentInfo.SiteId,
                                ChannelId = contentInfo.ChannelId,
                                ContentId = contentInfo.Id
                            });

                            retval.Set(attributeName, value);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(pluginId, ex);
                        }
                    }
                }
            }

            return retval;
        }

        public static bool IsCreatable(ChannelInfo channelInfo, IContentInfo contentInfo)
        {
            if (channelInfo == null || contentInfo == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (contentInfo.ReferenceId > 0 &&
                ETranslateContentTypeUtils.GetEnumType(contentInfo.GetString(ContentAttribute.TranslateContentType)) !=
                ETranslateContentType.ReferenceContent)
            {
                return false;
            }

            return channelInfo.Additional.IsContentCreatable && string.IsNullOrEmpty(contentInfo.LinkUrl) && contentInfo.IsChecked && contentInfo.SourceId != SourceManager.Preview && contentInfo.ChannelId > 0;
        }
    }
}