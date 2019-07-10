using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;
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

        public async Task<List<TableStyle>> GetStyleInfoListAsync(string tableName, List<int> relatedIdentities)
        {
            var allAttributeNames = new List<string>();
            var styleInfoList = new List<TableStyle>();

            var entries = await _tableStyleRepository.GetAllTableStylesAsync();
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
                var columnNames = await GetTableColumnNameListAsync(tableName, ContentAttribute.MetadataAttributes.Value);

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

        public async Task<List<TableStyle>> GetSiteStyleInfoListAsync(int siteId)
        {
            var relatedIdentities = GetRelatedIdentities(siteId);
            return await GetStyleInfoListAsync(_siteRepository.TableName, relatedIdentities);
        }

        public async Task<List<TableStyle>> GetChannelStyleInfoListAsync(Channel channelInfo)
        {
            var relatedIdentities = GetRelatedIdentities(channelInfo);
            return await GetStyleInfoListAsync(_channelRepository.TableName, relatedIdentities);
        }

        public async Task<List<TableStyle>> GetContentStyleInfoListAsync(IPluginManager pluginManager, Site siteInfo, Channel channelInfo)
        {
            var relatedIdentities = GetRelatedIdentities(channelInfo);
            var tableName = await _channelRepository.GetTableNameAsync(pluginManager, siteInfo, channelInfo);
            return await GetStyleInfoListAsync(tableName, relatedIdentities);
        }

        public async Task<List<TableStyle>> GetUserStyleInfoListAsync()
        {
            var relatedIdentities = EmptyRelatedIdentities;
            return await GetStyleInfoListAsync(_userRepository.TableName, relatedIdentities);
        }

        public IDictionary<string, object> GetDefaultAttributes(List<TableStyle> styleInfoList)
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
        public async Task<TableStyle> GetTableStyleInfoAsync(string tableName, string attributeName, List<int> relatedIdentities)
        {
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            var entries = await _tableStyleRepository.GetAllTableStylesAsync();
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

            return new TableStyle
            {
                TableName = tableName,
                AttributeName = attributeName,
                DisplayName = attributeName,
                IsVisibleInList = false,
                Type = InputType.Text,
                IsHorizontal = true
            };
        }

        public async Task<TableStyle> GetTableStyleInfoAsync(int id)
        {
            var entries = await _tableStyleRepository.GetAllTableStylesAsync();

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

        public async Task<Dictionary<string, List<TableStyle>>> GetTableStyleInfoWithItemsDictionaryAsync(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyle>>();

            var entries = await _tableStyleRepository.GetAllTableStylesAsync();
            foreach (var pair in entries)
            {
                var arr = pair.Key.Split('$');
                var identityFromKey = TranslateUtils.ToInt(arr[0]);
                var tableNameFromKey = arr[1];
                if (!StringUtils.EqualsIgnoreCase(tableNameFromKey, tableName) ||
                    !allRelatedIdentities.Contains(identityFromKey)) continue;

                var styleInfo = pair.Value;
                var tableStyleInfoWithItemList = dict.ContainsKey(styleInfo.AttributeName) ? dict[styleInfo.AttributeName] : new List<TableStyle>();
                tableStyleInfoWithItemList.Add(styleInfo);
                dict[styleInfo.AttributeName] = tableStyleInfoWithItemList;
            }

            return dict;
        }

        public string GetValidateInfo(TableStyle styleInfo)
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

        public List<int> GetRelatedIdentities(Channel channelInfo)
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

        private TableStyle GetDefaultContentTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyle
            {
                TableName = tableName,
                AttributeName = attributeName,
                DisplayName = attributeName,
                IsVisibleInList = false,
                Type = InputType.Text,
                IsHorizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Title)))
            {
                styleInfo.AttributeName = nameof(Content.Title);
                styleInfo.DisplayName = "标题";
                styleInfo.VeeValidate = "required";
                styleInfo.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.SubTitle)))
            {
                styleInfo.AttributeName = nameof(Content.SubTitle);
                styleInfo.DisplayName = "副标题";
                styleInfo.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.ImageUrl)))
            {
                styleInfo.AttributeName = nameof(Content.ImageUrl);
                styleInfo.DisplayName = "图片";
                styleInfo.Type = InputType.Image;
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.VideoUrl)))
            {
                styleInfo.AttributeName = nameof(Content.VideoUrl);
                styleInfo.DisplayName = "视频";
                styleInfo.Type = InputType.Video;
                styleInfo.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.FileUrl)))
            {
                styleInfo.AttributeName = nameof(Content.FileUrl);
                styleInfo.DisplayName = "附件";
                styleInfo.Type = InputType.File;
                styleInfo.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Body)))
            {
                styleInfo.AttributeName = nameof(Content.Body);
                styleInfo.DisplayName = "内容";
                styleInfo.VeeValidate = "required";
                styleInfo.Type = InputType.TextEditor;
                styleInfo.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Summary)))
            {
                styleInfo.AttributeName = nameof(Content.Summary);
                styleInfo.DisplayName = "内容摘要";
                styleInfo.Type = InputType.TextArea;
                styleInfo.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Author)))
            {
                styleInfo.AttributeName = nameof(Content.Author);
                styleInfo.DisplayName = "作者";
                styleInfo.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Source)))
            {
                styleInfo.AttributeName = nameof(Content.Source);
                styleInfo.DisplayName = "来源";
                styleInfo.Taxis = 9;
            }

            return styleInfo;
        }

        private TableStyle GetDefaultUserTableStyleInfo(string tableName, string attributeName)
        {
            var styleInfo = new TableStyle
            {
                TableName = tableName,
                AttributeName = attributeName,
                DisplayName = attributeName,
                IsVisibleInList = false,
                Type = InputType.Text,
                IsHorizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.DisplayName)))
            {
                styleInfo.AttributeName = nameof(User.DisplayName);
                styleInfo.DisplayName = "姓名";
                styleInfo.VeeValidate = "required";
                styleInfo.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Mobile)))
            {
                styleInfo.AttributeName = nameof(User.Mobile);
                styleInfo.DisplayName = "手机号";
                styleInfo.VeeValidate = "mobile";
                styleInfo.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Email)))
            {
                styleInfo.AttributeName = nameof(User.Email);
                styleInfo.DisplayName = "邮箱";
                styleInfo.VeeValidate = "email";
                styleInfo.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Bio)))
            {
                styleInfo.AttributeName = nameof(User.Bio);
                styleInfo.Type = InputType.TextArea;
                styleInfo.DisplayName = "个人简介";
                styleInfo.Taxis = 9;
            }

            return styleInfo;
        }
    }

}
