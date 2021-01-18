using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public partial class TableStyleRepository
    {
        public async Task<List<TableStyle>> GetTableStylesAsync(string tableName, List<int> relatedIdentities, List<string> excludeAttributeNames = null)
        {
            var allAttributeNames = new List<string>();
            var styleList = new List<TableStyle>();

            var styles = await GetAllAsync(tableName);
            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            foreach (var relatedIdentity in relatedIdentities)
            {
                var list = styles.Where(style =>
                    style.RelatedIdentity == relatedIdentity && style.TableName == tableName);

                foreach (var style in list)
                {
                    if (!allAttributeNames.Contains(style.AttributeName))
                    {
                        allAttributeNames.Add(style.AttributeName);
                        styleList.Add(style);
                    }
                }
            }

            var userTableName = _userRepository.TableName;
            var channelTableName = _channelRepository.TableName;
            var siteTableName = _siteRepository.TableName;

            if (tableName == userTableName || tableName == channelTableName || tableName == siteTableName)
            {
                if (tableName == userTableName)
                {
                    var tableStyleAttributes = new List<string>
                    {
                        nameof(User.DisplayName)
                    };

                    foreach (var columnName in tableStyleAttributes)
                    {
                        if (!ListUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                        {
                            allAttributeNames.Add(columnName);
                            styleList.Add(GetDefaultUserTableStyle(tableName, columnName));
                        }
                    }
                }
            }
            else
            {
                var excludeAttributeNameList = ColumnsManager.MetadataAttributes.Value;
                if (excludeAttributeNames != null)
                {
                    excludeAttributeNameList.AddRange(excludeAttributeNames);
                }

                var  list = await _repository.Database.GetTableColumnsAsync(tableName);
                if (excludeAttributeNameList != null && excludeAttributeNameList.Count > 0)
                {
                    list = list.Where(tableColumnInfo =>
                        !ListUtils.ContainsIgnoreCase(excludeAttributeNameList, tableColumnInfo.AttributeName)).ToList();
                }

                var columnNames = list.Select(tableColumnInfo => tableColumnInfo.AttributeName).ToList();

                foreach (var columnName in columnNames)
                {
                    if (!ListUtils.ContainsIgnoreCase(allAttributeNames, columnName))
                    {
                        allAttributeNames.Add(columnName);
                        styleList.Add(GetDefaultContentTableStyle(tableName, columnName));
                    }
                }
            }

            return styleList.OrderBy(style => style.Taxis == 0 ? int.MaxValue : style.Taxis).ToList();
        }

        public async Task<List<TableStyle>> GetSiteStylesAsync(int siteId)
        {
            var relatedIdentities = GetRelatedIdentities(siteId);
            var siteTableName = _siteRepository.TableName;
            return await GetTableStylesAsync(siteTableName, relatedIdentities);
        }

        public async Task<List<TableStyle>> GetChannelStylesAsync(Channel channel)
        {
            var relatedIdentities = GetRelatedIdentities(channel);
            var channelTableName = _channelRepository.TableName;
            return await GetTableStylesAsync(channelTableName, relatedIdentities);
        }

        public async Task<List<TableStyle>> GetContentStylesAsync(Site site, Channel channel)
        {
            var tableName = _channelRepository.GetTableName(site, channel);
            var relatedIdentities = GetRelatedIdentities(channel);
            return await GetTableStylesAsync(tableName, relatedIdentities);
        }

        public async Task<List<TableStyle>> GetUserStylesAsync()
        {
            var relatedIdentities = EmptyRelatedIdentities;
            var userTableName = _userRepository.TableName;
            var styles = await GetTableStylesAsync(userTableName, relatedIdentities);
            return styles.Where(x =>
                !StringUtils.EqualsIgnoreCase(x.AttributeName, nameof(User.Mobile)) &&
                !StringUtils.EqualsIgnoreCase(x.AttributeName, nameof(User.Email))).ToList();
        }

        //relatedIdentities从大到小，最后是0
        public async Task<TableStyle> GetTableStyleAsync(string tableName, string attributeName, List<int> relatedIdentities)
        {
            if (attributeName == null) attributeName = string.Empty;

            relatedIdentities = ParseRelatedIdentities(relatedIdentities);
            var styles = await GetAllAsync(tableName);
            foreach (var relatedIdentity in relatedIdentities)
            {
                var style = styles.FirstOrDefault(x => x.RelatedIdentity == relatedIdentity && x.TableName == tableName && x.AttributeName == attributeName);
                if (style == null) continue;

                return style;
            }

            var userTableName = _userRepository.TableName;
            var channelTableName = _channelRepository.TableName;
            var siteTableName = _siteRepository.TableName;

            if (tableName == userTableName || tableName == channelTableName || tableName == siteTableName)
            {
                if (tableName == userTableName)
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
                DisplayName = string.Empty,
                HelpText = string.Empty,
                List = false,
                InputType = InputType.Text,
                DefaultValue = string.Empty,
                Horizontal = true
            };
        }

        public async Task<bool> IsExistsAsync(int relatedIdentity, string tableName, string attributeName)
        {
            var styles = await GetAllAsync(tableName);
            return styles.Any(x => x.RelatedIdentity == relatedIdentity && x.AttributeName == attributeName);
        }

        public async Task<Dictionary<string, List<TableStyle>>> GetTableStyleWithItemsDictionaryAsync(string tableName, List<int> allRelatedIdentities)
        {
            var dict = new Dictionary<string, List<TableStyle>>();

            var styles = await GetAllAsync(tableName);
            foreach (var style in styles)
            {
                if (!StringUtils.EqualsIgnoreCase(style.TableName, tableName) ||
                    !allRelatedIdentities.Contains(style.RelatedIdentity)) continue;

                var tableStyleWithItemList = dict.ContainsKey(style.AttributeName) ? dict[style.AttributeName] : new List<TableStyle>();
                tableStyleWithItemList.Add(style);
                dict[style.AttributeName] = tableStyleWithItemList;
            }

            return dict;
        }

        public List<int> GetRelatedIdentities(int relatedIdentity)
        {
            return relatedIdentity == 0 ? EmptyRelatedIdentities : new List<int> {relatedIdentity, 0};
        }

        public List<int> GetRelatedIdentities(Channel channel)
        {
            var list = new List<int>();
            if (channel != null)
            {
                if (channel.ParentsCount > 0)
                {
                    list.Add(channel.Id);
                    if (channel.ParentsPath != null)
                    {
                        list.AddRange(channel.ParentsPath);
                    }
                    list.Add(0);
                }
                else
                {
                    list.Add(channel.Id);
                    list.Add(0);
                }
            }
            else
            {
                list.Add(0);
            }
            return list;
        }

        public List<int> EmptyRelatedIdentities => new List<int> { 0 };

        private async Task<int> GetMaxTaxisAsync(string tableName, List<int> relatedIdentities)
        {
            var list = await GetTableStylesAsync(tableName, relatedIdentities);
            if (list != null && list.Count > 0)
            {
                return list.Max(x => x.Taxis);
            }

            return 0;
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

        private TableStyle GetDefaultContentTableStyle(string tableName, string attributeName)
        {
            var style = new TableStyle
            {
                Id = 0,
                RelatedIdentity = 0,
                TableName = tableName,
                AttributeName = attributeName,
                Taxis = 0,
                DisplayName = string.Empty,
                HelpText = string.Empty,
                List = false,
                InputType = InputType.Text,
                DefaultValue = string.Empty,
                Horizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Title)))
            {
                style.AttributeName = nameof(Content.Title);
                style.DisplayName = "标题";
                style.RuleValues = TranslateUtils.JsonSerialize(new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Required,
                        Message = "标题为必填项"
                    }
                });
                style.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.SubTitle)))
            {
                style.AttributeName = nameof(Content.SubTitle);
                style.DisplayName = "副标题";
                style.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.ImageUrl)))
            {
                style.AttributeName = nameof(Content.ImageUrl);
                style.DisplayName = "图片";
                style.InputType = InputType.Image;
                style.Taxis = 3;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.VideoUrl)))
            {
                style.AttributeName = nameof(Content.VideoUrl);
                style.DisplayName = "视频";
                style.InputType = InputType.Video;
                style.Taxis = 4;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.FileUrl)))
            {
                style.AttributeName = nameof(Content.FileUrl);
                style.DisplayName = "附件";
                style.InputType = InputType.File;
                style.Taxis = 5;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Body)))
            {
                style.AttributeName = nameof(Content.Body);
                style.DisplayName = "内容";
                style.InputType = InputType.TextEditor;
                style.Taxis = 6;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Summary)))
            {
                style.AttributeName = nameof(Content.Summary);
                style.DisplayName = "内容摘要";
                style.InputType = InputType.TextArea;
                style.Taxis = 7;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Author)))
            {
                style.AttributeName = nameof(Content.Author);
                style.DisplayName = "作者";
                style.Taxis = 8;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(Content.Source)))
            {
                style.AttributeName = nameof(Content.Source);
                style.DisplayName = "来源";
                style.Taxis = 9;
            }

            style.Items = TranslateUtils.JsonDeserialize<List<InputStyleItem>>(style.ItemValues);
            style.Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues);

            return style;
        }

        private TableStyle GetDefaultUserTableStyle(string tableName, string attributeName)
        {
            var style = new TableStyle
            {
                Id = 0,
                RelatedIdentity = 0,
                TableName = tableName,
                AttributeName = attributeName,
                Taxis = 0,
                DisplayName = string.Empty,
                HelpText = string.Empty,
                List = false,
                InputType = InputType.Text,
                DefaultValue = string.Empty,
                Horizontal = true
            };

            if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.DisplayName)))
            {
                style.AttributeName = nameof(User.DisplayName);
                style.DisplayName = "姓名";
                style.RuleValues = TranslateUtils.JsonSerialize(new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Required,
                        Message = ValidateType.Required.GetDisplayName()
                    }
                });
                style.Taxis = 1;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Mobile)))
            {
                style.AttributeName = nameof(User.Mobile);
                style.DisplayName = "手机号";
                style.HelpText = "可用于登录、找回密码等功能";
                style.InputType = InputType.Number;
                style.RuleValues = TranslateUtils.JsonSerialize(new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Mobile,
                        Message = ValidateType.Mobile.GetDisplayName()
                    }
                });
                style.Taxis = 2;
            }
            else if (StringUtils.EqualsIgnoreCase(attributeName, nameof(User.Email)))
            {
                style.AttributeName = nameof(User.Email);
                style.DisplayName = "邮箱";
                style.HelpText = "可用于登录、找回密码等功能";
                style.RuleValues = TranslateUtils.JsonSerialize(new List<InputStyleRule>
                {
                    new InputStyleRule
                    {
                        Type = ValidateType.Email,
                        Message = ValidateType.Email.GetDisplayName()
                    }
                });
                style.Taxis = 3;
            }

            style.Items = TranslateUtils.JsonDeserialize<List<InputStyleItem>>(style.ItemValues);
            style.Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues);

            return style;
        }
    }
}
