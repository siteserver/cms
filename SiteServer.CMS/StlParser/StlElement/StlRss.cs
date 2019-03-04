using System;
using System.Text;
using SiteServer.Utils;
using SiteServer.Utils.IO;
using SiteServer.Utils.Rss;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;
using SiteServer.CMS.DataCache.Content;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "Rss订阅", Description = "通过 stl:rss 标签在模板中生成Rss阅读器能够浏览的Rss订阅")]
    public class StlRss
    {
        private StlRss() { }
        public const string ElementName = "stl:rss";

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "内容范围")]
        private const string Scope = nameof(Scope);

        [StlAttribute(Title = "指定显示的栏目组")]
        private const string GroupChannel = nameof(GroupChannel);

        [StlAttribute(Title = "指定不显示的栏目组")]
        private const string GroupChannelNot = nameof(GroupChannelNot);

        [StlAttribute(Title = "指定显示的内容组")]
        private const string GroupContent = nameof(GroupContent);

        [StlAttribute(Title = "指定不显示的内容组")]
        private const string GroupContentNot = nameof(GroupContentNot);

        [StlAttribute(Title = "指定标签")]
        private const string Tags = nameof(Tags);

        [StlAttribute(Title = "Rss订阅标题")]
        private const string Title = nameof(Title);

        [StlAttribute(Title = "Rss订阅摘要")]
        private const string Description = nameof(Description);

        [StlAttribute(Title = "显示内容数目")]
        private const string TotalNum = nameof(TotalNum);

        [StlAttribute(Title = "从第几条信息开始显示")]
        private const string StartNum = nameof(StartNum);

        [StlAttribute(Title = "排序")]
        private const string Order = nameof(Order);

        [StlAttribute(Title = "仅显示置顶内容")]
        private const string IsTop = nameof(IsTop);

        [StlAttribute(Title = "仅显示推荐内容")]
        private const string IsRecommend = nameof(IsRecommend);

        [StlAttribute(Title = "仅显示热点内容")]
        private const string IsHot = nameof(IsHot);

        [StlAttribute(Title = "仅显示醒目内容")]
        private const string IsColor = nameof(IsColor);


        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var title = string.Empty;
            var description = string.Empty;
            var scopeTypeString = string.Empty;
            var groupChannel = string.Empty;
            var groupChannelNot = string.Empty;
            var groupContent = string.Empty;
            var groupContentNot = string.Empty;
            var tags = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var totalNum = 0;
            var startNum = 1;
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var isTop = false;
            var isTopExists = false;
            var isRecommend = false;
            var isRecommendExists = false;
            var isHot = false;
            var isHotExists = false;
            var isColor = false;
            var isColorExists = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Title))
                {
                    title = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Description))
                {
                    description = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope))
                {
                    scopeTypeString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannel))
                {
                    groupChannel = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannelNot))
                {
                    groupChannelNot = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContent))
                {
                    groupContent = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContentNot))
                {
                    groupContentNot = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Tags))
                {
                    tags = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartNum))
                {
                    startNum = TranslateUtils.ToInt(value, 1);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Order))
                {
                    orderByString = StlDataUtility.GetContentOrderByString(pageInfo.SiteId, value, ETaxisType.OrderByTaxisDesc);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsTop))
                {
                    isTopExists = true;
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsRecommend))
                {
                    isRecommendExists = true;
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHot))
                {
                    isHotExists = true;
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsColor))
                {
                    isColorExists = true;
                    isColor = TranslateUtils.ToBool(value);
                }
            }

            return ParseImpl(pageInfo, contextInfo, title, description, scopeTypeString, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, channelIndex, channelName, totalNum, startNum, orderByString, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string title, string description, string scopeTypeString, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string channelIndex, string channelName, int totalNum, int startNum, string orderByString, bool isTop, bool isTopExists, bool isRecommend, bool isRecommendExists, bool isHot, bool isHotExists, bool isColor, bool isColorExists)
        {
            var feed = new RssFeed
            {
                Encoding = ECharsetUtils.GetEncoding(pageInfo.TemplateInfo.Charset),
                Version = RssVersion.RSS20
            };

            var channel = new RssChannel
            {
                Title = title,
                Description = description
            };

            var scopeType = !string.IsNullOrEmpty(scopeTypeString) ? EScopeTypeUtils.GetEnumType(scopeTypeString) : EScopeType.All;

            var channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, contextInfo.ChannelId, channelIndex, channelName);

            var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);
            if (string.IsNullOrEmpty(channel.Title))
            {
                channel.Title = nodeInfo.ChannelName;
            }
            if (string.IsNullOrEmpty(channel.Description))
            {
                channel.Description = nodeInfo.Content;
                channel.Description = string.IsNullOrEmpty(channel.Description) ? nodeInfo.ChannelName : StringUtils.MaxLengthText(channel.Description, 200);
            }
            channel.Link = new Uri(PageUtils.AddProtocolToUrl(PageUtility.GetChannelUrl(pageInfo.SiteInfo, nodeInfo, pageInfo.IsLocal)));

            var minContentInfoList = StlDataUtility.GetMinContentInfoList(pageInfo.SiteInfo, channelId, 0, groupContent, groupContentNot, tags, false, false, false, false, false, false, false, startNum, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, string.Empty, scopeType, groupChannel, groupChannelNot, null);

            if (minContentInfoList != null)
            {
                foreach (var minContentInfo in minContentInfoList)
                {
                    var item = new RssItem();

                    var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, minContentInfo.ChannelId, minContentInfo.Id);
                    item.Title = StringUtils.Replace("&", contentInfo.Title, "&amp;");
                    item.Description = contentInfo.Summary;
                    if (string.IsNullOrEmpty(item.Description))
                    {
                        item.Description = StringUtils.StripTags(contentInfo.Content);
                        item.Description = string.IsNullOrEmpty(item.Description) ? contentInfo.Title : StringUtils.MaxLengthText(item.Description, 200);
                    }
                    item.Description = StringUtils.Replace("&", item.Description, "&amp;");
                    item.PubDate = contentInfo.AddDate;
                    item.Link = new Uri(PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, false)));

                    channel.Items.Add(item);
                }
            }

            feed.Channels.Add(channel);

            var builder = new StringBuilder();
            var textWriter = new EncodedStringWriter(builder, ECharsetUtils.GetEncoding(pageInfo.TemplateInfo.Charset));
            feed.Write(textWriter);

            return builder.ToString();
        }
    }
}
