using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class TableStyleManager
    {
        private static readonly object LockObject = new object();
        private static bool _async = true;//缓存与数据库不同步
        private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TableStyleManager));

        private static List<Tuple<string, TableStyleInfo>> GetAllTableStyles()
        {
            lock (LockObject)
            {
                var entries = DataCacheManager.Get<List<Tuple<string, TableStyleInfo>>>(CacheKey);

                if (!_async && entries != null) return entries;

                entries = DataProvider.TableStyleDao.GetAllTableStyles();
                DataCacheManager.Insert(CacheKey, entries);
                _async = false;
                return entries;
            }
        }

        public static List<TableStyleInfo> GetTableStyleInfoList(string tableName, List<int> relatedIdentities)
        {
            var allAttributeNames = new List<string>();
            var styleInfoList = new List<TableStyleInfo>();

            var entries = GetAllTableStyles();
            relatedIdentities = GetRelatedIdentities(relatedIdentities);
            foreach (var relatedIdentity in relatedIdentities)
            {
                var startKey = GetCacheKeyStart(relatedIdentity, tableName);
                var tuples = entries.Where(tuple => tuple.Item1.StartsWith(startKey)).ToList();
                foreach (var tuple in tuples)
                {
                    if (tuple?.Item2 == null) continue;

                    if (!allAttributeNames.Contains(tuple.Item2.AttributeName))
                    {
                        allAttributeNames.Add(tuple.Item2.AttributeName);
                        styleInfoList.Add(tuple.Item2);
                    }
                }
            }

            if (SiteManager.IsSiteTable(tableName))
            {
                var columnNames = TableColumnManager.GetTableColumnNameList(tableName, ContentAttribute.MetadataAttributes);

                foreach (var columnName in columnNames)
                {
                    if (!StringUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                    {
                        allAttributeNames.Add(columnName);
                        styleInfoList.Add(GetDefaultTableStyleInfo(tableName, columnName));
                    }
                }
            }

            return styleInfoList.OrderBy(styleInfo => styleInfo.Taxis == 0 ? int.MaxValue : styleInfo.Taxis).ToList();
        }

        public static IAttributes GetDefaultAttributes(List<TableStyleInfo> styleInfoList)
        {
            var attributes = new AttributesImpl();

            foreach (var styleInfo in styleInfoList)
            {
                var defaultValue = string.Empty;
                if (!string.IsNullOrEmpty(styleInfo.DefaultValue))
                {
                    defaultValue = styleInfo.DefaultValue;
                }
                else if (styleInfo.StyleItems != null)
                {
                    var defaultValues = new List<string>();
                    foreach (var styleItem in styleInfo.StyleItems)
                    {
                        if (styleItem.IsSelected)
                        {
                            defaultValues.Add(styleItem.ItemValue);
                        }
                    }
                    if (defaultValues.Count > 0)
                    {
                        defaultValue = TranslateUtils.ObjectCollectionToString(defaultValues);
                    }
                }

                if (!string.IsNullOrEmpty(defaultValue))
                {
                    attributes.Set(styleInfo.AttributeName, defaultValue);
                }
            }

            return attributes;
        }

        public static bool IsChanged
        {
            set
            {
                lock (LockObject)
                {
                    if (value)
                    {
                        DataCacheManager.Remove(CacheKey);
                    }
                    _async = value;
                }
            }
        }

        public static string GetCacheKey(int relatedIdentity, string tableName, string attributeName)
        {
            return $"{relatedIdentity}${tableName}${attributeName}".ToLower();
        }

        private static string GetCacheKeyStart(int relatedIdentity, string tableName)
        {
            return $"{relatedIdentity}${tableName}$".ToLower();
        }

        //relatedIdentities从大到小，最后是0
        public static TableStyleInfo GetTableStyleInfo(string tableName, string attributeName, List<int> relatedIdentities)
        {
            TableStyleInfo styleInfo = null;
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = GetRelatedIdentities(relatedIdentities);

            var entries = GetAllTableStyles();

            foreach (var relatedIdentity in relatedIdentities)
            {
                var key = GetCacheKey(relatedIdentity, tableName, attributeName);
                if (entries.Any(tuple => tuple.Item1 == key))
                {
                    var tuple = entries.FirstOrDefault(pair => pair?.Item2 != null && pair.Item1 == key);
                    styleInfo = tuple?.Item2;
                    if (styleInfo != null)
                    {
                        break;
                    }
                }
            }

            return styleInfo ?? GetDefaultTableStyleInfo(tableName, attributeName);
        }

        public static TableStyleInfo GetTableStyleInfo(int id)
        {
            var entries = GetAllTableStyles();

            var entry = entries.FirstOrDefault(tuple => tuple.Item2 != null && tuple.Item2.Id == id);
            return entry?.Item2;
        }

        private static List<int> GetRelatedIdentities(IReadOnlyCollection<int> list)
        {
            var relatedIdentities = new List<int>();

            if (list != null && list.Count > 0)
            {
                foreach (var i in list)
                {
                    if (!relatedIdentities.Contains(i))
                    {
                        relatedIdentities.Add(i);
                    }
                }
            }

            relatedIdentities.Sort();
            relatedIdentities.Reverse();

            if (!relatedIdentities.Contains(0))
            {
                relatedIdentities.Add(0);
            }

            return relatedIdentities;
        }

        public static bool IsExists(int relatedIdentity, string tableName, string attributeName)
        {
            var key = GetCacheKey(relatedIdentity, tableName, attributeName);
            var entries = GetAllTableStyles();
            return entries.Any(tuple => tuple.Item1 == key);
        }

        public static Dictionary<string, List<TableStyleInfo>> GetTableStyleInfoWithItemsDictinary(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyleInfo>>();

            var entries = GetAllTableStyles();
            foreach (var tuple in entries)
            {
                var identityFromKey = TranslateUtils.ToInt(tuple.Item1.Split('$')[0]);
                var tableNameFromKey = tuple.Item1.Split('$')[1];
                if (!StringUtils.EqualsIgnoreCase(tableNameFromKey, tableName) ||
                    !allRelatedIdentities.Contains(identityFromKey)) continue;

                var styleInfo = tuple.Item2;
                //if (InputTypeUtils.IsWithStyleItems(styleInfo.InputType))
                //{
                //    styleInfo.StyleItems = DataProvider.TableStyleItemDao.GetStyleItemInfoList(styleInfo.Id);
                //}
                var tableStyleInfoWithItemList = dict.ContainsKey(styleInfo.AttributeName) ? dict[styleInfo.AttributeName] : new List<TableStyleInfo>();
                tableStyleInfoWithItemList.Add(styleInfo);
                dict[styleInfo.AttributeName] = tableStyleInfoWithItemList;
            }

            return dict;
        }

        public static string GetValidateInfo(TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();
            if (styleInfo.Additional.IsRequired)
            {
                builder.Append("必填项;");
            }
            if (styleInfo.Additional.MinNum > 0)
            {
                builder.Append($"最少{styleInfo.Additional.MinNum}个字符;");
            }
            if (styleInfo.Additional.MaxNum > 0)
            {
                builder.Append($"最多{styleInfo.Additional.MaxNum}个字符;");
            }
            if (styleInfo.Additional.ValidateType != ValidateType.None)
            {
                builder.Append($"验证:{ValidateTypeUtils.GetText(styleInfo.Additional.ValidateType)};");
            }

            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 1;
            }
            else
            {
                builder.Append("无验证");
            }
            return builder.ToString();
        }

        private static TableStyleInfo GetDefaultTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, false, InputType.Text, string.Empty, true, string.Empty);

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Title)))
            {
                styleInfo.DisplayName = "标题";
                styleInfo.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.SubTitle)))
            {
                styleInfo.DisplayName = "副标题";
                styleInfo.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.ImageUrl)))
            {
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = InputType.Image;
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.VideoUrl)))
            {
                styleInfo.DisplayName = "视频";
                styleInfo.InputType = InputType.Video;
                styleInfo.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.FileUrl)))
            {
                styleInfo.DisplayName = "附件";
                styleInfo.InputType = InputType.File;
                styleInfo.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Content)))
            {
                styleInfo.DisplayName = "内容";
                styleInfo.InputType = InputType.TextEditor;
                styleInfo.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Summary)))
            {
                styleInfo.DisplayName = "内容摘要";
                styleInfo.InputType = InputType.TextArea;
                styleInfo.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Author)))
            {
                styleInfo.DisplayName = "作者";
                styleInfo.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Source)))
            {
                styleInfo.DisplayName = "来源";
                styleInfo.Taxis = 9;
            }

            return styleInfo;
        }
    }

}
