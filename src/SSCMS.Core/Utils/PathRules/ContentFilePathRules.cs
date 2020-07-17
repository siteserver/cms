using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.PathRules
{
    public class ContentFilePathRules
    {
        private const string ChannelId = "{@channelId}";
        private const string ContentId = "{@contentId}";
        private const string Year = "{@year}";
        private const string Month = "{@month}";
        private const string Day = "{@day}";
        private const string Hour = "{@hour}";
        private const string Minute = "{@minute}";
        private const string Second = "{@second}";
        private const string Sequence = "{@sequence}";
        private const string ParentRule = "{@parentRule}";
        private const string ChannelName = "{@channelName}";
        private const string LowerChannelName = "{@lowerChannelName}";
        private const string ChannelIndex = "{@channelIndex}";
        private const string LowerChannelIndex = "{@lowerChannelIndex}";

        public const string DefaultRule = "/contents/{@channelId}/{@contentId}.html";
        public const string DefaultDirectoryName = "/contents/";
        public const string DefaultRegexString = "/contents/(?<channelId>[^/]*)/(?<contentId>[^/]*)_?(?<pageIndex>[^_]*)";

        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;

        public ContentFilePathRules(IPathManager pathManager, IDatabaseManager databaseManager)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
        }

        public async Task<Dictionary<string, string>> GetDictionaryAsync(Site site, int channelId)
        {
            var dictionary = new Dictionary<string, string>
                {
                    {ChannelId, "栏目ID"},
                    {ContentId, "内容ID"},
                    {Year, "年份"},
                    {Month, "月份"},
                    {Day, "日期"},
                    {Hour, "小时"},
                    {Minute, "分钟"},
                    {Second, "秒钟"},
                    {Sequence, "顺序数"},
                    {ParentRule, "父级命名规则"},
                    {ChannelName, "栏目名称"},
                    {LowerChannelName, "栏目名称(小写)"},
                    {ChannelIndex, "栏目索引"},
                    {LowerChannelIndex, "栏目索引(小写)"}
                };

            var channel = await _databaseManager.ChannelRepository.GetAsync(channelId);
            var tableName = _databaseManager.ChannelRepository.GetTableName(site, channel);
            var styleInfoList = await _databaseManager.TableStyleRepository.GetContentStylesAsync(channel, tableName);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.InputType == InputType.Text)
                {
                    dictionary.Add($@"{{@{StringUtils.LowerFirst(styleInfo.AttributeName)}}}", styleInfo.DisplayName);
                    dictionary.Add($@"{{@lower{styleInfo.AttributeName}}}", styleInfo.DisplayName + "(小写)");
                }
            }

            return dictionary;
        }

        public async Task<string> ParseAsync(Site site, int channelId, int contentId)
        {
            var contentFilePathRule = await _pathManager.GetContentFilePathRuleAsync(site, channelId);
            var contentInfo = await _databaseManager.ContentRepository.GetAsync(site, channelId, contentId);
            var filePath = await ParseContentPathAsync(site, channelId, contentInfo, contentFilePathRule);
            return filePath;
        }

        public async Task<string> ParseAsync(Site site, int channelId, Content content)
        {
            var contentFilePathRule = await _pathManager.GetContentFilePathRuleAsync(site, channelId);
            var filePath = await ParseContentPathAsync(site, channelId, content, contentFilePathRule);
            return filePath;
        }

        private async Task<string> ParseContentPathAsync(Site site, int channelId, Content content, string contentFilePathRule)
        {
            var filePath = contentFilePathRule.Trim();
            var regex = "(?<element>{@[^}]+})";
            var elements = RegexUtils.GetContents("element", regex, filePath);
            var addDate = content.AddDate;
            var contentId = content.Id;
            foreach (var element in elements)
            {
                var value = string.Empty;

                if (StringUtils.EqualsIgnoreCase(element, ChannelId))
                {
                    value = channelId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentId))
                {
                    value = contentId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, Sequence))
                {
                    var tableName = await _databaseManager.ChannelRepository.GetTableNameAsync(site, channelId);
                    value = Convert.ToString(await _databaseManager.ContentRepository.GetSequenceAsync(tableName, site.Id, channelId, contentId));
                }
                else if (StringUtils.EqualsIgnoreCase(element, ParentRule))//继承父级设置 20151113 sessionliang
                {
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    var parentInfo = await _databaseManager.ChannelRepository.GetAsync(nodeInfo.ParentId);
                    if (parentInfo != null)
                    {
                        var parentRule = await _pathManager.GetContentFilePathRuleAsync(site, parentInfo.Id);
                        value = DirectoryUtils.GetDirectoryPath(await ParseContentPathAsync(site, parentInfo.Id, content, parentRule)).Replace("\\", "/");
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelName))
                {
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (nodeInfo != null)
                    {
                        value = nodeInfo.ChannelName;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, LowerChannelName))
                {
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (nodeInfo != null)
                    {
                        value = StringUtils.ToLower(nodeInfo.ChannelName);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelIndex))
                {
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (nodeInfo != null)
                    {
                        value = nodeInfo.IndexName;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, LowerChannelIndex))
                {
                    var nodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (nodeInfo != null)
                    {
                        value = StringUtils.ToLower(nodeInfo.IndexName);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Year) || StringUtils.EqualsIgnoreCase(element, Month) || StringUtils.EqualsIgnoreCase(element, Day) || StringUtils.EqualsIgnoreCase(element, Hour) || StringUtils.EqualsIgnoreCase(element, Minute) || StringUtils.EqualsIgnoreCase(element, Second))
                {
                    if (StringUtils.EqualsIgnoreCase(element, Year))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Year.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Month))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Month.ToString("D2");
                        }

                        //value = addDate.ToString("MM");
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Day))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Day.ToString("D2");
                        }

                        //value = addDate.ToString("dd");
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Hour))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Hour.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Minute))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Minute.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Second))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Second.ToString();
                        }
                    }
                }
                else
                {
                    var attributeName = element.Replace("{@", string.Empty).Replace("}", string.Empty);

                    var isLower = false;
                    if (StringUtils.StartsWithIgnoreCase(attributeName, "lower"))
                    {
                        isLower = true;
                        attributeName = attributeName.Substring(5);
                    }

                    value = content.Get<string>(attributeName);
                    if (isLower)
                    {
                        value = StringUtils.ToLower(value);
                    }
                }

                value = StringUtils.HtmlDecode(value);

                filePath = filePath.Replace(element, value);
            }

            if (filePath.Contains("//"))
            {
                filePath = Regex.Replace(filePath, @"(/)\1{2,}", "/");
                filePath = filePath.Replace("//", "/");
            }

            if (filePath.Contains("("))
            {
                regex = @"(?<element>\([^\)]+\))";
                elements = RegexUtils.GetContents("element", regex, filePath);
                foreach (var element in elements)
                {
                    if (!element.Contains("|")) continue;

                    var value = element.Replace("(", string.Empty).Replace(")", string.Empty);
                    var value1 = value.Split('|')[0];
                    var value2 = value.Split('|')[1];
                    value = value1 + value2;

                    if (!string.IsNullOrEmpty(value1) && !string.IsNullOrEmpty(value1))
                    {
                        value = value1;
                    }

                    filePath = filePath.Replace(element, value);
                }
            }
            return filePath;
        }
    }
}
