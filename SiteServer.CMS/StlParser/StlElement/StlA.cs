using System.Text;
using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "获取链接", Description = "通过 stl:a 标签在模板中创建链接，系统将根据所处上下文计算链接地址")]
    public static class StlA
    {
        public const string ElementName = "stl:a";

        private static readonly Attr Id = new Attr("id", "唯一标识符");
        private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引", AttrType.Enum);
        private static readonly Attr ChannelName = new Attr("channelName", "栏目名称", AttrType.Enum);
        private static readonly Attr Parent = new Attr("parent", "显示父栏目", AttrType.Boolean);
        private static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别", AttrType.Integer);
        private static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别", AttrType.Integer);
        private static readonly Attr Context = new Attr("context", "所处上下文", AttrType.Enum);
        private static readonly Attr Href = new Attr("href", "链接地址");
        private static readonly Attr Host = new Attr("host", "链接域名");
        private static readonly Attr QueryString = new Attr("queryString", "链接参数");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlAnchor = new HtmlAnchor();
            var htmlId = string.Empty;
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
                if (StringUtils.EqualsIgnoreCase(name, Id.Name))
                {
                    htmlId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent.Name))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel.Name))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel.Name))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        contextInfo.ContextType = EContextType.Channel;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context.Name))
                {
                    contextInfo.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Href.Name))
                {
                    href = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, QueryString.Name))
                {
                    queryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Host.Name))
                {
                    host = value;
                } 
                else
                {
                    ControlUtils.AddAttributeIfNotExists(stlAnchor, name, value);
                }
            } 

            var parsedContent = ParseImpl(pageInfo, contextInfo, stlAnchor, htmlId, channelIndex, channelName, upLevel, topLevel, removeTarget, href, queryString, host);

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string htmlId, string channelIndex, string channelName, int upLevel, int topLevel, bool removeTarget, string href, string queryString, string host)
        {
            if (!string.IsNullOrEmpty(htmlId) && !string.IsNullOrEmpty(contextInfo.ContainerClientId))
            {
                htmlId = contextInfo.ContainerClientId + "_" + htmlId;
            }
            stlAnchor.ID = htmlId;

            var url = string.Empty;
            var onclick = string.Empty;
            if (!string.IsNullOrEmpty(href))
            {
                url = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, href, pageInfo.IsLocal);

                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            else
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                {
                    contextInfo.ContextType = contextInfo.ContentId != 0 ? EContextType.Content : EContextType.Channel;
                }
                if (contextInfo.ContextType == EContextType.Content)//获取内容Url
                {
                    if (contextInfo.ContentInfo != null)
                    {
                        url = PageUtility.GetContentUrl(pageInfo.SiteInfo, contextInfo.ContentInfo, pageInfo.IsLocal);
                    }
                    else
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);
                        url = PageUtility.GetContentUrl(pageInfo.SiteInfo, nodeInfo, contextInfo.ContentId, pageInfo.IsLocal);
                    }
                    if (string.IsNullOrEmpty(contextInfo.InnerHtml))
                    {
                        var title = contextInfo.ContentInfo?.Title;
                        title = ContentUtility.FormatTitle(contextInfo.ContentInfo?.GetString("BackgroundContentAttribute.TitleFormatString"), title);

                        if (pageInfo.SiteInfo.Additional.IsContentTitleBreakLine)
                        {
                            title = title.Replace("  ", string.Empty);
                        }

                        stlAnchor.InnerHtml = title;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        stlAnchor.InnerHtml = innerBuilder.ToString();
                    }
                }
                else if (contextInfo.ContextType == EContextType.Channel)//获取栏目Url
                {
                    contextInfo.ChannelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);
                    contextInfo.ChannelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, contextInfo.ChannelId, channelIndex, channelName);
                    var channel = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);

                    url = PageUtility.GetChannelUrl(pageInfo.SiteInfo, channel, pageInfo.IsLocal);
                    if (string.IsNullOrWhiteSpace(contextInfo.InnerHtml))
                    {
                        stlAnchor.InnerHtml = channel.ChannelName;
                    }
                    else
                    {
                        var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                        StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                        stlAnchor.InnerHtml = innerBuilder.ToString();
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

            stlAnchor.HRef = url;

            if (!string.IsNullOrEmpty(onclick))
            {
                stlAnchor.Attributes.Add("onclick", onclick);
            }

            if (removeTarget)
            {
                stlAnchor.Target = string.Empty;
            }

            // 如果是实体标签，则只返回url
            if (contextInfo.IsStlEntity)
            {
                return stlAnchor.HRef;
            }
            else
            {
                return ControlUtils.GetControlRenderHtml(stlAnchor);
            }
        }
    }
}
