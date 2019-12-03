using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
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

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var attributes = new Dictionary<string, string>();
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            const bool removeTarget = false;
            var href = string.Empty;
            var queryString = string.Empty;
            var host = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];
                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = await StlEntityParser.ReplaceStlEntitiesForAttributeValueAsync(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await StlEntityParser.ReplaceStlEntitiesForAttributeValueAsync(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Href))
                {
                    href = await StlEntityParser.ReplaceStlEntitiesForAttributeValueAsync(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString))
                {
                    queryString = await StlEntityParser.ReplaceStlEntitiesForAttributeValueAsync(value, pageInfo, contextInfo);
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

            var parsedContent = await ParseImplAsync(pageInfo, contextInfo, channelIndex, channelName, upLevel, topLevel,
                removeTarget, href, queryString, host, attributes);

            return parsedContent;
        }

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, string channelIndex,
            string channelName, int upLevel, int topLevel, bool removeTarget, string href, string queryString,
            string host, Dictionary<string, string> attributes)
        {
            string htmlId;
            attributes.TryGetValue("id", out htmlId);

            if (!string.IsNullOrEmpty(htmlId) && !string.IsNullOrEmpty(contextInfo.ContainerClientId))
            {
                htmlId = contextInfo.ContainerClientId + "_" + htmlId;
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
                url = PageUtility.ParseNavigationUrl(pageInfo.Site, href, pageInfo.IsLocal);

                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                innerHtml = innerBuilder.ToString();
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                {
                    contextInfo.ContextType = contextInfo.ContentId != 0 ? EContextType.Content : EContextType.Channel;
                }

                if (contextInfo.ContextType == EContextType.Content) //获取内容Url
                {
                    var contentInfo = await contextInfo.GetContentAsync();
                    if (contentInfo != null)
                    {
                        url = await PageUtility.GetContentUrlAsync(pageInfo.Site, contentInfo, pageInfo.IsLocal);
                    }
                    else
                    {
                        var nodeInfo = await ChannelManager.GetChannelAsync(pageInfo.SiteId, contextInfo.ChannelId);
                        url = await PageUtility.GetContentUrlAsync(pageInfo.Site, nodeInfo, contextInfo.ContentId,
                            pageInfo.IsLocal);
                    }

                    if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                    {
                        var title = contentInfo?.Title;
                        title = ContentUtility.FormatTitle(
                            contentInfo?.Get<string>("BackgroundContentAttribute.TitleFormatString"), title);

                        if (pageInfo.Site.IsContentTitleBreakLine)
                        {
                            title = title.Replace("  ", string.Empty);
                        }

                        innerHtml = title;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                        await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                        innerHtml = innerBuilder.ToString();
                    }
                }
                else if (contextInfo.ContextType == EContextType.Channel) //获取栏目Url
                {
                    contextInfo.ChannelId =
                        await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                    contextInfo.ChannelId =
                        await ChannelManager.GetChannelIdAsync(pageInfo.SiteId,
                            contextInfo.ChannelId, channelIndex, channelName);
                    var channel = await ChannelManager.GetChannelAsync(pageInfo.SiteId, contextInfo.ChannelId);

                    url = await PageUtility.GetChannelUrlAsync(pageInfo.Site, channel, pageInfo.IsLocal);
                    if (string.IsNullOrWhiteSpace(contextInfo.InnerHtml))
                    {
                        innerHtml = channel.ChannelName;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                        await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
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
                    url = PageUtils.AddProtocolToUrl(url, host);
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
            if (contextInfo.IsStlEntity)
            {
                return url;
            }

            return $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
    }
}
