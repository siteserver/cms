using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PathManager
    {
        private const string ChannelRulesChannelId = "{@channelId}";
        private const string ChannelRulesYear = "{@year}";
        private const string ChannelRulesMonth = "{@month}";
        private const string ChannelRulesDay = "{@day}";
        private const string ChannelRulesHour = "{@hour}";
        private const string ChannelRulesMinute = "{@minute}";
        private const string ChannelRulesSecond = "{@second}";
        private const string ChannelRulesSequence = "{@sequence}";
        private const string ChannelRulesParentRule = "{@parentRule}";
        private const string ChannelRulesChannelName = "{@channelName}";
        private const string ChannelRulesLowerChannelName = "{@lowerChannelName}";
        private const string ChannelRulesChannelIndex = "{@channelIndex}";
        private const string ChannelRulesLowerChannelIndex = "{@lowerChannelIndex}";

        public const string ChannelRulesDefaultRule = "/channels/{@channelId}.html";
        public const string ChannelRulesDefaultDirectoryName = "/channels/";
        public const string ChannelRulesDefaultRegexString = "/channels/(?<channelId>[^_]*)_?(?<pageIndex>[^_]*)";

        public async Task<IDictionary> ChannelRulesGetDictionaryAsync(Site siteInfo, int channelId)
        {
            var dictionary = new ListDictionary
                {
                    {ChannelRulesChannelId, "栏目ID"},
                    {ChannelRulesYear, "年份"},
                    {ChannelRulesMonth, "月份"},
                    {ChannelRulesDay, "日期"},
                    {ChannelRulesHour, "小时"},
                    {ChannelRulesMinute, "分钟"},
                    {ChannelRulesSecond, "秒钟"},
                    {ChannelRulesSequence, "顺序数"},
                    {ChannelRulesParentRule, "父级命名规则"},
                    {ChannelRulesChannelName, "栏目名称"},
                    {ChannelRulesLowerChannelName, "栏目名称(小写)"},
                    {ChannelRulesChannelIndex, "栏目索引"},
                    {ChannelRulesLowerChannelIndex, "栏目索引(小写)"}
                };

            var channelInfo = await _channelRepository.GetChannelAsync(channelId);
            var styleInfoList = await _tableStyleRepository.GetChannelStyleInfoListAsync(channelInfo);
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

        public async Task<string> ChannelRulesParseAsync(Site siteInfo, int channelId)
        {
            var channelFilePathRule = await GetChannelFilePathRuleAsync(siteInfo, channelId);
            var filePath = await ChannelRulesParseChannelPathAsync(siteInfo, channelId, channelFilePathRule);
            return filePath;
        }

        //递归处理
        private async Task<string> ChannelRulesParseChannelPathAsync(Site siteInfo, int channelId, string channelFilePathRule)
        {
            var filePath = channelFilePathRule.Trim();
            const string regex = "(?<element>{@[^}]+})";
            var elements = RegexUtils.GetContents("element", regex, filePath);
            Channel channelInfo = null;

            foreach (var element in elements)
            {
                var value = string.Empty;

                if (StringUtils.EqualsIgnoreCase(element, ChannelRulesChannelId))
                {
                    value = channelId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesYear))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    if (channelInfo.CreatedDate.HasValue)
                    {
                        value = channelInfo.CreatedDate.Value.Year.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesMonth))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    if (channelInfo.CreatedDate.HasValue)
                    {
                        value = channelInfo.CreatedDate.Value.Month.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesDay))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    if (channelInfo.CreatedDate.HasValue)
                    {
                        value = channelInfo.CreatedDate.Value.Day.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesHour))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    if (channelInfo.CreatedDate.HasValue)
                    {
                        value = channelInfo.CreatedDate.Value.Hour.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesMinute))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    if (channelInfo.CreatedDate.HasValue)
                    {
                        value = channelInfo.CreatedDate.Value.Minute.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesSecond))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    if (channelInfo.CreatedDate.HasValue)
                    {
                        value = channelInfo.CreatedDate.Value.Second.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesSequence))
                {
                    var sequence = await _channelRepository.GetSequenceAsync(siteInfo.Id, channelId);
                    value = sequence.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesParentRule))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    var parentInfo = await _channelRepository.GetChannelAsync(channelInfo.ParentId);
                    if (parentInfo != null)
                    {
                        var parentRule = await GetChannelFilePathRuleAsync(siteInfo, parentInfo.Id);
                        value = DirectoryUtils.GetDirectoryPath(await ChannelRulesParseChannelPathAsync(siteInfo, parentInfo.Id, parentRule)).Replace("\\", "/");
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesChannelName))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    value = channelInfo.ChannelName;
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesLowerChannelName))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    value = channelInfo.ChannelName.ToLower();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelRulesLowerChannelIndex))
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    value = channelInfo.IndexName.ToLower();
                }
                else
                {
                    if (channelInfo == null) channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    var attributeName = element.Replace("{@", string.Empty).Replace("}", string.Empty);

                    var isLower = false;
                    if (StringUtils.StartsWithIgnoreCase(attributeName, "lower"))
                    {
                        isLower = true;
                        attributeName = attributeName.Substring(5);
                    }

                    value = channelInfo.Get<string>(attributeName);

                    if (isLower)
                    {
                        value = value.ToLower();
                    }
                }

                filePath = filePath.Replace(element, value);
            }

            if (!filePath.Contains("//")) return filePath;

            filePath = Regex.Replace(filePath, @"(/)\1{2,}", "/");
            filePath = Regex.Replace(filePath, @"//", "/");
            return filePath;
        }
    }
}