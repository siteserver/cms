using System.Collections.Specialized;
using System.Text;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "获取链接")]
    public static class StlA
    {
        public const string ElementName = "stl:a";

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "显示父栏目")]
        private const string Parent = nameof(Parent);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "链接地址")]
        private const string Href = nameof(Href);

        [StlAttribute(Title = "链接域名")]
        private const string Host = nameof(Host);

        [StlAttribute(Title = "链接参数")]
        private const string QueryString = nameof(QueryString);

        public static string Parse(ParseContext parseContext)
        {
            var attributes = new NameValueCollection();
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            const bool removeTarget = false;
            var href = string.Empty;
            var queryString = string.Empty;
            var host = string.Empty;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];
                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        parseContext.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        parseContext.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        parseContext.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        parseContext.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        parseContext.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseContext.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Href))
                {
                    href = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Host))
                {
                    host = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            var parsedContent = ParseImpl(parseContext, channelIndex, channelName, upLevel, topLevel,
                removeTarget, href, queryString, host, attributes);

            return parsedContent;
        }

        private static string ParseImpl(ParseContext parseContext, string channelIndex,
            string channelName, int upLevel, int topLevel, bool removeTarget, string href, string queryString,
            string host, NameValueCollection attributes)
        {
            string htmlId = attributes["id"];

            if (!string.IsNullOrEmpty(htmlId) && !string.IsNullOrEmpty(parseContext.ContainerClientId))
            {
                htmlId = parseContext.ContainerClientId + "_" + htmlId;
            }

            if (!string.IsNullOrEmpty(htmlId))
            {
                attributes["id"] = htmlId;
            }

            var innerHtml = string.Empty;

            var url = string.Empty;
            var onclick = string.Empty;
            if (!string.IsNullOrEmpty(href))
            {
                url = parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, href, parseContext.IsLocal);

                var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                parseContext.ParseInnerContent(innerBuilder);
                innerHtml = innerBuilder.ToString();
            }
            else
            {
                if (parseContext.ContextType == EContextType.Undefined)
                {
                    parseContext.ContextType = parseContext.ContentId != 0 ? EContextType.Content : EContextType.Channel;
                }

                if (parseContext.ContextType == EContextType.Content) //获取内容Url
                {
                    if (parseContext.ContentInfo != null)
                    {
                        url = parseContext.UrlManager.GetContentUrl(parseContext.SiteInfo, parseContext.ContentInfo, parseContext.IsLocal);
                    }
                    else
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);
                        url = parseContext.UrlManager.GetContentUrl(parseContext.SiteInfo, nodeInfo, parseContext.ContentId, parseContext.IsLocal);
                    }

                    if (string.IsNullOrEmpty(parseContext.InnerHtml))
                    {
                        var title = parseContext.ContentInfo?.Title;
                        title = ContentUtility.FormatTitle(
                            parseContext.ContentInfo?.TitleFormatString, title);

                        if (parseContext.SiteInfo.IsContentTitleBreakLine)
                        {
                            title = title.Replace("  ", string.Empty);
                        }

                        innerHtml = title;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                        parseContext.ParseInnerContent(innerBuilder);
                        innerHtml = innerBuilder.ToString();
                    }
                }
                else if (parseContext.ContextType == EContextType.Channel) //获取栏目Url
                {
                    parseContext.ChannelId =
                        StlDataUtility.GetChannelIdByLevel(parseContext.SiteId, parseContext.ChannelId, upLevel, topLevel);
                    parseContext.ChannelId =
                        ChannelManager.GetChannelId(parseContext.SiteId,
                            parseContext.ChannelId, channelIndex, channelName);
                    var channel = ChannelManager.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);

                    url = parseContext.UrlManager.GetChannelUrl(parseContext.SiteInfo, channel, parseContext.IsLocal);
                    if (string.IsNullOrWhiteSpace(parseContext.InnerHtml))
                    {
                        innerHtml = channel.ChannelName;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                        parseContext.ParseInnerContent(innerBuilder);
                        innerHtml = innerBuilder.ToString();
                    }
                }
            }

            if (url.Equals(PageUtils.UnclickedUrl))
            {
                removeTarget = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(host))
                {
                    url = parseContext.UrlManager.AddProtocolToUrl(url, host);
                }

                if (!string.IsNullOrEmpty(queryString))
                {
                    url = PageUtils.AddQueryString(url, queryString);
                }
            }

            attributes["href"] = url;

            if (!string.IsNullOrEmpty(onclick))
            {
                attributes["onclick"] = onclick;
            }

            if (removeTarget)
            {
                attributes["target"] = string.Empty;
            }

            // 如果是实体标签，则只返回url
            return parseContext.IsStlEntity
                ? url
                : $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
    }
}
