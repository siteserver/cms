using System;
using System.Text;
using SiteServer.Utils;
using SiteServer.Utils.IO;
using SiteServer.Utils.Rss;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "Rss订阅", Description = "通过 stl:rss 标签在模板中生成Rss阅读器能够浏览的Rss订阅")]
    public class StlRss
    {
        private StlRss() { }
        public const string ElementName = "stl:rss";

        private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
        private static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
        private static readonly Attr Scope = new Attr("scope", "内容范围");
        private static readonly Attr GroupChannel = new Attr("groupChannel", "指定显示的栏目组");
        private static readonly Attr GroupChannelNot = new Attr("groupChannelNot", "指定不显示的栏目组");
        private static readonly Attr GroupContent = new Attr("groupContent", "指定显示的内容组");
        private static readonly Attr GroupContentNot = new Attr("groupContentNot", "指定不显示的内容组");
        private static readonly Attr Tags = new Attr("tags", "指定标签");
        private static readonly Attr Title = new Attr("title", "Rss订阅标题");
        private static readonly Attr Description = new Attr("description", "Rss订阅摘要");
        private static readonly Attr TotalNum = new Attr("totalNum", "显示内容数目");
        private static readonly Attr StartNum = new Attr("startNum", "从第几条信息开始显示");
        private static readonly Attr Order = new Attr("order", "排序");
        private static readonly Attr IsTop = new Attr("isTop", "仅显示置顶内容");
        private static readonly Attr IsRecommend = new Attr("isRecommend", "仅显示推荐内容");
        private static readonly Attr IsHot = new Attr("isHot", "仅显示热点内容");
        private static readonly Attr IsColor = new Attr("isColor", "仅显示醒目内容");

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

                if (StringUtils.EqualsIgnoreCase(name, Title.Name))
                {
                    title = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Description.Name))
                {
                    description = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Scope.Name))
                {
                    scopeTypeString = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannel.Name))
                {
                    groupChannel = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupChannelNot.Name))
                {
                    groupChannelNot = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContent.Name))
                {
                    groupContent = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, GroupContentNot.Name))
                {
                    groupContentNot = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Tags.Name))
                {
                    tags = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, TotalNum.Name))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StartNum.Name))
                {
                    startNum = TranslateUtils.ToInt(value, 1);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Order.Name))
                {
                    orderByString = StlDataUtility.GetContentOrderByString(pageInfo.SiteId, value, ETaxisType.OrderByTaxisDesc);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsTop.Name))
                {
                    isTopExists = true;
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsRecommend.Name))
                {
                    isRecommendExists = true;
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHot.Name))
                {
                    isHotExists = true;
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsColor.Name))
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

            var dataSource = StlDataUtility.GetContentsDataSource(pageInfo.SiteInfo, channelId, 0, groupContent, groupContentNot, tags, false, false, false, false, false, false, false, startNum, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, string.Empty, scopeType, groupChannel, groupChannelNot, null);

            if (dataSource != null)
            {
                //foreach (var dataItem in dataSource)
                //{
                //    var item = new RssItem();

                //    var contentInfo = new BackgroundContentInfo(dataItem);
                //    item.Title = StringUtils.Replace("&", contentInfo.Title, "&amp;");
                //    item.Description = contentInfo.Summary;
                //    if (string.IsNullOrEmpty(item.Description))
                //    {
                //        item.Description = StringUtils.StripTags(contentInfo.Content);
                //        item.Description = string.IsNullOrEmpty(item.Description) ? contentInfo.Title : StringUtils.MaxLengthText(item.Description, 200);
                //    }
                //    item.Description = StringUtils.Replace("&", item.Description, "&amp;");
                //    item.PubDate = contentInfo.AddDate;
                //    item.Link = new Uri(PageUtils.AddProtocolToUrl(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo)));

                //    channel.Items.Add(item);
                //}
            }

            feed.Channels.Add(channel);

            var builder = new StringBuilder();
            var textWriter = new EncodedStringWriter(builder, ECharsetUtils.GetEncoding(pageInfo.TemplateInfo.Charset));
            feed.Write(textWriter);

            return builder.ToString();
        }
    }
}
