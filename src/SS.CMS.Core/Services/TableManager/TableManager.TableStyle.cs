using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services.IPluginManager;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class TableManager
    {
        public static string GetKey(int relatedIdentity, string tableName, string attributeName)
        {
            return $"{GetKeyPrefix(relatedIdentity, tableName)}{attributeName}".ToLower();
        }

        private static string GetKeyPrefix(int relatedIdentity, string tableName)
        {
            return $"{relatedIdentity}${tableName}$".ToLower();
        }

        public List<TableStyleInfo> GetStyleInfoList(string tableName, List<int> relatedIdentities)
        {
            var allAttributeNames = new List<string>();
            var styleInfoList = new List<TableStyleInfo>();

            var entries = _tableStyleRepository.GetAllTableStyles();
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

            if (tableName == _userRepository.TableName || tableName == _channelRepository.TableName || tableName == _siteRepository.TableName)
            {
                if (tableName == _userRepository.TableName)
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
                var columnNames = GetTableColumnNameList(tableName, ContentAttribute.MetadataAttributes.Value);

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

        public List<TableStyleInfo> GetSiteStyleInfoList(int siteId)
        {
            var relatedIdentities = GetRelatedIdentities(siteId);
            return GetStyleInfoList(_siteRepository.TableName, relatedIdentities);
        }

        public List<TableStyleInfo> GetChannelStyleInfoList(ChannelInfo channelInfo)
        {
            var relatedIdentities = GetRelatedIdentities(channelInfo);
            return GetStyleInfoList(_channelRepository.TableName, relatedIdentities);
        }

        public List<TableStyleInfo> GetContentStyleInfoList(IPluginManager pluginManager, SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var relatedIdentities = GetRelatedIdentities(channelInfo);
            var tableName = _channelRepository.GetTableName(pluginManager, siteInfo, channelInfo);
            return GetStyleInfoList(tableName, relatedIdentities);
        }

        public List<TableStyleInfo> GetUserStyleInfoList()
        {
            var relatedIdentities = EmptyRelatedIdentities;
            return GetStyleInfoList(_userRepository.TableName, relatedIdentities);
        }

        public IDictionary<string, object> GetDefaultAttributes(List<TableStyleInfo> styleInfoList)
        {
            var attributes = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

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
                    attributes[styleInfo.AttributeName] = defaultValue;
                }
            }

            return attributes;
        }

        //relatedIdentities从大到小，最后是0
        public TableStyleInfo GetTableStyleInfo(string tableName, string attributeName, List<int> relatedIdentities)
        {
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            var entries = _tableStyleRepository.GetAllTableStyles();
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

            if (tableName == _userRepository.TableName || tableName == _channelRepository.TableName || tableName == _siteRepository.TableName)
            {
                if (tableName == _userRepository.TableName)
                {
                    return GetDefaultUserTableStyleInfo(tableName, attributeName);
                }
            }
            else
            {
                return GetDefaultContentTableStyleInfo(tableName, attributeName);
            }

            return new TableStyleInfo
            {
                TableName = tableName,
                AttributeName = attributeName,
                DisplayName = attributeName,
                IsVisibleInList = false,
                Type = InputType.Text,
                IsHorizontal = true
            };
        }

        public TableStyleInfo GetTableStyleInfo(int id)
        {
            var entries = _tableStyleRepository.GetAllTableStyles();

            var entry = entries.FirstOrDefault(x => x.Value != null && x.Value.Id == id);
            return entry.IsDefault() ? null : entry.Value;
        }

        private List<int> ParseRelatedIdentities(IReadOnlyCollection<int> list)
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

        public Dictionary<string, List<TableStyleInfo>> GetTableStyleInfoWithItemsDictionary(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyleInfo>>();

            var entries = _tableStyleRepository.GetAllTableStyles();
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

        public string GetValidateInfo(TableStyleInfo styleInfo)
        {
            var builder = new StringBuilder();
            if (styleInfo.IsRequired)
            {
                builder.Append("必填项;");
            }
            if (styleInfo.MinNum > 0)
            {
                builder.Append($"最少{styleInfo.MinNum}个字符;");
            }
            if (styleInfo.MaxNum > 0)
            {
                builder.Append($"最多{styleInfo.MaxNum}个字符;");
            }

            var validateType = ValidateType.GetValidateType(styleInfo.ValidateType);
            if (validateType != ValidateType.None)
            {
                builder.Append($"验证:{ValidateType.GetText(validateType)};");
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

        public List<int> GetRelatedIdentities(int siteId)
        {
            return new List<int> { siteId, 0 };
        }

        public List<int> GetRelatedIdentities(ChannelInfo channelInfo)
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

        public List<int> EmptyRelatedIdentities => new List<int> { 0 };

        private TableStyleInfo GetDefaultContentTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo
            {
                TableName = tableName,
                AttributeName = attributeName,
                DisplayName = attributeName,
                IsVisibleInList = false,
                Type = InputType.Text,
                IsHorizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Title)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Title);
                styleInfo.DisplayName = "标题";
                styleInfo.VeeValidate = "required";
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
                styleInfo.Type = InputType.Image;
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.VideoUrl)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.VideoUrl);
                styleInfo.DisplayName = "视频";
                styleInfo.Type = InputType.Video;
                styleInfo.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.FileUrl)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.FileUrl);
                styleInfo.DisplayName = "附件";
                styleInfo.Type = InputType.File;
                styleInfo.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Content)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Content);
                styleInfo.DisplayName = "内容";
                styleInfo.VeeValidate = "required";
                styleInfo.Type = InputType.TextEditor;
                styleInfo.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(ContentInfo.Summary)))
            {
                styleInfo.AttributeName = nameof(ContentInfo.Summary);
                styleInfo.DisplayName = "内容摘要";
                styleInfo.Type = InputType.TextArea;
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

        private TableStyleInfo GetDefaultUserTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyleInfo
            {
                TableName = tableName,
                AttributeName = attributeName,
                DisplayName = attributeName,
                IsVisibleInList = false,
                Type = InputType.Text,
                IsHorizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.DisplayName)))
            {
                styleInfo.AttributeName = nameof(UserInfo.DisplayName);
                styleInfo.DisplayName = "姓名";
                styleInfo.VeeValidate = "required";
                styleInfo.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Mobile)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Mobile);
                styleInfo.DisplayName = "手机号";
                styleInfo.VeeValidate = "mobile";
                styleInfo.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Email)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Email);
                styleInfo.DisplayName = "邮箱";
                styleInfo.VeeValidate = "email";
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(UserInfo.Bio)))
            {
                styleInfo.AttributeName = nameof(UserInfo.Bio);
                styleInfo.Type = InputType.TextArea;
                styleInfo.DisplayName = "个人简介";
                styleInfo.Taxis = 9;
            }

            return styleInfo;
        }
    }

}
