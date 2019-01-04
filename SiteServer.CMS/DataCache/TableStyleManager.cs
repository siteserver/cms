using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.DataCache
{
    public static class TableStyleManager
    {
        private static class TableStyleManagerCache
        {
            private static readonly object LockObject = new object();

            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TableStyleManager));

            public static List<KeyValuePair<string, TableStyleInfo>> GetAllTableStyles()
            {
                var retval = DataCacheManager.Get<List<KeyValuePair<string, TableStyleInfo>>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = DataCacheManager.Get<List<KeyValuePair<string, TableStyleInfo>>>(CacheKey);
                    if (retval == null)
                    {
                        retval = DataProvider.TableStyleDao.GetAllTableStyles();

                        DataCacheManager.Insert(CacheKey, retval);
                    }
                }

                return retval;
            }

            public static void Clear()
            {
                DataCacheManager.Remove(CacheKey);
            }
        }

        public static string GetKey(int relatedIdentity, string tableName, string attributeName)
        {
            return $"{GetKeyPrefix(relatedIdentity, tableName)}{attributeName}".ToLower();
        }

        private static string GetKeyPrefix(int relatedIdentity, string tableName)
        {
            return $"{relatedIdentity}${tableName}$".ToLower();
        }

        public static List<TableStyleInfo> GetStyleInfoList(string tableName, List<int> relatedIdentities)
        {
            var allAttributeNames = new List<string>();
            var styleInfoList = new List<TableStyleInfo>();

            var entries = TableStyleManagerCache.GetAllTableStyles();
            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            foreach (var relatedIdentity in relatedIdentities)
            {
                var startKey = GetKeyPrefix(relatedIdentity, tableName);
                var list = entries.Where(tuple => tuple.Key.StartsWith(startKey)).ToList();
                foreach (var pair in list)
                {
                    if (pair.IsDefault()) continue;

                    if (!allAttributeNames.Contains(pair.Value.AttributeName))
                    {
                        allAttributeNames.Add(pair.Value.AttributeName);
                        styleInfoList.Add(pair.Value);
                    }
                }
            }

            if (tableName == DataProvider.UserDao.TableName || tableName == DataProvider.ChannelDao.TableName || tableName == DataProvider.SiteDao.TableName)
            {
                if (tableName == DataProvider.UserDao.TableName)
                {
                    foreach (var columnName in UserAttribute.TableStyleAttributes.Value)
                    {
                        if (!StringUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                        {
                            allAttributeNames.Add(columnName);
                            styleInfoList.Add(GetDefaultUserTableStyleInfo(tableName, columnName));
                        }
                    }
                }
            }
            else
            {
                var columnNames = TableColumnManager.GetTableColumnNameList(tableName, ContentAttribute.MetadataAttributes.Value);

                foreach (var columnName in columnNames)
                {
                    if (!StringUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                    {
                        allAttributeNames.Add(columnName);
                        styleInfoList.Add(GetDefaultContentTableStyleInfo(tableName, columnName));
                    }
                }
            }

            return styleInfoList.OrderBy(styleInfo => styleInfo.Taxis == 0 ? int.MaxValue : styleInfo.Taxis).ToList();
        }

        public static List<TableStyleInfo> GetSiteStyleInfoList(int siteId)
        {
            var relatedIdentities = GetRelatedIdentities(siteId);
            return GetStyleInfoList(DataProvider.SiteDao.TableName, relatedIdentities);
        }

        public static List<TableStyleInfo> GetChannelStyleInfoList(ChannelInfo channelInfo)
        {
            var relatedIdentities = GetRelatedIdentities(channelInfo);
            return GetStyleInfoList(DataProvider.ChannelDao.TableName, relatedIdentities);
        }

        public static List<TableStyleInfo> GetContentStyleInfoList(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var relatedIdentities = GetRelatedIdentities(channelInfo);
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            return GetStyleInfoList(tableName, relatedIdentities);
        }

        public static List<TableStyleInfo> GetUserStyleInfoList()
        {
            var relatedIdentities = EmptyRelatedIdentities;
            return GetStyleInfoList(DataProvider.UserDao.TableName, relatedIdentities);
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

        public static void ClearCache()
        {
            TableStyleManagerCache.Clear();
        }

        //relatedIdentities从大到小，最后是0
        public static TableStyleInfo GetTableStyleInfo(string tableName, string attributeName, List<int> relatedIdentities)
        {
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            var entries = TableStyleManagerCache.GetAllTableStyles();
            foreach (var relatedIdentity in relatedIdentities)
            {
                var key = GetKey(relatedIdentity, tableName, attributeName);
                var pair = entries.FirstOrDefault(x => x.Key == key && x.Value != null);
                if (pair.IsDefault()) continue;

                if (pair.Value != null)
                {
                    return pair.Value;
                }
            }

            if (tableName == DataProvider.UserDao.TableName || tableName == DataProvider.ChannelDao.TableName || tableName == DataProvider.SiteDao.TableName)
            {
                if (tableName == DataProvider.UserDao.TableName)
                {
                    return GetDefaultUserTableStyleInfo(tableName, attributeName);
                }
            }
            else
            {
                return GetDefaultContentTableStyleInfo(tableName, attributeName);
            }

            return new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, false, InputType.Text, string.Empty, true, string.Empty);
        }

        public static TableStyleInfo GetTableStyleInfo(int id)
        {
            var entries = TableStyleManagerCache.GetAllTableStyles();

            var entry = entries.FirstOrDefault(x => x.Value != null && x.Value.Id == id);
            return entry.IsDefault() ? null : entry.Value;
        }

        private static List<int> ParseRelatedIdentities(IReadOnlyCollection<int> list)
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
            var key = GetKey(relatedIdentity, tableName, attributeName);
            var entries = TableStyleManagerCache.GetAllTableStyles();
            return entries.Any(x => x.Key == key);
        }

        public static Dictionary<string, List<TableStyleInfo>> GetTableStyleInfoWithItemsDictinary(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyleInfo>>();

            var entries = TableStyleManagerCache.GetAllTableStyles();
            foreach (var pair in entries)
            {
                var arr = pair.Key.Split('$');
                var identityFromKey = TranslateUtils.ToInt(arr[0]);
                var tableNameFromKey = arr[1];
                if (!StringUtils.EqualsIgnoreCase(tableNameFromKey, tableName) ||
                    !allRelatedIdentities.Contains(identityFromKey)) continue;

                var styleInfo = pair.Value;
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

        public static List<int> GetRelatedIdentities(int siteId)
        {
            return new List<int> {siteId, 0};
        }

        public static List<int> GetRelatedIdentities(ChannelInfo channelInfo)
        {
            var list = new List<int>();
            if (channelInfo != null)
            {
                var channelIdCollection = "0," + channelInfo.Id;
                if (channelInfo.ParentsCount > 0)
                {
                    channelIdCollection = "0," + channelInfo.ParentsPath + "," + channelInfo.Id;
                }

                list = TranslateUtils.StringCollectionToIntList(channelIdCollection);
                list.Reverse();
            }
            else
            {
                list.Add(0);
            }
            return list;
        }

        public static List<int> EmptyRelatedIdentities => new List<int> {0};

        private static TableStyleInfo GetDefaultContentTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, false, InputType.Text, string.Empty, true, string.Empty);

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Title)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Title);
                styleInfo.DisplayName = "标题";
                styleInfo.Additional.VeeValidate = "required";
                styleInfo.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.SubTitle)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.SubTitle);
                styleInfo.DisplayName = "副标题";
                styleInfo.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.ImageUrl)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.ImageUrl);
                styleInfo.DisplayName = "图片";
                styleInfo.InputType = InputType.Image;
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.VideoUrl)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.VideoUrl);
                styleInfo.DisplayName = "视频";
                styleInfo.InputType = InputType.Video;
                styleInfo.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.FileUrl)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.FileUrl);
                styleInfo.DisplayName = "附件";
                styleInfo.InputType = InputType.File;
                styleInfo.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Content)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Content);
                styleInfo.DisplayName = "内容";
                styleInfo.Additional.VeeValidate = "required";
                styleInfo.InputType = InputType.TextEditor;
                styleInfo.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Summary)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Summary);
                styleInfo.DisplayName = "内容摘要";
                styleInfo.InputType = InputType.TextArea;
                styleInfo.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Author)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Author);
                styleInfo.DisplayName = "作者";
                styleInfo.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Source)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Source);
                styleInfo.DisplayName = "来源";
                styleInfo.Taxis = 9;
            }

            return styleInfo;
        }

        private static TableStyleInfo GetDefaultUserTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo(0, 0, tableName, attributeName, 0, attributeName, string.Empty, false, InputType.Text, string.Empty, true, string.Empty);

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.DisplayName)))
            {
                styleInfo.AttributeName = nameof(UserInfo.DisplayName);
                styleInfo.DisplayName = "姓名";
                styleInfo.Additional.VeeValidate = "required";
                styleInfo.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Mobile)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Mobile);
                styleInfo.DisplayName = "手机号";
                styleInfo.Additional.VeeValidate = "mobile";
                styleInfo.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Email)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Email);
                styleInfo.DisplayName = "邮箱";
                styleInfo.Additional.VeeValidate = "email";
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Gender)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Gender);
                styleInfo.DisplayName = "性别";
                styleInfo.InputType = InputType.Radio;
                styleInfo.StyleItems = new List<TableStyleItemInfo>
                    {
                        new TableStyleItemInfo
                        {
                            ItemTitle = "男",
                            ItemValue = "男"
                        },
                        new TableStyleItemInfo
                        {
                            ItemTitle = "女",
                            ItemValue = "女"
                        }
                    };
                styleInfo.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Birthday)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Birthday);
                styleInfo.DisplayName = "出生日期";
                styleInfo.InputType = InputType.Date;
                styleInfo.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.WeiXin)))
            {
                styleInfo.AttributeName = nameof(UserInfo.WeiXin);
                styleInfo.DisplayName = "微博";
                styleInfo.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Qq)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Qq);
                styleInfo.DisplayName = "QQ";
                styleInfo.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.WeiBo)))
            {
                styleInfo.AttributeName = nameof(UserInfo.WeiBo);
                styleInfo.DisplayName = "微信";
                styleInfo.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Bio)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Bio);
                styleInfo.InputType = InputType.TextArea;
                styleInfo.DisplayName = "个人简介";
                styleInfo.Taxis = 9;
            }

            return styleInfo;
        }
    }

}
