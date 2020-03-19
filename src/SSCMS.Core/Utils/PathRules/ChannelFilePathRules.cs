using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.PathRules
{
    public class ChannelFilePathRules
    {
        private const string ChannelId = "{@channelId}";
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

        public static string DefaultRule = "/channels/{@channelId}.html";
        public static string DefaultDirectoryName = "/channels/";
        public static string DefaultRegexString = "/channels/(?<channelId>[^_]*)_?(?<pageIndex>[^_]*)";

        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;

        public ChannelFilePathRules(IPathManager pathManager, IDatabaseManager databaseManager)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
        }

        public async Task<Dictionary<string, string>> GetDictionaryAsync(int channelId)
        {
            var dictionary = new Dictionary<string, string>
                {
                    {ChannelId, "栏目ID"},
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

            var channelInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
            var styleInfoList = await _databaseManager.TableStyleRepository.GetChannelStyleListAsync(channelInfo);
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

        public async Task<string> ParseAsync(Site site, int channelId)
        {
            var channelFilePathRule = await _pathManager.GetChannelFilePathRuleAsync(site, channelId);
            var filePath = await ParseChannelPathAsync(site, channelId, channelFilePathRule);
            return filePath;
        }

        //递归处理
        private async Task<string> ParseChannelPathAsync(Site site, int channelId, string channelFilePathRule)
        {
            var filePath = channelFilePathRule.Trim();
            const string regex = "(?<element>{@[^}]+})";
            var elements = RegexUtils.GetContents("element", regex, filePath);
            Channel node = null;

            foreach (var element in elements)
            {
                var value = string.Empty;

                if (StringUtils.EqualsIgnoreCase(element, ChannelId))
                {
                    value = channelId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, Year))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (node.AddDate.HasValue)
                    {
                        value = node.AddDate.Value.Year.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Month))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (node.AddDate.HasValue)
                    {
                        value = node.AddDate.Value.Month.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Day))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (node.AddDate.HasValue)
                    {
                        value = node.AddDate.Value.Day.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Hour))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (node.AddDate.HasValue)
                    {
                        value = node.AddDate.Value.Hour.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Minute))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (node.AddDate.HasValue)
                    {
                        value = node.AddDate.Value.Minute.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Second))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    if (node.AddDate.HasValue)
                    {
                        value = node.AddDate.Value.Second.ToString();
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, Sequence))
                {
                    value = (await _databaseManager.ChannelRepository.GetSequenceAsync(site.Id, channelId)).ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(element, ParentRule))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    var parentInfo = await _databaseManager.ChannelRepository.GetAsync(node.ParentId);
                    if (parentInfo != null)
                    {
                        var parentRule = await _pathManager.GetChannelFilePathRuleAsync(site, parentInfo.Id);
                        value = DirectoryUtils.GetDirectoryPath(await ParseChannelPathAsync(site, parentInfo.Id, parentRule)).Replace("\\", "/");
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(element, ChannelName))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    value = node.ChannelName;
                }
                else if (StringUtils.EqualsIgnoreCase(element, LowerChannelName))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    value = node.ChannelName.ToLower();
                }
                else if (StringUtils.EqualsIgnoreCase(element, LowerChannelIndex))
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    value = node.IndexName.ToLower();
                }
                else
                {
                    if (node == null) node = await _databaseManager.ChannelRepository.GetAsync(channelId);
                    var attributeName = element.Replace("{@", string.Empty).Replace("}", string.Empty);

                    var isLower = false;
                    if (StringUtils.StartsWithIgnoreCase(attributeName, "lower"))
                    {
                        isLower = true;
                        attributeName = attributeName.Substring(5);
                    }

                    value = node.Get<string>(attributeName);

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
