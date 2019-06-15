using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Stl;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PathManager
    {
        private const string ContentRulesChannelId = "{@channelId}";
        private const string ContentRulesContentId = "{@contentId}";
        private const string ContentRulesYear = "{@year}";
        private const string ContentRulesMonth = "{@month}";
        private const string ContentRulesDay = "{@day}";
        private const string ContentRulesHour = "{@hour}";
        private const string ContentRulesMinute = "{@minute}";
        private const string ContentRulesSecond = "{@second}";
        private const string ContentRulesSequence = "{@sequence}";
        private const string ContentRulesParentRule = "{@parentRule}";
        private const string ContentRulesChannelName = "{@channelName}";
        private const string ContentRulesLowerChannelName = "{@lowerChannelName}";
        private const string ContentRulesChannelIndex = "{@channelIndex}";
        private const string ContentRulesLowerChannelIndex = "{@lowerChannelIndex}";

        public const string ContentRulesDefaultRule = "/contents/{@channelId}/{@contentId}.html";
        public const string ContentRulesDefaultDirectoryName = "/contents/";
        public const string ContentRulesDefaultRegexString = "/contents/(?<channelId>[^/]*)/(?<contentId>[^/]*)_?(?<pageIndex>[^_]*)";

        public IDictionary ContentRulesGetDictionary(IPluginManager pluginManager, ITableStyleRepository tableStyleRepository, SiteInfo siteInfo, int channelId)
        {
            var dictionary = new ListDictionary
                {
                    {ContentRulesChannelId, "栏目ID"},
                    {ContentRulesContentId, "内容ID"},
                    {ContentRulesYear, "年份"},
                    {ContentRulesMonth, "月份"},
                    {ContentRulesDay, "日期"},
                    {ContentRulesHour, "小时"},
                    {ContentRulesMinute, "分钟"},
                    {ContentRulesSecond, "秒钟"},
                    {ContentRulesSequence, "顺序数"},
                    {ContentRulesParentRule, "父级命名规则"},
                    {ContentRulesChannelName, "栏目名称"},
                    {ContentRulesLowerChannelName, "栏目名称(小写)"},
                    {ContentRulesChannelIndex, "栏目索引"},
                    {ContentRulesLowerChannelIndex, "栏目索引(小写)"}
                };

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            var styleInfoList = tableStyleRepository.GetContentStyleInfoList(pluginManager, siteInfo, channelInfo);
            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.Type == InputType.Text)
                {
                    dictionary.Add($@"{{@{StringUtils.LowerFirst(styleInfo.AttributeName)}}}", styleInfo.DisplayName);
                    dictionary.Add($@"{{@lower{styleInfo.AttributeName}}}", styleInfo.DisplayName + "(小写)");
                }
            }

            return dictionary;
        }

        public string ContentRulesParse(SiteInfo siteInfo, int channelId, int contentId)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            var contentFilePathRule = GetContentFilePathRule(siteInfo, channelId);
            var contentInfo = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);
            var filePath = ContentRulesParseContentPath(siteInfo, channelId, contentInfo, contentFilePathRule);
            return filePath;
        }

        public string ContentRulesParse(SiteInfo siteInfo, int channelId, ContentInfo contentInfo)
        {
            var contentFilePathRule = GetContentFilePathRule(siteInfo, channelId);
            var filePath = ContentRulesParseContentPath(siteInfo, channelId, contentInfo, contentFilePathRule);
            return filePath;
        }

        private string ContentRulesParseContentPath(SiteInfo siteInfo, int channelId, ContentInfo contentInfo, string contentFilePathRule)
        {
            var filePath = contentFilePathRule.Trim();
            var regex = "(?<element>{@[^}]+})";
            var elements = RegexUtils.GetContents("element", regex, filePath);
            var addDate = contentInfo.AddDate;
            var contentId = contentInfo.Id;
            foreach (var element in elements)
            {
                var value = string.Empty;

                if (StringUtils.EqualsIgnoreCase(element, ContentRulesChannelId))
                {
                    value = channelId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesContentId))
                {
                    value = contentId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesSequence))
                {
                    var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    value = channelInfo.ContentRepository.StlGetSequence(channelInfo, contentId).ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesParentRule))//继承父级设置 20151113 sessionliang
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    var parentInfo = ChannelManager.GetChannelInfo(siteInfo.Id, nodeInfo.ParentId);
                    if (parentInfo != null)
                    {
                        var parentRule = GetContentFilePathRule(siteInfo, parentInfo.Id);
                        value = DirectoryUtils.GetDirectoryPath(ContentRulesParseContentPath(siteInfo, parentInfo.Id, contentInfo, parentRule)).Replace("\\", "/");
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesChannelName))
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    if (nodeInfo != null)
                    {
                        value = nodeInfo.ChannelName;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesLowerChannelName))
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    if (nodeInfo != null)
                    {
                        value = nodeInfo.ChannelName.ToLower();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesChannelIndex))
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    if (nodeInfo != null)
                    {
                        value = nodeInfo.IndexName;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesLowerChannelIndex))
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    if (nodeInfo != null)
                    {
                        value = nodeInfo.IndexName.ToLower();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ContentRulesYear) || StringUtils.EqualsIgnoreCase(element, ContentRulesMonth) || StringUtils.EqualsIgnoreCase(element, ContentRulesDay) || StringUtils.EqualsIgnoreCase(element, ContentRulesHour) || StringUtils.EqualsIgnoreCase(element, ContentRulesMinute) || StringUtils.EqualsIgnoreCase(element, ContentRulesSecond))
                {
                    if (StringUtils.EqualsIgnoreCase(element, ContentRulesYear))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Year.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentRulesMonth))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Month.ToString("D2");
                        }

                        //value = addDate.ToString("MM");
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentRulesDay))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Day.ToString("D2");
                        }

                        //value = addDate.ToString("dd");
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentRulesHour))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Hour.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentRulesMinute))
                    {
                        if (addDate.HasValue)
                        {
                            value = addDate.Value.Minute.ToString();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentRulesSecond))
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

                    value = contentInfo.Get<string>(attributeName);
                    if (isLower)
                    {
                        value = value.ToLower();
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