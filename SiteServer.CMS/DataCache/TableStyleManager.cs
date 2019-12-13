using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.DataCache
{
    public static class TableStyleManager
    {
        private static class TableStyleManagerCache
        {
            private static readonly string CacheKey = DataCacheManager.GetCacheKey(nameof(TableStyleManager));

            public static async Task<List<KeyValuePair<string, TableStyle>>> GetAllTableStylesAsync()
            {
                var retVal = DataCacheManager.Get<List<KeyValuePair<string, TableStyle>>>(CacheKey);
                if (retVal != null) return retVal;

                retVal = DataCacheManager.Get<List<KeyValuePair<string, TableStyle>>>(CacheKey);
                if (retVal == null)
                {
                    retVal = await DataProvider.TableStyleRepository.GetAllTableStylesAsync();

                    DataCacheManager.Insert(CacheKey, retVal);
                }

                return retVal;
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

        public static async Task<List<TableStyle>> GetStyleListAsync(string tableName, List<int> relatedIdentities)
        {
            var allAttributeNames = new List<string>();
            var styleList = new List<TableStyle>();

            var entries = await TableStyleManagerCache.GetAllTableStylesAsync();
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
                        styleList.Add(pair.Value);
                    }
                }
            }

            if (tableName == DataProvider.UserRepository.TableName || tableName == DataProvider.ChannelRepository.TableName || tableName == DataProvider.SiteRepository.TableName)
            {
                if (tableName == DataProvider.UserRepository.TableName)
                {
                    var tableStyleAttributes = new List<string>
                    {
                        nameof(User.DisplayName),
                        nameof(User.Mobile),
                        nameof(User.Email),
                        nameof(User.Gender),
                        nameof(User.Birthday),
                        nameof(User.WeiXin),
                        nameof(User.Qq),
                        nameof(User.WeiBo),
                        nameof(User.Bio)
                    };

                    foreach (var columnName in tableStyleAttributes)
                    {
                        if (!StringUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                        {
                            allAttributeNames.Add(columnName);
                            styleList.Add(GetDefaultUserTableStyle(tableName, columnName));
                        }
                    }
                }
            }
            else
            {
                var columnNames = await TableColumnManager.GetTableColumnNameListAsync(tableName, ContentAttribute.MetadataAttributes.Value);

                foreach (var columnName in columnNames)
                {
                    if (!StringUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                    {
                        allAttributeNames.Add(columnName);
                        styleList.Add(GetDefaultContentTableStyle(tableName, columnName));
                    }
                }
            }

            return styleList.OrderBy(style => style.Taxis == 0 ? int.MaxValue : style.Taxis).ToList();
        }

        public static async Task<List<TableStyle>> GetSiteStyleListAsync(int siteId)
        {
            var relatedIdentities = GetRelatedIdentities(siteId);
            return await GetStyleListAsync(DataProvider.SiteRepository.TableName, relatedIdentities);
        }

        public static async Task<List<TableStyle>> GetChannelStyleListAsync(Channel channel)
        {
            var relatedIdentities = GetRelatedIdentities(channel);
            return await GetStyleListAsync(DataProvider.ChannelRepository.TableName, relatedIdentities);
        }

        public static async Task<List<TableStyle>> GetContentStyleListAsync(Site site, Channel channel)
        {
            var relatedIdentities = GetRelatedIdentities(channel);
            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
            return await GetStyleListAsync(tableName, relatedIdentities);
        }

        public static async Task<List<TableStyle>> GetUserStyleListAsync()
        {
            var relatedIdentities = EmptyRelatedIdentities;
            return await GetStyleListAsync(DataProvider.UserRepository.TableName, relatedIdentities);
        }

        public static Dictionary<string, object> GetDefaultAttributes(List<TableStyle> styleList)
        {
            var attributes = new Dictionary<string, object>();

            foreach (var style in styleList)
            {
                var defaultValue = string.Empty;
                if (!string.IsNullOrEmpty(style.DefaultValue))
                {
                    defaultValue = style.DefaultValue;
                }
                else if (style.StyleItems != null)
                {
                    var defaultValues = new List<string>();
                    foreach (var styleItem in style.StyleItems)
                    {
                        if (styleItem.Selected)
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
                    attributes[style.AttributeName] = defaultValue;
                }
            }

            return attributes;
        }

        public static void ClearCache()
        {
            TableStyleManagerCache.Clear();
        }

        //relatedIdentities从大到小，最后是0
        public static async Task<TableStyle> GetTableStyleAsync(string tableName, string attributeName, List<int> relatedIdentities)
        {
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            var entries = await TableStyleManagerCache.GetAllTableStylesAsync();
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

            if (tableName == DataProvider.UserRepository.TableName || tableName == DataProvider.ChannelRepository.TableName || tableName == DataProvider.SiteRepository.TableName)
            {
                if (tableName == DataProvider.UserRepository.TableName)
                {
                    return GetDefaultUserTableStyle(tableName, attributeName);
                }
            }
            else
            {
                return GetDefaultContentTableStyle(tableName, attributeName);
            }

            return new TableStyle
            {
                Id = 0,
                RelatedIdentity = 0,
                TableName = tableName,
                AttributeName = attributeName,
                Taxis = 0,
                DisplayName = attributeName,
                HelpText = string.Empty,
                VisibleInList = false,
                Type = InputType.Text,
                DefaultValue = string.Empty,
                Horizontal = true
            };
        }

        public static async Task<TableStyle> GetTableStyleAsync(int id)
        {
            var entries = await TableStyleManagerCache.GetAllTableStylesAsync();

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

        public static async Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName)
        {
            var key = GetKey(relatedIdentity, tableName, attributeName);
            var entries = await TableStyleManagerCache.GetAllTableStylesAsync();
            return entries.Any(x => x.Key == key);
        }

        public static async Task<Dictionary<string, List<TableStyle>>> GetTableStyleWithItemsDictionaryAsync(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyle>>();

            var entries = await TableStyleManagerCache.GetAllTableStylesAsync();
            foreach (var pair in entries)
            {
                var arr = pair.Key.Split('$');
                var identityFromKey = TranslateUtils.ToInt(arr[0]);
                var tableNameFromKey = arr[1];
                if (!StringUtils.EqualsIgnoreCase(tableNameFromKey, tableName) ||
                    !allRelatedIdentities.Contains(identityFromKey)) continue;

                var style = pair.Value;
                var tableStyleWithItemList = dict.ContainsKey(style.AttributeName) ? dict[style.AttributeName] : new List<TableStyle>();
                tableStyleWithItemList.Add(style);
                dict[style.AttributeName] = tableStyleWithItemList;
            }

            return dict;
        }

        public static string GetValidateInfo(TableStyle style)
        {
            var builder = new StringBuilder();
            if (style.IsRequired)
            {
                builder.Append("必填项;");
            }
            if (style.MinNum > 0)
            {
                builder.Append($"最少{style.MinNum}个字符;");
            }
            if (style.MaxNum > 0)
            {
                builder.Append($"最多{style.MaxNum}个字符;");
            }
            if (style.ValidateType != ValidateType.None)
            {
                builder.Append($"验证:{ValidateTypeUtils.GetText(style.ValidateType)};");
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

        public static List<int> GetRelatedIdentities(Channel channel)
        {
            var list = new List<int>();
            if (channel != null)
            {
                var channelIdCollection = "0," + channel.Id;
                if (channel.ParentsCount > 0)
                {
                    channelIdCollection = "0," + channel.ParentsPath + "," + channel.Id;
                }

                list = StringUtils.GetIntList(channelIdCollection);
                list.Reverse();
            }
            else
            {
                list.Add(0);
            }
            return list;
        }

        public static List<int> EmptyRelatedIdentities => new List<int> {0};

        private static TableStyle GetDefaultContentTableStyle(string tableName, string attributeName)
        {
            var style = new TableStyle
            {
                Id = 0,
                RelatedIdentity = 0,
                TableName = tableName,
                AttributeName = attributeName,
                Taxis = 0,
                DisplayName = attributeName,
                HelpText = string.Empty,
                VisibleInList = false,
                Type = InputType.Text,
                DefaultValue = string.Empty,
                Horizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Title)))
            {
                style.AttributeName = nameof(Content.Title);
                style.DisplayName = "标题";
                style.VeeValidate = "required";
                style.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SubTitle))
            {
                style.AttributeName = ContentAttribute.SubTitle;
                style.DisplayName = "副标题";
                style.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.ImageUrl))
            {
                style.AttributeName = ContentAttribute.ImageUrl;
                style.DisplayName = "图片";
                style.Type = InputType.Image;
                style.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.VideoUrl))
            {
                style.AttributeName = ContentAttribute.VideoUrl;
                style.DisplayName = "视频";
                style.Type = InputType.Video;
                style.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.FileUrl))
            {
                style.AttributeName = ContentAttribute.FileUrl;
                style.DisplayName = "附件";
                style.Type = InputType.File;
                style.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Content))
            {
                style.AttributeName = ContentAttribute.Content;
                style.DisplayName = "内容";
                style.VeeValidate = "required";
                style.Type = InputType.TextEditor;
                style.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Summary))
            {
                style.AttributeName = ContentAttribute.Summary;
                style.DisplayName = "内容摘要";
                style.Type = InputType.TextArea;
                style.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Author))
            {
                style.AttributeName = ContentAttribute.Author;
                style.DisplayName = "作者";
                style.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Source))
            {
                style.AttributeName = ContentAttribute.Source;
                style.DisplayName = "来源";
                style.Taxis = 9;
            }

            return style;
        }

        private static TableStyle GetDefaultUserTableStyle(string tableName, string attributeName)
        {
            var style = new TableStyle
            {
                Id = 0,
                RelatedIdentity = 0,
                TableName = tableName,
                AttributeName = attributeName,
                Taxis = 0,
                DisplayName = attributeName,
                HelpText = string.Empty,
                VisibleInList = false,
                Type = InputType.Text,
                DefaultValue = string.Empty,
                Horizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.DisplayName)))
            {
                style.AttributeName = nameof(User.DisplayName);
                style.DisplayName = "姓名";
                style.VeeValidate = "required";
                style.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Mobile)))
            {
                style.AttributeName = nameof(User.Mobile);
                style.DisplayName = "手机号";
                style.VeeValidate = "mobile";
                style.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Email)))
            {
                style.AttributeName = nameof(User.Email);
                style.DisplayName = "邮箱";
                style.VeeValidate = "email";
                style.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Gender)))
            {
                style.AttributeName = nameof(User.Gender);
                style.DisplayName = "性别";
                style.Type = InputType.Radio;
                style.StyleItems = new List<TableStyleItem>
                    {
                        new TableStyleItem
                        {
                            ItemTitle = "男",
                            ItemValue = "男"
                        },
                        new TableStyleItem
                        {
                            ItemTitle = "女",
                            ItemValue = "女"
                        }
                    };
                style.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Birthday)))
            {
                style.AttributeName = nameof(User.Birthday);
                style.DisplayName = "出生日期";
                style.Type = InputType.Date;
                style.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.WeiXin)))
            {
                style.AttributeName = nameof(User.WeiXin);
                style.DisplayName = "微博";
                style.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Qq)))
            {
                style.AttributeName = nameof(User.Qq);
                style.DisplayName = "QQ";
                style.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.WeiBo)))
            {
                style.AttributeName = nameof(User.WeiBo);
                style.DisplayName = "微信";
                style.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Bio)))
            {
                style.AttributeName = nameof(User.Bio);
                style.Type = InputType.TextArea;
                style.DisplayName = "个人简介";
                style.Taxis = 9;
            }

            return style;
        }
    }

}
